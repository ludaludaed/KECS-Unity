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
            _world = Worlds.Get(gameObject.scene.name);
            var entity = _world.CreateEntity();
            entity.SetEvent(new InstantiateEvent() {GameObject = gameObject});
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