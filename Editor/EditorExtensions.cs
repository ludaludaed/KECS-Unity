#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ludaludaed.KECS.Unity
{
    public interface IComponent
    {
    }
    internal static class EditorExtensions
    {
        private static readonly MethodInfo _setComponentMethod;
        private static readonly MethodInfo _removeComponentMethod;
        private static readonly MethodInfo _hasComponentMethod;
        private static readonly MethodInfo _regComponentMethod;

        static EditorExtensions()
        {
            _setComponentMethod = typeof(EditorExtensions).GetMethod("SetComponent");
            _removeComponentMethod = typeof(EditorExtensions).GetMethod("RemoveComponent");
            _hasComponentMethod = typeof(EditorExtensions).GetMethod("HasComponent");
            _regComponentMethod = typeof(EditorExtensions).GetMethod("Registration");
            Init();
        }

        private static void Init()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IComponent).IsAssignableFrom(type) || type.IsInterface) continue;
                    var regComponentGeneric = _regComponentMethod.MakeGenericMethod(type);
                    regComponentGeneric.Invoke(null,null);
                }
            }
        }

        internal static void SetExecute(this in Entity entity, object component)
        {
            var addComponentGeneric = _setComponentMethod.MakeGenericMethod(component.GetType());
            addComponentGeneric.Invoke(null, new[] {entity, component});
        }
        
        internal static void RemoveExecute(this in Entity entity, Type component)
        {
            var addComponentGeneric = _removeComponentMethod.MakeGenericMethod(component);
            addComponentGeneric.Invoke(null, new object[] {entity});
        }
        
        internal static bool HasExecute(this in Entity entity, Type component)
        {
            var hasComponentGeneric = _hasComponentMethod.MakeGenericMethod(component);
            return (bool) hasComponentGeneric.Invoke(null, new object[] {entity});
        }
        
        public static void SetComponent<T>(this Entity entity, T component) where T : struct
        {
            entity.Set(component);
        }
        
        public static void RemoveComponent<T>(this Entity entity) where T : struct
        {
            entity.Remove<T>();
        }
        
        public static bool HasComponent<T>(this Entity entity) where T : struct
        {
            return entity.Has<T>();
        }
        
        public static void Registration<T>() where T : struct
        {
            var idx = ComponentTypeInfo<T>.TypeIndex;
        }
    }
}
#endif