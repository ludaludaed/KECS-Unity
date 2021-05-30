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
        public Entity GetEntity() => _entity;

        private void Start()
        {
            var world = Worlds.Get(gameObject.scene.name);
            var entity = world.CreateEntity();
            entity.SetEvent(new InstantiateEventComponent() {GO = gameObject});
        }

        public void SetEntity(Entity entity)
        {
            _entity = entity;
            entityID = entity.Id;
        }
    }

    public struct InstantiateEventComponent
    {
        public GameObject GO;
    }

    public struct GameObjectComponent
    {
        public GameObject GameObject;
        public EntityProvider Entity;
    }
}