using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FasterGames.Aseprite.Editor.Library
{
    [Serializable]
    public class FrameOptions
    {
        [Serializable]
        public class LayerFilter
        {
            public enum LayerFilterType
            {
                Equals,
                Prefix,
                Contains,
                Postfix,
                Regex
            }

            public string name;
            public LayerFilterType type;
            public string content;

            public bool Matches(string data)
            {
                return type switch
                {
                    LayerFilterType.Equals => data.Equals(content, StringComparison.Ordinal),
                    LayerFilterType.Contains => data.Contains(content),
                    LayerFilterType.Postfix => data.EndsWith(content),
                    LayerFilterType.Prefix => data.StartsWith(content),
                    LayerFilterType.Regex => new Regex(content).IsMatch(data),
                    _ => throw new NotImplementedException()
                };
            }
        }

        public List<LayerFilter> include;
        public List<LayerFilter> exclude;

        public bool Includes(string layerName)
        {
            // neither null or empty
            if (!IsNullOrEmpty(include) && !IsNullOrEmpty(exclude))
            {
                var matchedInclude = include.Any(f => f.Matches(layerName));
                var matchedExclude = exclude.Any(f => f.Matches(layerName));

                return matchedInclude && !matchedExclude;
            }
            // only include present
            else if (!IsNullOrEmpty(include) && IsNullOrEmpty(exclude))
            {
                return include.Any(f => f.Matches(layerName));
            }
            // only exclude present
            else if (IsNullOrEmpty(include) && !IsNullOrEmpty(exclude))
            {
                return !exclude.Any(f => f.Matches(layerName));
            }
            // neither present
            else
            {
                return true;
            }
        }

        private bool IsNullOrEmpty<TList>(List<TList> list)
        {
            return list == null || list.Count == 0;
        }
    }
}