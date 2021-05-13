#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class WorldObserver : MonoBehaviour, IWorldDebugListener
    {
        public World World;

        private Transform _entitiesGO;
        private Transform _archetypesGO;

        private bool _isDestroyed = false;

        public readonly Dictionary<int, GameObject> EntityGameObjects = new Dictionary<int, GameObject>(1024);

        public static GameObject Create(World world)
        {
            if (world == null)
            {
                throw new Exception("World is null and cannot be observable");
            }

            var gameObj = new GameObject($"[WORLD] {world.Name}");
            DontDestroyOnLoad(gameObj);

            var observer = gameObj.AddComponent<WorldObserver>();
            observer.World = world;
            var worldTransform = observer.transform;

            observer._entitiesGO = new GameObject("Entities").transform;
            observer._entitiesGO.SetParent(worldTransform, false);

            observer._archetypesGO = new GameObject("Archetypes").transform;
            observer._archetypesGO.SetParent(worldTransform, false);

            world.AddDebugListener(observer);

            return gameObj;
        }

        public WorldInfo GetInfo()
        {
            return World.GetInfo();
        }

        public void OnEntityCreated(in Entity entity)
        {
            if (_isDestroyed) return;
            if (!EntityGameObjects.TryGetValue(entity.Id, out var go))
            {
                go = new GameObject();
                go.transform.SetParent(_entitiesGO, false);
                var unityEntity = go.AddComponent<EntityObserver>();
                unityEntity.World = World;
                unityEntity.Entity = entity;
                EntityGameObjects[entity.Id] = go;
            }
            else
            {
                go.GetComponent<EntityObserver>().Entity = entity;
            }

            go.name = entity.ToString();
            go.SetActive(true);
        }

        public void OnEntityDestroyed(in Entity entity)
        {
            if (_isDestroyed) return;
            if (EntityGameObjects.TryGetValue(entity.Id, out var go))
            {
                go.name = entity.ToString();
                go.SetActive(false);
            }
        }

        public void OnArchetypeCreated(Archetype archetype)
        {
            if (_isDestroyed) return;
            var go = new GameObject();
            go.transform.SetParent(_archetypesGO);
            var observer = go.AddComponent<ArchetypeObserver>();

            observer.worldObserver = this;
            observer.World = World;
            observer.Archetype = archetype;

            var goName = "Archetype ";
            foreach (var type in archetype.TypesCache)
            {
                goName += $"[{type.Name}] ";
            }

            go.name = goName;
        }

        public void OnWorldDestroyed(World world)
        {
            OnDestroy();
            Destroy(gameObject);
        }

        public void OnDestroy()
        {
            if (World == null) return;
            World.RemoveDebugListener(this);
            World = null;
        }
    }

    public sealed class SystemsObserver : MonoBehaviour, ISystemsDebugListener
    {
        private Systems _systems;

        public static GameObject Create(Systems systems, Transform worldParent)
        {
            if (systems == null)
            {
                throw new ArgumentNullException(nameof(systems));
            }

            var go = new GameObject( $"[SYSTEMS] {systems.Name}");
            DontDestroyOnLoad(go);
            go.transform.SetParent(worldParent);
            var observer = go.AddComponent<SystemsObserver>();
            observer._systems = systems;
            systems.AddDebugListener(observer);
            return go;
        }

        public Systems GetSystems()
        {
            return _systems;
        }

        public void OnSystemsDestroyed(Systems systems)
        {
            OnDestroy();
            Destroy(gameObject);
        }

        public void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.RemoveDebugListener(this);
                _systems = null;
            }
        }
    }

    public sealed class ArchetypeObserver : MonoBehaviour
    {
        public WorldObserver worldObserver;
        public World World;
        public Archetype Archetype;
    }

    public sealed class EntityObserver : MonoBehaviour
    {
        public World World;
        public Entity Entity;
        public bool[] unfoldedComponents = new bool[256];
    }
}
#endif