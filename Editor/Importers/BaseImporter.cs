using UnityEditor.AssetImporters;

namespace FasterGames.Aseprite.Editor.Importers
{
    public abstract class BaseImporter<TSource, TOutput>
    {
        protected TSource Source { get; private set; }

        public BaseImporter(TSource source)
        {
            this.Source = source;
        }

        public abstract TOutput ImportAsset(AssetImportContext ctx);
    }
}