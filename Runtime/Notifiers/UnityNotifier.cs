using System;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Ludaludaed.KECS.Unity
{
    [RequireComponent(typeof(EntityProvider))]
    public abstract class UnityNotifier: MonoBehaviour
    {
        private EntityProvider _provider;
        protected EntityProvider provider
        {
            get
            {
                if (_provider != null) return _provider;
                _provider = GetComponent<EntityProvider>();
                return _provider;
            }
        }
    }
}