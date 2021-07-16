#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class WorldObserver : MonoBehaviour, IWorldDebugListener
    {
        private World _world;
        private Transform _entitiesGO;
        private Transform _archetypesGO;

        private bool _isDestroyed = false;

        public readonly HandleMap<GameObject> EntityGameObjects = new HandleMap<GameObject>(1024);

        public static GameObject Create(World world)
        {
            if (world == null)
            {
                throw new Exception("World is null and cannot be observable");
            }

            var gameObj = new GameObject($"[WORLD] Name: {world.Name} Id: {world.Id}");
            DontDestroyOnLoad(gameObj);

            var observer = gameObj.AddComponent<WorldObserver>();
            observer._world = world;
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
            return _world.GetInfo();
        }

        public void OnEntityCreated(in Entity entity)
        {
            if (_isDestroyed) return;
            if (!EntityGameObjects.Contains(entity.Id))
            {
                var go = new GameObject();
                go.transform.SetParent(_entitiesGO, false);
                var unityEntity = go.AddComponent<EntityObserver>();
                unityEntity.Entity = entity;
                go.name = entity.IsAlive() ? $"Entity {entity.Id} {entity.Age}" : "Destroyed Entity";
                go.SetActive(true);
                EntityGameObjects.Set(entity.Id, go);
            }
            else
            {
                ref var go = ref EntityGameObjects.Get(entity.Id);
                go.GetComponent<EntityObserver>().Entity = entity;
                go.name = entity.IsAlive() ? $"Entity {entity.Id} {entity.Age}" : "Destroyed Entity";
                go.SetActive(true);
            }
        }

        public void OnEntityDestroyed(in Entity entity)
        {
            if (_isDestroyed) return;
            if (!EntityGameObjects.Contains(entity.Id)) return;
            var go = EntityGameObjects.Get(entity.Id);
            go.name = entity.IsAlive() ? $"Entity {entity.Id} {entity.Age}" : "Destroyed Entity";
            go.SetActive(false);
        }

        public void OnArchetypeCreated(Archetype archetype)
        {
            if (_isDestroyed) return;
            var go = new GameObject();
            go.transform.SetParent(_archetypesGO);
            var observer = go.AddComponent<ArchetypeObserver>();

            observer.worldObserver = this;
            observer.Archetype = archetype;

            var goName = "Archetype ";
            foreach (var typeIdx in archetype.Mask)
            {
                goName += $"[{EcsTypeManager.GetTypeByIndex(typeIdx).Name}] ";
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
            if (_world == null) return;
            _world.RemoveDebugListener(this);
            _world = null;
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
            if (_systems == null) return;
            _systems.RemoveDebugListener(this);
            _systems = null;
        }
    }

    public sealed class ArchetypeObserver : MonoBehaviour
    {
        public WorldObserver worldObserver;
        public Archetype Archetype;
    }

    public sealed class EntityObserver : MonoBehaviour
    {
        public Entity Entity;
        public bool[] unfoldedComponents = new bool[256];
    }
}
#endif