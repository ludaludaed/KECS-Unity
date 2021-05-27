using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public abstract class BaseStartup : MonoBehaviour
    {
        public World World;
        public Systems Systems;

        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        private string worldName;

        [Header("World configuration")] public int cacheEntitiesCapacity;
        public int cacheArchetypesCapacity;
        public int cacheComponentsCapacity;
        public int filtersComponentsCapacity;

        public void Awake()
        {
            worldName = gameObject.scene.name;
            World = Worlds.Create(worldName, new WorldConfig()
                {
                    Entities = cacheEntitiesCapacity,
                    Archetypes = cacheArchetypesCapacity,
                    Components = cacheComponentsCapacity,
                    Filters = filtersComponentsCapacity
                });


            Systems = new Systems(World);
            Systems.Add<UnityConversionSystem>();

#if UNITY_EDITOR && DEBUG
            var worldGO = WorldObserver.Create(World);
            SystemsObserver.Create(Systems, worldGO.transform);
#endif

            Bootstrap();

            Systems.Initialize();
        }

        public abstract void Bootstrap();

        public void Update()
        {
            World.ExecuteTasks();
            Systems.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            Systems.FixedUpdate(Time.fixedDeltaTime);
        }

        public void LateUpdate()
        {
            Systems.LateUpdate(Time.deltaTime);
        }

        public void OnDestroy()
        {
            Systems.Destroy();
            World.Destroy();
        }
    }
}