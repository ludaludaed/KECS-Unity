namespace Ludaludaed.KECS.Unity {
    public struct OnBecameInvisibleEvent { }

    public struct OnBecameVisibleEvent { }

    public struct OnCollisionEnter2DEvent {
        public Entity first;
        public Entity second;
    }
}