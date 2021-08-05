using System;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public abstract class BaseStartup : MonoBehaviour
    {
        public World World;
        public SystemGroup UpdateSystems;
        public SystemGroup FixedUpdateSystems;
        public SystemGroup LateUpdateSystems;

        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        private string worldName;

        [Header("World configuration")] public int EntitiesCapacity;
        public int ArchetypesCapacity;
        public int ComponentsCapacity;

        public void Awake()
        {
            worldName = gameObject.scene.name;
            World = Worlds.Create(worldName, new WorldConfig()
            {
                Entities = EntitiesCapacity,
                Archetypes = ArchetypesCapacity,
                Components = ComponentsCapacity,
            });


            UpdateSystems = new SystemGroup(World, "Update");
            FixedUpdateSystems = new SystemGroup(World, "Fixed Update");
            LateUpdateSystems = new SystemGroup(World, "Late Update");

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
            UpdateSystems.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            FixedUpdateSystems.Update(Time.deltaTime);
        }

        public void LateUpdate()
        {
            LateUpdateSystems.Update(Time.deltaTime);
        }

        public void OnDestroy()
        {
            UpdateSystems.Destroy();
            FixedUpdateSystems.Destroy();
            LateUpdateSystems.Destroy();
            World.Destroy();
        }
    }
}