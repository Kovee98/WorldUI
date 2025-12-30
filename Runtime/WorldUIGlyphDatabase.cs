using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace WorldUI {
    [CreateAssetMenu(fileName = "GlyphDatabase", menuName = "WorldUI/Glyph Database")]
    public class WorldUIGlyphDatabase : ScriptableObject {
        // The main Sprite Asset that holds all your glyphs
        public TMP_SpriteAsset masterSpriteAsset;

        // A list of mappings from an InputBinding path to a glyph name
        [System.Serializable]
        public struct GlyphMapping {
            public string bindingPath;
            public string glyphName;
        }

        public GlyphMapping[] glyphMappings;

        private Dictionary<string, string> _glyphDictionary;

        private void OnEnable () {
            // On load, populate a dictionary for quick lookups
            _glyphDictionary = new Dictionary<string, string>();
            foreach (var mapping in glyphMappings) {
                _glyphDictionary[mapping.bindingPath] = mapping.glyphName;
            }
        }

        public string GetGlyphNameForPath (string path) {
            if (_glyphDictionary.TryGetValue(path, out string glyphName)) {
                return glyphName;
            }

            Debug.LogWarning($"No glyph found for path: {path}");

            // Return an empty string if no glyph is found
            return string.Empty;
        }
    }
}
