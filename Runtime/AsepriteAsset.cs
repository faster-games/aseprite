using System;
using UnityEngine;

namespace FasterGames.Aseprite
{
    [Serializable]
    public class AsepriteAsset : ScriptableObject
    {
        public int layerCount;
        public int frameCount;
        public int width;
        public int height;
        public string[] layerNames;
        public Texture2D[] layerTextures;
    }
}