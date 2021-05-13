using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public abstract class BaseStartup : MonoBehaviour
    {
        private bool _isWorking = false;
        public World World;
        public Systems Systems;

        [SerializeField, ReadOnly] private string worldName;

        [Header("World configuration")] 
        public int cacheEntitiesCapacity;
        public int cacheArchetypesCapacity;
        public int cacheComponentsCapacity;

        public void Awake()
        {
            worldName = gameObject.scene.name;
            World = Worlds.Create(worldName,
                new WorldConfig()
                {
                    Entities = this.cacheEntitiesCapacity,
                    Archetypes = this.cacheArchetypesCapacity,
                    Components = this.cacheComponentsCapacity
                });
            

            Systems = new Systems(World);
            Systems.Add<UnityConversionSystem>();

#if UNITY_EDITOR && DEBUG
            var worldGO = WorldObserver.Create(World);
            SystemsObserver.Create(Systems, worldGO.transform);
#endif

            Bootstrap();

            Systems.Initialize();
            _isWorking = true;
        }

        public abstract void Bootstrap();

        public void Update()
        {
            if (_isWorking)
            {
                World.ExecuteTasks();
                Systems.Update(Time.deltaTime);
            }
        }

        public void FixedUpdate()
        {
            if (_isWorking)
            {
                Systems.FixedUpdate(Time.fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            if (_isWorking)
            {
                Systems.LateUpdate(Time.deltaTime);
            }
        }

        public void OnDestroy()
        {
            Systems.Destroy();

            World.Destroy();
            _isWorking = false;
        }
    }
}