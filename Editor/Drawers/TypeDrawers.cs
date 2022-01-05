using System;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor {
    public interface ITypeDrawer {
        Type Type { get; }
        (bool, object) OnDraw(string label, object value);
    }

    public abstract class TypeDrawer<T> : ITypeDrawer {
        public Type Type { get; } = typeof(T);

        public (bool, object) OnDraw(string label, object value) {
            if (value == null) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(label, GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 16),
                    GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.LabelField("null", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                GUILayout.EndHorizontal();
                return (default, default);
            }

            var typedValue = (T) value;
            var changed = OnTypedDraw(label.ClearString(), ref typedValue);
            return changed ? (true, typedValue) : (default, default);
        }

        protected abstract bool OnTypedDraw(string label, ref T value);
    }

    public sealed class AnimationCurveTypeDrawer : TypeDrawer<AnimationCurve> {
        protected override bool OnTypedDraw(string label, ref AnimationCurve value) {
            var newValue = EditorGUILayout.CurveField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class BoolTypeDrawer : TypeDrawer<bool> {
        protected override bool OnTypedDraw(string label, ref bool value) {
            var newValue = EditorGUILayout.Toggle(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class BoundsTypeDrawer : TypeDrawer<Bounds> {
        protected override bool OnTypedDraw(string label, ref Bounds value) {
            var newValue = EditorGUILayout.BoundsField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class ColorTypeDrawer : TypeDrawer<Color> {
        protected override bool OnTypedDraw(string label, ref Color value) {
            var newValue = EditorGUILayout.ColorField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class FloatTypeDrawer : TypeDrawer<float> {
        protected override bool OnTypedDraw(string label, ref float value) {
            var newValue = EditorGUILayout.FloatField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class IntTypeDrawer : TypeDrawer<int> {
        protected override bool OnTypedDraw(string label, ref int value) {
            var newValue = EditorGUILayout.IntField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class StringTypeDrawer : TypeDrawer<string> {
        protected override bool OnTypedDraw(string label, ref string value) {
            var newValue = EditorGUILayout.TextField(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class Vector2Drawer : TypeDrawer<Vector2> {
        protected override bool OnTypedDraw(string label, ref Vector2 value) {
            var newValue = EditorGUILayout.Vector2Field(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class Vector3Drawer : TypeDrawer<Vector3> {
        protected override bool OnTypedDraw(string label, ref Vector3 value) {
            var newValue = EditorGUILayout.Vector3Field(label, value);
            if (newValue.Equals(value)) return false;
            value = newValue;
            return true;
        }
    }

    public sealed class QuaternionTypeDrawer : TypeDrawer<Quaternion> {
        protected override bool OnTypedDraw(string label, ref Quaternion value) {
            var eulerAngles = value.eulerAngles;
            var newValue = EditorGUILayout.Vector3Field(label, eulerAngles);
            if (newValue == eulerAngles) return false;
            value = Quaternion.Euler(newValue);
            return true;
        }
    }
}