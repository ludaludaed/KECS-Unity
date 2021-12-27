#if UNITY_EDITOR
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor {
    public interface ITypeDrawer {
        bool IsTypeDrawer(Type type);
        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }

    public class AnimationCurveTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(AnimationCurve);

        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.CurveField(memberName.ClearString(), (AnimationCurve) value);
        }
    }

    public class BoolTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(bool);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.Toggle(memberName.ClearString(), (bool) value);
        }
    }

    public class BoundsTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(Bounds);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.BoundsField(memberName.ClearString(), (Bounds) value);
        }
    }

    public class CharTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(char);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            var str = EditorGUILayout.TextField(memberName.ClearString(), ((char) value).ToString());
            return str.Length > 0 ? str[0] : default(char);
        }
    }

    public class ColorTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(Color);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.ColorField(memberName.ClearString(), (Color) value);
        }
    }

    public class FloatTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(float);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.FloatField(memberName.ClearString(), (float) value);
        }
    }

    public class IntTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(int);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.IntField(memberName.ClearString(), (int) value);
        }
    }

    public class RectTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(Rect);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.RectField(memberName.ClearString(), (Rect) value);
        }
    }

    public class StringTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(string);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.TextField(memberName.ClearString(), (string) value);
        }
    }

    public class UnityObjectTypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(UnityEngine.Object);
        public bool IsTypeDrawer(Type type) => type == _type || type.IsSubclassOf(_type);

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.ObjectField(memberName.ClearString(), (UnityEngine.Object) value, memberType, true);
        }
    }

    public class Vector2TypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(Vector2);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.Vector2Field(memberName.ClearString(), (Vector2) value);
        }
    }

    public class Vector3TypeDrawer : ITypeDrawer {
        private readonly Type _type = typeof(Vector3);
        public bool IsTypeDrawer(Type type) => type == _type;

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
            return EditorGUILayout.Vector3Field(memberName.ClearString(), (Vector3) value);
        }
    }

    public static class StringHelper {
        public static string ClearString(this string src) {
            src = src.Replace("_", "");
            var sb = new StringBuilder();
            var needUp = true;
            foreach (var c in src) {
                if (char.IsLetterOrDigit(c)) {
                    sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                    needUp = false;
                } else {
                    needUp = true;
                }
            }

            return sb.ToString();
        }
    }
}
#endif