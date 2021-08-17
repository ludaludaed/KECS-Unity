#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ludaludaed.KECS.Unity.Editor
{
    internal static class EntityEditorExtensions
    {
        private static readonly MethodInfo _setComponentMethod;
        private static readonly MethodInfo _removeComponentMethod;
        private static readonly MethodInfo _hasComponentMethod;
        private static readonly MethodInfo _regComponentMethod;

        static EntityEditorExtensions()
        {
            _setComponentMethod = typeof(EntityEditorExtensions).GetMethod("SetComponent");
            _removeComponentMethod = typeof(EntityEditorExtensions).GetMethod("RemoveComponent");
            _hasComponentMethod = typeof(EntityEditorExtensions).GetMethod("HasComponent");
            _regComponentMethod = typeof(EntityEditorExtensions).GetMethod("Registration");
            Init();
        }

        private static void Init()
        {
            var attrType = typeof(AutoRegistrationComponentAttribute);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = GetTypesWithAttribute(attrType, assembly);

                foreach (var type in types)
                {
                    var regComponentGeneric = _regComponentMethod.MakeGenericMethod(type);
                    regComponentGeneric.Invoke(null, null);
                }
            }
        }

        private static IEnumerable<Type> GetTypesWithAttribute(Type attributeType, Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0);
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