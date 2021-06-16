using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class UnityConversionSystem : SystemBase, IUpdate
    {
        private Filter _filter;

        public override void Initialize()
        {
            _filter = _world.Filter().With<InstantiateEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            _filter.ForEach((Entity entity, ref InstantiateEvent instantiate) =>
            {
                if (instantiate.GameObject != null)
                {
                    var gameObject = instantiate.GameObject;
                    var provider = gameObject.GetComponent<EntityProvider>();
                    var newEntity = _world.CreateEntity();

                    provider.SetEntity(newEntity);
                    newEntity.Set(new ViewComponent()
                        {GameObject = gameObject, Transform = gameObject.transform, Provider = provider});
                    foreach (var component in gameObject.GetComponents<BaseMonoProvider>())
                    {
                        component.SetComponentToEntity(newEntity);
                    }
                }

                entity.Destroy();
            });
        }
    }
}