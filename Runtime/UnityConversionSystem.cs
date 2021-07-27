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
                    provider.Build();
                }
            });
        }
    }
}