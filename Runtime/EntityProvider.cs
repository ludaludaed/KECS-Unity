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
        
        private Entity _entity;
        private World _world;

        private void Start()
        {
            if(_world != null) return;
            _world = Worlds.Get(gameObject.scene.name);
            var entity = _world.CreateEntity();
            entity.SetEvent(new InstantiateEvent() {GameObject = gameObject});
        }

        private void OnEnable()
        {
            if(_world == null || !_world.IsAlive()) return;
            var entity = _world.CreateEntity();
            entity.SetEvent(new InstantiateEvent() {GameObject = gameObject});
        }

        private void OnDisable()
        {
            if(_world == null || !_world.IsAlive()) return;
            if(!_entity.IsAlive()) return;
            _entity.Destroy();
            entityID = -1;
        }

        public ref Entity GetEntity() => ref _entity;
        
        public void SetEntity(Entity entity)
        {
            _entity = entity;
            entityID = entity.Id;
        }
    }

    public struct InstantiateEvent
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