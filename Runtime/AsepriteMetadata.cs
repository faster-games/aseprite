using System;
using System.IO;
using UnityEngine;

namespace FasterGames.Aseprite
{
    [Serializable]
    public class AsepriteMetadata : ScriptableObject
    {
        /// <summary>
        /// Formats a name of an Aseprite sub-resource
        /// </summary>
        /// <param name="assetPath">asset path</param>
        /// <param name="layerName">sub-resource layer name</param>
        /// <param name="itemName">sub-resource item name</param>
        /// <returns>name</returns>
        public static string FormatName(string assetPath, string layerName, string itemName)
        {
            var assetName = Path.GetFileNameWithoutExtension(assetPath);

            return $"{assetName}_{layerName ?? "all"}" + ((itemName != null) ? $"_{itemName}" : "");
        }
        
        public int layerCount;
        public int frameCount;
        public int width;
        public int height;
        public string[] layerNames;
        public Texture2D[] layerTextures;
    }
}