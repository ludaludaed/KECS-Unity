using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class EntityProvider : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int entityID = -1;

        private void Start()
        {
            var world = Worlds.Get(gameObject.scene.name);
            var entity = world.CreateEntity();
            entity.Event(new InstantiateEventComponent() {GO = gameObject});
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
    }
}