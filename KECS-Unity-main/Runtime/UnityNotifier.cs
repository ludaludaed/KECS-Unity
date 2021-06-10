using System;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    [RequireComponent(typeof(EntityProvider))]
    public class UnityNotifier: MonoBehaviour
    {
        private EntityProvider _provider;
        private EntityProvider provider
        {
            get
            {
                if (_provider != null) return _provider;
                _provider = GetComponent<EntityProvider>();
                return _provider;
            }
        }

        private void OnBecameInvisible()
        {
            ref var entity = ref provider.GetEntity();
            if(!entity.IsAlive()) return;
            entity.SetEvent(new OnBecameInvisibleEvent());
        }
        
        private void OnBecameVisible()
        {
            ref var entity = ref provider.GetEntity();
            if(!entity.IsAlive()) return;
            entity.SetEvent(new OnBecameVisibleEvent());
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            ref var entity = ref provider.GetEntity();
            if(!entity.IsAlive()) return;
            
            var otherTransform = other.transform;
            if (!otherTransform.TryGetComponent<EntityProvider>(out var otherProvider)) return;

            ref var otherEntity = ref otherProvider.GetEntity();
            if (!otherEntity.IsAlive()) return;
            entity.SetEvent(new OnCollisionEnter2DEvent() {other = otherEntity});
        }
    }
    
    public struct OnBecameInvisibleEvent
    {
    }
    
    public struct OnBecameVisibleEvent
    {
    }
    
    public struct OnCollisionEnter2DEvent
    {
        public Entity other;
    }
}