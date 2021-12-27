using UnityEngine;

namespace Ludaludaed.KECS.Unity {
    public class OnBecameInvisibleNotifier : UnityNotifier {
        private void OnBecameInvisible() {
            ref var entity = ref provider.GetEntity();
            if (!entity.IsAlive()) return;
            entity.SetEvent(new OnBecameInvisibleEvent());
        }
    }
}