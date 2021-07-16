#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor
{
    [CustomEditor(typeof(WorldObserver))]
    public class WorldViewer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var observer = (WorldObserver) target;
            var worldInfo = observer.GetInfo();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Components: ", worldInfo.ComponentsCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Archetypes: ", worldInfo.ArchetypesCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Active entities: ", worldInfo.EntitiesCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Reserved entities: ", worldInfo.FreeEntitiesCount.ToString());
            GUILayout.EndVertical();
        }
    }
}
#endif