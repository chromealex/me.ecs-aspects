namespace ME.ECS {

    public ref struct RefRW<T> where T : unmanaged, IComponent {

        private long safePtr;

        public RefRW(Entity entity) {

            this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

        }

        public RefRW(Entity entity, in ME.ECS.Collections.V3.MemoryAllocator allocator) {

            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);
            
        }

        public readonly ref T Value(in ME.ECS.Collections.V3.MemoryAllocator allocator) => ref allocator.Ref<T>(this.safePtr);

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

        public RefRO(Entity entity, in ME.ECS.Collections.V3.MemoryAllocator allocator) {

            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);
            
        }

        public readonly ref T Value(in ME.ECS.Collections.V3.MemoryAllocator allocator) => ref allocator.Ref<T>(this.safePtr);

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

        public RefWO(Entity entity, in ME.ECS.Collections.V3.MemoryAllocator allocator) {

            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);
            
        }

        public readonly void Value(in ME.ECS.Collections.V3.MemoryAllocator allocator, T value) {
            allocator.Ref<T>(this.safePtr) = value;
        }

        public readonly T value {
            set => Worlds.current.currentState.allocator.Ref<T>(this.safePtr) = value;
        }

        public static implicit operator RefWO<T>(Entity ent) {
            return new RefWO<T>(ent);
        }

    }

}