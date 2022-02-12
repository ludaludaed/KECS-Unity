using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludaludaed.KECS.Unity.Editor {
    [CustomEditor(typeof(EntityObserver))]
    public class EntityViewer : UnityEditor.Editor {
        private static (int, object)[] _componentsCache = new (int, object)[32];
        private static Type[] _componentsInfosCache = new Type[32];

        private EntityObserver _observer;

        public override void OnInspectorGUI() {
            if (targets.Length == 1) {
                _observer = (EntityObserver) target;
                DrawEntity(_observer.Entity);
            }

            if (target != null) {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawEntity(Entity entity) {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (entity.IsEmpty() || !entity.IsAlive()) GUI.backgroundColor = Color.grey;
            if (GUILayout.Button("Destroy Entity")) {
                if (!entity.IsEmpty() && entity.IsAlive()) entity.Destroy();
            }

            GUI.backgroundColor = bgColor;

            if (_observer.gameObject.activeSelf && _observer.Entity.IsAlive()) {
                DrawComponents();
            }
        }

        private void DrawComponents() {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawAddComponentMenu(_observer.Entity);

            var count = _observer.Entity.GetComponents(ref _componentsCache);

            var colors = DrawHelper.GetColoredBoxStyle(count);
            var boldText = new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold};

            EditorGUILayout.LabelField($"Components({count})", boldText);
            EditorGUILayout.Space();

            for (var i = 0; i < count; i++) {
                DrawComponent(i, colors[i]);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawComponent(int idx, GUIStyle style) {
            var component = _componentsCache[idx].Item2;
            var componentIndex = _componentsCache[idx].Item1;
            var type = component.GetType();
            var memberInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            GUILayout.BeginVertical(style);

            EditorGUILayout.BeginHorizontal();
            var boldFoldout = EditorStyles.foldout;
            boldFoldout.fontStyle = FontStyle.Bold;
            GUILayout.Space(15);
            if (memberInfos.Length > 0) {
                var foldout = EditorGUILayout.Foldout(_observer.unfoldedComponents.GetBit(componentIndex),
                    type.GetCleanGenericTypeName(), true, boldFoldout);
                if (foldout) {
                    _observer.unfoldedComponents.SetBit(componentIndex);
                } else {
                    _observer.unfoldedComponents.ClearBit(componentIndex);
                }
            } else {
                EditorGUILayout.LabelField(type.GetCleanGenericTypeName(),
                    new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
                _observer.unfoldedComponents.ClearBit(componentIndex);
            }

            if (GUILayout.Button("✘", GUILayout.Width(19), GUILayout.Height(19))) {
                _observer.Entity.RemoveExecute(type);
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (_observer.unfoldedComponents.GetBit(componentIndex)) {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                foreach (var info in memberInfos) {
                    DrawComponentField(info, component, _observer.Entity);
                }

                EditorGUI.indentLevel = indent;
            }

            GUILayout.EndVertical();
        }

        private static void DrawComponentField(FieldInfo field, object target, Entity entity) {
            var fieldValue = field.GetValue(target);
            var fieldType = field.FieldType;
            var (renderer, changed, newValue) = InspectorDrawer.Draw(field.Name, fieldType, fieldValue);
            if (!renderer) {
                if (fieldType == typeof(Object) || fieldType.IsSubclassOf(typeof(Object))) {
                    GUILayout.BeginVertical();
                    var newObjValue = EditorGUILayout.ObjectField(field.Name.ClearString(), (Object) fieldValue,
                        fieldType, true);
                    if (newObjValue != (Object) fieldValue) {
                        field.SetValue(target, newObjValue);
                        entity.SetExecute(target);
                    }

                    GUILayout.EndVertical();
                } else if (fieldType.IsEnum) {
                    var (enumChanged, enumNewValue) = InspectorDrawer.DrawEnum(field.Name, fieldValue,
                        Attribute.IsDefined(fieldType, typeof(FlagsAttribute)));
                    if (!enumChanged) return;
                    field.SetValue(target, enumNewValue);
                    entity.SetExecute(target);
                } else {
                    EditorGUILayout.LabelField(field.Name, fieldValue.ToString(),
                        new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
                }
            } else {
                if (changed) {
                    field.SetValue(target, newValue);
                    entity.SetExecute(target);
                }
            }
        }

        private static void DrawAddComponentMenu(in Entity entity) {
            var componentInfos = new List<Type>();
            var componentNames = new List<string>() {"Add"};

            var length = EcsTypeManager.GetAllTypes(ref _componentsInfosCache);

            for (var i = 0; i < length; i++) {
                if (entity.HasExecute(_componentsInfosCache[i])) continue;
                componentInfos.Add(_componentsInfosCache[i]);
                componentNames.Add(_componentsInfosCache[i].GetCleanGenericTypeName());
            }

            var index = EditorGUILayout.Popup(0, componentNames.ToArray());
            if (index == 0) return;
            entity.SetExecute(Activator.CreateInstance(componentInfos[index - 1]));
        }
    }
}