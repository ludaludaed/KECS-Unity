using UnityEngine;

namespace Ludaludaed.KECS.Unity {
    public struct OnCollisionEnter2D {
        public Entity first;
        public Entity second;
    }

    public class OnCollisionEnter2DNotifier : UnityNotifier {
        private void OnCollisionEnter2D(Collision2D other) {
            ref var entity = ref provider.GetEntity();
            if (!entity.IsAlive()) return;

            var otherTransform = other.transform;
            if (!otherTransform.TryGetComponent<EntityProvider>(out var otherProvider)) return;

            ref var otherEntity = ref otherProvider.GetEntity();
            if (!otherEntity.IsAlive()) return;

            // var newEvent = entity.World.CreateEntity();
            // newEvent.SetEvent(new OnCollisionEnter2D() {first = entity, second = otherEntity});
            entity.SetEvent(new OnCollisionEnter2DEvent() {other = otherEntity});
        }
    }
}