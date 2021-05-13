using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class UnityConversionSystem:SystemBase, IUpdate
    {
        private Filter _filter;
        public override void Initialize()
        {
            _filter = _world.Filter().With<InstantiateEventComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            _filter.ForEach((Entity entity, ref InstantiateEventComponent instantiate) =>
            {
                if (instantiate.GO != null)
                {
                    var gameObject = instantiate.GO;
                    var newEntity = _world.CreateEntity();
                    newEntity.Set(new GameObjectComponent() {GameObject = gameObject});
                    foreach (var component in gameObject.GetComponents<BaseMonoProvider>())
                    {
                        component.SetComponentToEntity(newEntity);
                        Object.Destroy(component);
                    }
                }
                entity.Destroy();
            });
        }
    }
}