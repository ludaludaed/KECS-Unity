#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludaludaed.KECS.Unity {
    public class WorldObserver : MonoBehaviour, IWorldDebugListener {
        private World _world;
        private Transform _entitiesGO;
        private Transform _archetypesGO;

        private bool _isDestroyed = false;

        public readonly HandleMap<GameObject> EntityGameObjects = new HandleMap<GameObject>(1024);

        public static GameObject Create(World world) {
            if (world == null) {
                throw new Exception("|KECS| World is null and cannot be observable");
            }

            var gameObj = new GameObject($"|KECS| [WORLD] Name: {world.Name}");
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

        public WorldInfo GetInfo() => _world.GetInfo();

        public void OnEntityCreated(in Entity entity) {
            if (_isDestroyed) return;
            GameObject go;
            if (!EntityGameObjects.Contains(entity.Id)) {
                go = new GameObject();
                go.transform.SetParent(_entitiesGO, false);
                var unityEntity = go.AddComponent<EntityObserver>();
                unityEntity.Entity = entity;
                EntityGameObjects.Set(entity.Id, go);
            } else {
                go = EntityGameObjects.Get(entity.Id);
                go.GetComponent<EntityObserver>().Entity = entity;
            }

            go.name = $"{entity.Id}\t Entity";
            go.SetActive(true);
        }


        public void OnEntityChanged(in Entity entity) {
            if (!_world.IsAlive()) return;
            var debugGO = EntityGameObjects.Get(entity.Id);
            if (!entity.Has<ViewComponent>()) {
                debugGO.name = $"{entity.Id}\t Entity";
                return;
            }

            ref var view = ref entity.Get<ViewComponent>();
            debugGO.name = $"{entity.Id}\t {view.GameObject.name}";
        }


        public void OnEntityDestroyed(in Entity entity) {
            if (_isDestroyed) return;
            if (!EntityGameObjects.Contains(entity.Id)) return;
            var go = EntityGameObjects.Get(entity.Id);
            go.name = "Destroyed Entity";
            go.SetActive(false);
        }

        public void OnArchetypeCreated(Archetype archetype) {
            if (_isDestroyed) return;
            var go = new GameObject();
            go.transform.SetParent(_archetypesGO);
            var observer = go.AddComponent<ArchetypeObserver>();

            observer.worldObserver = this;
            observer.Archetype = archetype;

            var goName = $"Archetype ";
            foreach (var typeIdx in archetype.Signature) {
                goName += $"[{EcsTypeManager.GetTypeByIndex(typeIdx).Name}] ";
            }

            go.name = goName;
        }

        public void OnWorldDestroyed(World world) {
            OnDestroy();
            Destroy(gameObject);
        }

        public void OnDestroy() {
            if (_world == null) return;
            _world.RemoveDebugListener(this);
            _world = null;
        }
    }

    public sealed class SystemsObserver : MonoBehaviour, ISystemsDebugListener {
        public FastList<Systems> Systems;

        public static SystemsObserver Create() {
            var go = new GameObject("|KECS| [SYSTEMS]");
            DontDestroyOnLoad(go);
            var observer = go.AddComponent<SystemsObserver>();
            observer.Systems = new FastList<Systems>();
            return observer;
        }


        public SystemsObserver Add(Systems systems) {
            Systems.Add(systems);
            systems.AddDebugListener(this);
            return this;
        }


        public void OnSystemsDestroyed(Systems systems) {
            OnDestroy();
            Destroy(gameObject);
        }

        public void OnDestroy() {
            if (Systems == null) return;
            foreach (var system in Systems) {
                system.RemoveDebugListener(this);
            }

            Systems.Clear();
        }
    }

    public sealed class ArchetypeObserver : MonoBehaviour {
        public WorldObserver worldObserver;
        public Archetype Archetype;
    }

    public sealed class EntityObserver : MonoBehaviour {
        public Entity Entity;
        public readonly BitSet unfoldedComponents = new BitSet(256);

        private void Awake() {
            unfoldedComponents.SetAll();
        }
    }
}
#endif