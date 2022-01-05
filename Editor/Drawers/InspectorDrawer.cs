using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Ludaludaed.KECS.Unity.Editor {
    public static class InspectorDrawer {
        private static readonly Dictionary<Type, ITypeDrawer> _typeDrawers;

        static InspectorDrawer() {
            _typeDrawers = new Dictionary<Type, ITypeDrawer>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (!typeof(ITypeDrawer).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract) continue;
                    if (Activator.CreateInstance(type) is ITypeDrawer inspector) {
                        if (_typeDrawers.ContainsKey (inspector.Type)) {
                            Debug.LogWarningFormat ($"Inspector for {inspector.Type.Name} already exists, new inspector will be used instead.");
                        }
                        _typeDrawers.Add(inspector.Type, inspector);
                    }
                }
            }
        }

        public static (bool, bool, object) Draw(string label, Type type, object value) {
            if (!_typeDrawers.TryGetValue(type, out var drawer)) return (false, false, null);
            var (changed, newValue) = drawer.OnDraw(label, value);
            return (true, changed, newValue);
        }
        
        public static (bool, object) DrawEnum (string label, object value, bool isFlags) {
            var enumValue = (Enum) value;
            var newValue = isFlags ? EditorGUILayout.EnumFlagsField (label, enumValue) : EditorGUILayout.EnumPopup (label, enumValue);
            return Equals (newValue, value) ? (default, default) : (true, newValue);
        }
    }
}