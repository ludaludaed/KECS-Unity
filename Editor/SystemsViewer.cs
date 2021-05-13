using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    [CustomEditor(typeof(SystemsObserver))]
    public class SystemsViewer : Editor
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

        private static void SystemDraw(List<SystemData> data)
        {
            EditorGUI.indentLevel++;
            var colors = DrawHelper.GetColoredBoxStyle(data.Count);
            for (int i = 0, lenght = data.Count; i < lenght; i++)
            {
                GUILayout.BeginVertical(colors[i]);
                var runItem = data[i];
                var type = runItem.Base.GetType();
                runItem.IsEnable = EditorGUILayout.ToggleLeft(type.Name, runItem.IsEnable);
                GUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        public override void OnInspectorGUI()
        {
            var savedState = GUI.enabled;
            GUI.enabled = true;
            var systemsGroup = _observer.GetSystems();

            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Base systems", EditorStyles.boldLabel);
            SystemDraw(systemsGroup.GetOnlyBaseSystems());
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Update systems", EditorStyles.boldLabel);
            SystemDraw(systemsGroup.GetUpdateSystems());
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("FixedUpdate systems", EditorStyles.boldLabel);
            SystemDraw(systemsGroup.GetFixedUpdateSystems());
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("LateUpdate systems", EditorStyles.boldLabel);
            SystemDraw(systemsGroup.GetLateUpdateSystems());
            GUILayout.EndVertical();

            GUI.enabled = savedState;
        }
    }
}