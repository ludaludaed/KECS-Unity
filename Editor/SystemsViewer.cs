#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor
{
    [CustomEditor(typeof(SystemsObserver))]
    public class SystemsViewer : UnityEditor.Editor
    {
        private SystemsObserver _observer;

        private void OnEnable()
        {
            _observer = (SystemsObserver) target;
        }

        private void OnDisable()
        {
            _observer = null;
        }

        private static void SystemDraw(Systems systems)
        {
            EditorGUI.indentLevel++;
            var data = systems.GetSystems();
            var colors = DrawHelper.GetColoredBoxStyle(data.Count);
            for (int i = 0, lenght = data.Count; i < lenght; i++)
            {
                var runItem = data.Get(i);
                if (runItem is Systems systemsItem)
                {
                    if (runItem.IsEnable)
                    {
                        GUILayout.BeginVertical(GUI.skin.box);
                        runItem.IsEnable = EditorGUILayout.ToggleLeft(systemsItem.Name, runItem.IsEnable, EditorStyles.boldLabel);
                        SystemDraw(systemsItem);
                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.BeginVertical(colors[i]);
                        runItem.IsEnable = EditorGUILayout.ToggleLeft(systemsItem.Name, runItem.IsEnable, EditorStyles.boldLabel);
                        GUILayout.EndVertical();
                    }
                }
                else
                {
                    GUILayout.BeginVertical(colors[i]);
                    var type = runItem.GetType();
                    runItem.IsEnable = EditorGUILayout.ToggleLeft(type.Name, runItem.IsEnable);
                    GUILayout.EndVertical();
                }
            }
            EditorGUI.indentLevel--;
        }

        public override void OnInspectorGUI()
        {
            var savedState = GUI.enabled;
            GUI.enabled = true;
            var systemsGroup = _observer.GetSystems();
            if(systemsGroup == null) return;

            foreach (var group in systemsGroup)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(group.Name, EditorStyles.boldLabel);
                SystemDraw(group);
                GUILayout.EndVertical();
            }
            
            GUI.enabled = savedState;
        }
    }
}
#endif