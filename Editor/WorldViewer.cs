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
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Components: ", observer.GetInfo().ComponentsCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Archetypes: ", observer.GetInfo().ArchetypesCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Active entities: ", observer.GetInfo().EntitiesCount.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Reserved entities: ", observer.GetInfo().FreeEntitiesCount.ToString());
            GUILayout.EndVertical();
        }
    }
}
#endif