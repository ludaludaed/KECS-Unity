using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class UnityConversionSystem : SystemBase, IUpdate
    {
        private Filter _filter;

        public override void Initialize()
        {
            _filter = _world.Filter().With<InstantiateComponent>().Without<ViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            _filter.ForEach((Entity entity, ref InstantiateComponent instantiate) =>
            {
                if (instantiate.GameObject != null)
                {
                    var gameObject = instantiate.GameObject;
                    var provider = gameObject.GetComponent<EntityProvider>();
                    if (provider == null) return;
                    
                    entity.Set(new ViewComponent()
                    {
                        GameObject = gameObject, 
                        Transform = gameObject.transform, 
                        Provider = provider
                    });

                    entity.Remove<InstantiateComponent>();
                    foreach (var component in gameObject.GetComponents<BaseMonoProvider>())
                    {
                        component.SetComponentToEntity(entity);
                        if(provider.InjectAndDestroy) Object.Destroy(component);
                    }
                }
            });
        }
    }
}