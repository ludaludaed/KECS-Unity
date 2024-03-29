using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor {
    [CustomEditor(typeof(SystemsObserver))]
    public class SystemsViewer : UnityEditor.Editor {
        private SystemsObserver _observer;

        private void OnEnable() {
            _observer = (SystemsObserver) target;
        }

        private void OnDisable() {
            _observer = null;
        }

        private static void SystemDraw(Systems systems) {
            EditorGUI.indentLevel++;
            var data = systems.GetSystems();
            for (int i = 0, lenght = data.Count; i < lenght; i++) {
                GUILayout.BeginVertical();
                var runItem = data[i];
                if (runItem is Systems systemsItem) {
                    runItem.IsEnable = EditorGUILayout.ToggleLeft($"System group: {systemsItem.Name}",
                        runItem.IsEnable, EditorStyles.boldLabel);
                    if (runItem.IsEnable) {
                        SystemDraw(systemsItem);
                    }
                } else {
                    var type = runItem.GetType();
                    runItem.IsEnable = EditorGUILayout.ToggleLeft(type.GetCleanGenericTypeName(), runItem.IsEnable);
                }
                GUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        public override void OnInspectorGUI() {
            var systemsGroup = _observer.Systems;
            if (systemsGroup == null) return;
            foreach (var group in systemsGroup) {
                GUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField($"System group: {group.Name}", EditorStyles.boldLabel);
                SystemDraw(group);
                GUILayout.EndVertical();
            }
        }
    }
}