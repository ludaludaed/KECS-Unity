using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class MonoProvider<T> : BaseMonoProvider where T : struct
    {
        [SerializeField] private T _serializedData;

        public override void SetComponentToEntity(Entity entity)
        {
            if (entity.IsAlive())
            {
                entity.Set(_serializedData);
            }
            Destroy(this);
        }
    }

    [RequireComponent(typeof(EntityProvider))]
    public abstract class BaseMonoProvider : MonoBehaviour
    {
        public abstract void SetComponentToEntity(Entity entity);
    }
}