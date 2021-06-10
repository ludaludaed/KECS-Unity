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
            EditorGUILayout.LabelField("Components: ", worldInfo.Components.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Archetypes: ", worldInfo.Archetypes.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Active entities: ", worldInfo.ActiveEntities.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Reserved entities: ", worldInfo.ReservedEntities.ToString());
            GUILayout.EndVertical();
        }
    }
}
#endif