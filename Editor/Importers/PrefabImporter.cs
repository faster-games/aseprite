using System;
using System.IO;
using FasterGames.Aseprite.Editor.Library;
using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FasterGames.Aseprite.Editor.Importers
{
    public class PrefabImporter : BaseImporter<AseFile, GameObject>
    {   
        [Serializable]
        public class UserOptions
        {
            public string sortingLayerName = "Default";
            public int baseSortingOrder = 0;
        }

        private readonly UserOptions m_UserOptions;
        
        public PrefabImporter(AseFile source, UserOptions opts) : base(source)
        {
            this.m_UserOptions = opts;
        }

        public override GameObject ImportAsset(AssetImportContext ctx)
        {
            // get the layers, for future use
            var layerChunks = Source.GetChunks<LayerChunk>();
            
            var rootObject = new GameObject(AsepriteMetadata.FormatName(ctx.assetPath, null, null));
            rootObject.AddComponent<Animator>();
            
            // create the layer objects
            for (var layerIndex = 0; layerIndex < layerChunks.Count; layerIndex++)
            {
                var layer = layerChunks[layerIndex];
                var layerName = layer.LayerName;

                var layerObject = new GameObject(layerName);
                
                layerObject.transform.SetParent(rootObject.transform);
                
                var layerSprite = layerObject.AddComponent<SpriteRenderer>();
                layerSprite.sortingOrder = m_UserOptions.baseSortingOrder + layerIndex;
                layerSprite.sortingLayerName = m_UserOptions.sortingLayerName;
            }
            
            ctx.AddObjectToAsset(rootObject.name, rootObject);
            ctx.SetMainObject(rootObject);

            return rootObject;
        }
    }
}