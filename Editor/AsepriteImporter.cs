using System.IO;
using System.Linq;
using FasterGames.Aseprite.Editor.Importers;
using FasterGames.Aseprite.Editor.Library;
using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FasterGames.Aseprite.Editor
{
    [ScriptedImporter(version: 2, new []{"ase", "aseprite"})]
    public class AsepriteImporter : ScriptedImporter
    {
        public PrefabImporter.UserOptions prefabOptions = new PrefabImporter.UserOptions();
        public TextureSpriteImporter.UserOptions textureOptions = new TextureSpriteImporter.UserOptions();
        public AnimationRigImporter.UserOptions rigOptions = new AnimationRigImporter.UserOptions();

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // open and read the file
            using var fs = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
            var file = new AseFile(fs);
            
            var layerChunks = file.GetChunks<LayerChunk>();
            
            var meta = ScriptableObject.CreateInstance<AsepriteMetadata>();
            meta.name = AsepriteMetadata.FormatName(ctx.assetPath, null, null);
            meta.width = file.Header.Width;
            meta.height = file.Header.Height;
            meta.frameCount = file.Header.Frames;
            meta.layerCount = layerChunks.Count;
            meta.layerNames = layerChunks.Select(c => c.LayerName).ToArray();
            
            ctx.AddObjectToAsset(meta.name, meta);
            
            var prefabImporter = new PrefabImporter(file, prefabOptions);
            var rootPrefab = prefabImporter.ImportAsset(ctx);

            var textureImporter = new TextureSpriteImporter(file, textureOptions);

            if (!textureImporter.TryCanImportInto(ctx.assetPath))
            {
                // first import
                EditorUtils.Once(() => { SaveAndReimport(); });
                return;
            }

            var spritesByLayer = textureImporter.ImportAsset(ctx);

            var rigImporter = new AnimationRigImporter(file, new AnimationRigImporter.Options()
            {
                userOptions = rigOptions,
                rootObject = rootPrefab,
                spritesByLayerName = spritesByLayer
            });

            rigImporter.ImportAsset(ctx);
        }
    }
}