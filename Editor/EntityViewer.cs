#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor {
    [CustomEditor(typeof(EntityObserver))]
    public class EntityViewer : UnityEditor.Editor {
        private static (int, object)[] _componentsCache = new (int, object)[32];

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

            ArrayExtension.EnsureLength(ref _observer.unfoldedComponents, componentIndex);

            var memberInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            GUILayout.BeginVertical(style);
            EditorGUILayout.BeginHorizontal();

            var boldText = new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold};

            var boldFoldout = EditorStyles.foldout;
            boldFoldout.fontStyle = FontStyle.Bold;

            GUILayout.Space(15);
            if (memberInfos.Length > 0) {
                _observer.unfoldedComponents[componentIndex] =
                    !EditorGUILayout.Foldout(!_observer.unfoldedComponents[componentIndex], type.Name, true,
                        boldFoldout);
            } else {
                EditorGUILayout.LabelField(type.Name, boldText);
            }

            if (GUILayout.Button("✘", GUILayout.Width(19), GUILayout.Height(19))) {
                _observer.Entity.RemoveExecute(type);
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (!_observer.unfoldedComponents[componentIndex] && memberInfos.Length > 0) {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                var changed = false;

                foreach (var info in memberInfos) {
                    if (DrawHelper.DrawField(info, component, info.SetValue)) changed = true;
                }

                if (changed) _observer.Entity.SetExecute(component);
                EditorGUI.indentLevel = indent;
            }

            GUILayout.EndVertical();
        }

        private static void DrawAddComponentMenu(in Entity entity) {
            var componentInfos = new List<Type>();
            var componentNames = new List<string>() {"Add"};

            var arrayOfComponentsInfos = EcsTypeManager.GetAllTypes();

            for (int i = 0, lenght = arrayOfComponentsInfos.Length; i < lenght; i++) {
                if (entity.HasExecute(arrayOfComponentsInfos[i])) continue;
                componentInfos.Add(arrayOfComponentsInfos[i]);
                componentNames.Add(arrayOfComponentsInfos[i].Name);
            }

            var index = EditorGUILayout.Popup(0, componentNames.ToArray());
            if (index == 0) return;
            entity.SetExecute(Activator.CreateInstance(componentInfos[index - 1]));
        }
    }
}
#endif