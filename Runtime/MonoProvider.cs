using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class MonoProvider<T> : BaseMonoProvider where T : struct
    {
        [SerializeField] protected T serializedData;

        public override void SetComponentToEntity(in Entity entity)
        {
            if (entity.IsAlive())
            {
                entity.Set(serializedData);
            }
        }
    }

    [RequireComponent(typeof(EntityProvider))]
    public abstract class BaseMonoProvider : MonoBehaviour
    {
        public abstract void SetComponentToEntity(in Entity entity);
    }
}