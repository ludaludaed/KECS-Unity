#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Ludaludaed.KECS.Unity.Editor
{
    public class TemplateGenerator : ScriptableObject
    {
        private const string Title = "KECS template generator";
        private const string StartupTemplate = "Startup.cs.txt";
        private const string BaseSystemTemplate = "BaseSystem.cs.txt";
        private const string UpdateSystemTemplate = "UpdateSystem.cs.txt";
        private const string ComponentTemplate = "Component.cs.txt";

        [MenuItem("Assets/Create/KECS/Create Startup", false, -200)]
        private static void CreateStartupTemplate()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/Startup.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                (name) => CreateTemplateInternal(GetTemplateContent(StartupTemplate), name));
        }

        [MenuItem("Assets/Create/KECS/Create Component", false, -199)]
        private static void CreateComponentTemplate()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/Component.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                (name) => CreateTemplateInternal(GetTemplateContent(ComponentTemplate), name, "Provider"));
        }

        [MenuItem("Assets/Create/KECS/Systems/Create BaseSystem", false, -198)]
        private static void CreateBaseSystemTemplate()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/BaseSystem.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                (name) => CreateTemplateInternal(GetTemplateContent(BaseSystemTemplate), name));
        }

        [MenuItem("Assets/Create/KECS/Systems/Create UpdateSystem", false, -197)]
        private static void CreateUpdateSystemTemplate()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/UpdateSystem.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                (name) => CreateTemplateInternal(GetTemplateContent(UpdateSystemTemplate), name));
        }

        private static string CreateTemplate(string proto, string fileName, string suffix)
        {
            if (string.IsNullOrEmpty(fileName)) 
                return "Invalid filename";
            
            var ns = EditorSettings.projectGenerationRootNamespace.Trim();
            if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace)) 
                ns = "DefaultNS";
            

            proto = proto.Replace("#NS#", ns);
            proto = proto.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(fileName));
            try
            {
                File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName.Replace(".cs", "") + suffix + ".cs"), proto);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            AssetDatabase.Refresh();
            return null;
        }

        private static void CreateTemplateInternal(string proto, string fileName, string suffix = "")
        {
            var res = CreateTemplate(proto, fileName, suffix);
            if (res != null)
            {
                EditorUtility.DisplayDialog(Title, res, "Close");
            }
        }

        private static string GetTemplateContent(string proto)
        {
            var pathHelper = CreateInstance<TemplateGenerator>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(pathHelper)));
            DestroyImmediate(pathHelper);
            try
            {
                return File.ReadAllText(Path.Combine(path ?? "", proto));
            }
            catch
            {
                return null;
            }
        }

        private static string GetAssetPath()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.Contains(Selection.activeObject))
            {
                if (!AssetDatabase.IsValidFolder(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }
            else
            {
                path = "Assets";
            }

            return path;
        }

        private static void CreateAndRenameAsset(string fileName, Texture2D icon, Action<string> onSuccess)
        {
            var action = CreateInstance<CustomEndNameAction>();
            action.Callback = onSuccess;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, fileName, icon, null);
        }

        private sealed class CustomEndNameAction : EndNameEditAction
        {
            [NonSerialized] public Action<string> Callback;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Callback?.Invoke(pathName);
            }
        }
    }
}
#endif