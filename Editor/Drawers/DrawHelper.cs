using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor {
    public static class DrawHelper {
        public static GUIStyle[] GetColoredBoxStyle(int totalCount) {
            var styles = new GUIStyle[totalCount];
            for (var i = 0; i < totalCount; i++) {
                var hue = (float) i / totalCount;
                var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                componentColor.a = 0.15f;

                styles[i] = new GUIStyle(GUI.skin.box) {normal = {background = CreateTexture(componentColor)}};
            }

            return styles;
        }

        private static Texture2D CreateTexture(Color color) {
            var pixels = new[] {color};
            var result = new Texture2D(1, 1);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
    }
}