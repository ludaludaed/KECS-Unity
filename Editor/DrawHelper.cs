#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor
{
    public static class DrawHelper
    {
        private static readonly ITypeDrawer[] _typeDrawers;
        private static readonly int _countOfDrawers;

        static DrawHelper()
        {
            var counter = 0;
            _typeDrawers = new ITypeDrawer[64];
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ITypeDrawer).IsAssignableFrom(type) || type.IsInterface) continue;
                    if (!(Activator.CreateInstance(type) is ITypeDrawer inspector)) continue;
                    ArrayExtension.EnsureLength(ref _typeDrawers, counter);
                    _typeDrawers[counter++] = inspector;
                }
            }
            _countOfDrawers = counter;
        }

        private static bool TryGetDrawer(Type type, out ITypeDrawer drawer)
        {
            drawer = null;
            for (int i = 0; i < _countOfDrawers; i++)
            {
                if (_typeDrawers[i].IsTypeDrawer(type))
                {
                    drawer = _typeDrawers[i];
                    return true;
                }
            }
            return false;
        }

        public static GUIStyle[] GetColoredBoxStyle(int totalCount)
        {
            var styles = new GUIStyle[totalCount];
            for (var i = 0; i < totalCount; i++)
            {
                var hue = (float) i / totalCount;
                var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                componentColor.a = 0.15f;

                styles[i] = new GUIStyle(GUI.skin.box) {normal = {background = CreateTexture(2, 2, componentColor)}};
            }

            return styles;
        }

        private static Texture2D CreateTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static bool DrawField(FieldInfo field, object target, Action<object, object> setValue)
        {
            var fieldValue = field.GetValue(target);
            var fieldType = field.FieldType;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();

            if (TryGetDrawer(fieldType, out var drawer))
            {
                var newValue = drawer.DrawAndGetNewValue(fieldType, field.Name, fieldValue, target);
                setValue(target, newValue);
            }
            else
            {
                EditorGUILayout.LabelField(field.Name, fieldValue.ToString());

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                EditorGUILayout.BeginVertical();

                var memberInfos = fieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (var info in memberInfos)
                {
                    DrawField(info, fieldValue, info.SetValue);
                    if (fieldType.IsValueType)
                    {
                        setValue(target, fieldValue);
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = indent;
            }
            
            EditorGUILayout.EndVertical();
            return EditorGUI.EndChangeCheck();
        }
    }
}
#endif