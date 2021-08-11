using UnityEngine;

namespace FasterGames.Aseprite.Editor.Library
{
    public abstract class Pixel
    {
        protected Frame Frame = null;
        public abstract Color GetColor();

        public Pixel(Frame frame)
        {
            Frame = frame;
        }
    }
}

