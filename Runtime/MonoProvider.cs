using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class MonoProvider<T> : BaseMonoProvider where T : struct
    {
        [SerializeField] protected T serializedData;

        public override void SetComponentToEntity(EntityBuilder entityBuilder)
        {
            entityBuilder.Append(serializedData);
        }
    }

    [RequireComponent(typeof(EntityProvider))]
    public abstract class BaseMonoProvider : MonoBehaviour
    {
        public abstract void SetComponentToEntity(EntityBuilder entityBuilder);
    }
}