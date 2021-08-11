using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEngine;

namespace FasterGames.Aseprite.Editor.Library.PixelFormats
{
    public class IndexedPixel : Pixel
    {
        public byte Index { get; private set; }

        public IndexedPixel(Frame frame, byte index) : base(frame)
        {
            Index = index;
        }

        public override Color GetColor()
        {
            PaletteChunk palette = Frame.File.GetChunk<PaletteChunk>();

            if (palette != null)
                return palette.GetColor(Index);
            else
                return Color.magenta;
        }
    }
}
