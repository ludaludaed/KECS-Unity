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

        private static void SystemDraw(FastList<SystemGroup.SystemData> data)
        {
            EditorGUI.indentLevel++;
            var colors = DrawHelper.GetColoredBoxStyle(data.Count);
            for (int i = 0, lenght = data.Count; i < lenght; i++)
            {
                GUILayout.BeginVertical(colors[i]);
                var runItem = data.Get(i);
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
            if(systemsGroup == null) return;

            foreach (var group in systemsGroup)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(group.Name, EditorStyles.boldLabel);
                
                var update = group.GetUpdateSystems();
                if (update.Count != 0)
                {
                    SystemDraw(group.GetUpdateSystems());
                }
                GUILayout.EndVertical();
            }
            
            GUI.enabled = savedState;
        }
    }
}
#endif