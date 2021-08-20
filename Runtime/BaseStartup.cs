using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public abstract class BaseStartup : MonoBehaviour
    {
        public Systems UpdateSystems;
        public Systems FixedUpdateSystems;
        public Systems LateUpdateSystems;
        public World World;

        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        private string worldName;

        [Header("World configuration")] public int EntitiesCapacity = WorldConfig.DefaultEntities;
        public int ArchetypesCapacity = WorldConfig.DefaultArchetypes;
        public int ComponentsCapacity = WorldConfig.DefaultComponents;
        public int QueriesCapacity = WorldConfig.DefaultQueries;

        public void Awake()
        {
            worldName = gameObject.scene.name;
            World = Worlds.Create(worldName, new WorldConfig()
            {
                Entities = EntitiesCapacity,
                Archetypes = ArchetypesCapacity,
                Components = ComponentsCapacity,
                Queries = QueriesCapacity
            });


            UpdateSystems = new Systems(World, "Update");
            FixedUpdateSystems = new Systems(World, "Fixed Update");
            LateUpdateSystems = new Systems(World, "Late Update");

#if UNITY_EDITOR && DEBUG
            WorldObserver.Create(World);
            SystemsObserver.Create()
                .Add(UpdateSystems)
                .Add(FixedUpdateSystems)
                .Add(LateUpdateSystems);
#endif

            Bootstrap();

            UpdateSystems.Initialize();
            FixedUpdateSystems.Initialize();
            LateUpdateSystems.Initialize();
        }

        public abstract void Bootstrap();

        public void Update()
        {
            World.ExecuteTasks();
            UpdateSystems.OnUpdate(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            FixedUpdateSystems.OnUpdate(Time.deltaTime);
        }

        public void LateUpdate()
        {
            LateUpdateSystems.OnUpdate(Time.deltaTime);
        }

        public void OnDestroy()
        {
            UpdateSystems.OnDestroy();
            FixedUpdateSystems.OnDestroy();
            LateUpdateSystems.OnDestroy();
            World.Destroy();
        }
    }
}