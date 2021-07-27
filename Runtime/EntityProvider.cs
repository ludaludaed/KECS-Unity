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
        private EntityBuilder _builder;

        private void Start()
        {
            if(_world != null) return;
            _world = Worlds.Get(gameObject.scene.name);
            var entity = _world.CreateEntity();
            entity.SetEvent(new InstantiateComponent() {GameObject = gameObject});
        }

        private void OnEnable()
        {
            if(_world == null || !_world.IsAlive()) return;
            var entity = _world.CreateEntity();
            entity.SetEvent(new InstantiateComponent() {GameObject = gameObject});
        }

        private void OnDisable()
        {
            if(_world == null || !_world.IsAlive()) return;
            if(!_entity.IsAlive()) return;
            _entity.Destroy();
            entityID = -1;
        }

        public void Build()
        {
            if(_entity.IsAlive()) _entity.Destroy();
            if (_builder == null)
            {
                _builder = new EntityBuilder();
                    
                foreach (var component in gameObject.GetComponents<BaseMonoProvider>())
                {
                    component.SetComponentToEntity(_builder);
                    Object.Destroy(component);
                }
                    
                _builder.Append(new ViewComponent()
                {
                    GameObject = gameObject, 
                    Transform = gameObject.transform, 
                    Provider = this
                });
            }
            _entity = _builder.Build(_world);
            entityID = _entity.Id;
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