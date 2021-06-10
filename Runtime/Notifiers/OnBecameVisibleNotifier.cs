using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    public class OnBecameVisibleNotifier : UnityNotifier
    {
        private void OnBecameVisible()
        {
            ref var entity = ref provider.GetEntity();
            if (!entity.IsAlive()) return;
            entity.SetEvent(new OnBecameVisibleEvent());
        }
    }
}