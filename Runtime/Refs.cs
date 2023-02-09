namespace ME.ECS {

    public ref struct RefRW<T> where T : unmanaged, IComponent {

        private long safePtr;

        public RefRW(Entity entity) {

            this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

        }

        public readonly ref T value => ref Worlds.current.currentState.allocator.Ref<T>(this.safePtr);
        
        public static implicit operator RefRW<T>(Entity ent) {
            return new RefRW<T>(ent);
        }
        
    }

    public ref struct RefRO<T> where T : unmanaged, IComponent {

        private long safePtr;

        public RefRO(Entity entity) {

            this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

        }

        public readonly ref readonly T value => ref Worlds.current.currentState.allocator.Ref<T>(this.safePtr);

        public static implicit operator RefRO<T>(Entity ent) {
            return new RefRO<T>(ent);
        }

    }

    public ref struct RefWO<T> where T : unmanaged, IComponent {

        private long safePtr;

        public RefWO(Entity entity) {

            this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

        }

        public readonly T value {
            set => Worlds.current.currentState.allocator.Ref<T>(this.safePtr) = value;
        }

        public static implicit operator RefWO<T>(Entity ent) {
            return new RefWO<T>(ent);
        }

    }

}