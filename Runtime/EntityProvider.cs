using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class EntityProvider : MonoBehaviour
    {
        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        private int entityID = -1;
        public bool InjectAndDestroy = false;
        
        private Entity _entity;
        private World _world;

        private void Start()
        {
            if(_world != null) return;
            _world = Worlds.Get(gameObject.scene.name);
            _entity = _world.CreateEntity();
            _entity.Set(new InstantiateComponent() {GameObject = gameObject});
            entityID = _entity.Id;
        }

        private void OnEnable()
        {
            if(_world == null || !_world.IsAlive()) return;
            _entity = _world.CreateEntity();
            _entity.Set(new InstantiateComponent() {GameObject = gameObject});
            entityID = _entity.Id;
        }

        private void OnDisable()
        {
            if(_world == null || !_world.IsAlive()) return;
            if(!_entity.IsAlive()) return;
            _entity.Destroy();
            entityID = -1;
        }

        public ref Entity GetEntity() => ref _entity;
    }

    public struct InstantiateComponent
    {
        public GameObject GameObject;
    }

    public struct ViewComponent
    {
        public GameObject GameObject;
        public Transform Transform;
        public EntityProvider Provider;
    }
}