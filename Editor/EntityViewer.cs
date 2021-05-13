using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    [CustomEditor(typeof(EntityObserver))]
    public class EntityViewer : Editor
    {
        private static object[] _componentsCache = new object[32];
        private static int[] _componentsIndexes = new int[32];

        private EntityObserver _observer;


        public override void OnInspectorGUI()
        {
            if (targets.Length == 1)
            {
                _observer = (EntityObserver) target;
                DrawEntity(_observer.Entity);
            }

            if (target != null)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawEntity(Entity entity)
        {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (entity.IsEmpty() || !entity.IsAlive())
            {
                GUI.backgroundColor = Color.grey;
            }

            if (GUILayout.Button("Destroy Entity"))
            {
                if (!entity.IsEmpty() && entity.IsAlive())
                {
                    entity.Destroy();
                }
            }

            GUI.backgroundColor = bgColor;

            if (_observer.gameObject.activeSelf && _observer.Entity.IsAlive())
            {
                DrawComponents();
            }
        }

        private void DrawComponents()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawAddComponentMenu(_observer.Entity);
            var count = _observer.Entity.GetComponents(ref _componentsCache);
            _observer.Entity.GetComponentsIndexes(ref _componentsIndexes);
            var colors = DrawHelper.GetColoredBoxStyle(count);
            var boldText = new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold};

            EditorGUILayout.LabelField($"Components({count})", boldText);
            EditorGUILayout.Space();

            for (var i = 0; i < count; i++)
            {
                DrawComponent(i, colors[i]);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawComponent(int idx, GUIStyle style)
        {
            var component = _componentsCache[idx];
            var componentIndex = _componentsIndexes[idx];
            var type = component.GetType();

            ArrayExtension.EnsureLength(ref _observer.unfoldedComponents, componentIndex);

            var memberInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            GUILayout.BeginVertical(style);
            EditorGUILayout.BeginHorizontal();

            var boldText = new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold};

            var boldFoldout = EditorStyles.foldout;
            boldFoldout.fontStyle = FontStyle.Bold;

            if (memberInfos.Length > 0)
            {
                GUILayout.Space(15);
                _observer.unfoldedComponents[componentIndex] = !EditorGUILayout.Foldout(
                    !_observer.unfoldedComponents[componentIndex], type.Name, true, boldFoldout);
            }
            else
            {
                GUILayout.Space(15);
                EditorGUILayout.LabelField(type.Name, boldText);
            }

            if (GUILayout.Button("✘", GUILayout.Width(19), GUILayout.Height(19)))
            {
                _observer.Entity.Remove(componentIndex);
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (!_observer.unfoldedComponents[componentIndex] && memberInfos.Length > 0)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                var changed = false;

                foreach (var info in memberInfos)
                {
                    if (DrawHelper.DrawField(info, component, info.SetValue))
                    {
                        changed = true;
                    }
                }

                if (changed)
                {
                    _observer.Entity.Set(component, componentIndex);
                }

                EditorGUI.indentLevel = indent;
            }

            GUILayout.EndVertical();
        }

        private static void DrawAddComponentMenu(Entity entity)
        {
            var componentInfos = new List<EcsTypeManager.TypeInfo>();
            var componentNames = new List<string>() {"Add"};

            var arrayOfComponentsInfos = EcsTypeManager.ComponentsInfos;

            for (int i = 0, lenght = EcsTypeManager.ComponentTypesCount; i < lenght; i++)
            {
                if (!entity.Has(arrayOfComponentsInfos[i].Index))
                {
                    componentInfos.Add(arrayOfComponentsInfos[i]);
                    componentNames.Add(arrayOfComponentsInfos[i].Type.Name);
                }
            }

            var index = EditorGUILayout.Popup(0, componentNames.ToArray());

            if (index > 0)
            {
                entity.Set(Activator.CreateInstance(componentInfos[index - 1].Type), componentInfos[index - 1].Index);
            }
        }
    }
}