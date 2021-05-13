#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor
{
    [CustomEditor(typeof(ArchetypeObserver))]
    public class ArchetypeViewer : UnityEditor.Editor
    {
        private ArchetypeObserver _observer;

        public override void OnInspectorGUI()
        {
            var guiEnabled = GUI.enabled;
            GUI.enabled = true;

            var count = _observer.Archetype.Count;
            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField($"Entities: {count}", EditorStyles.boldLabel);
            if (count > 0)
            {
                var colors = DrawHelper.GetColoredBoxStyle(count);
                var ego = _observer.worldObserver.EntityGameObjects;

                var counter = 0;
                foreach (var entity in _observer.Archetype)
                {
                    if (!ego.TryGetValue(entity.Id, out var entityGo)) continue;
                    GUILayout.BeginVertical(colors[counter++]);
                    EditorGUILayout.LabelField(entityGo.name);
                    EditorGUILayout.ObjectField(entityGo, typeof(GameObject), true);
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
            GUI.enabled = guiEnabled;
            EditorUtility.SetDirty(target);
        }

        private void OnEnable()
        {
            _observer = target as ArchetypeObserver;
        }

        private void OnDisable()
        {
            _observer = null;
        }
    }
}
#endif