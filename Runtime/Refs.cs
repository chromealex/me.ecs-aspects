namespace ME.ECS {
    
    using ME.ECS.Collections.LowLevel.Unsafe;
    using INLINE = System.Runtime.CompilerServices.MethodImplAttribute;

    public unsafe struct RegRefRW<T> where T : unmanaged, IComponentBase {

        private MemPtr safePtr;
        private int sparseVersion;
        private int denseVersion;
        private int stateVersion;
        private int* sparsePtr;
        private Component<T>* densePtr;
        private UnmanagedComponentsStorage.Item<T>* itemPtr;

        [INLINE(256)]
        public RegRefRW(State state) {
            this.safePtr = state.structComponents.unmanagedComponentsStorage.GetRegistryPtr<T>(in state.allocator);
            var components = state.allocator.Ref<UnmanagedComponentsStorage.Item<T>>(this.safePtr).components;
            this.sparseVersion = components.GetSparse().version;
            this.denseVersion = components.GetDense().version;
            this.stateVersion = state.localVersion;
            this.sparsePtr = (int*)components.GetSparse().GetUnsafePtr(in state.allocator);
            this.densePtr = (Component<T>*)components.GetDense().GetUnsafePtr(in state.allocator);
            this.itemPtr = (UnmanagedComponentsStorage.Item<T>*)state.allocator.GetUnsafePtr(in this.safePtr);
        }

        [INLINE(256)]
        public void ValidatePointers(State state) {
            var stateChanged = false;
            if (state.localVersion != this.stateVersion) {
                this.itemPtr = (UnmanagedComponentsStorage.Item<T>*)state.allocator.GetUnsafePtr(in this.safePtr);
                this.stateVersion = state.localVersion;
                stateChanged = true;
            }
            
            if (stateChanged == true ||
                this.sparseVersion != this.itemPtr->components.GetSparse().version ||
                this.denseVersion != this.itemPtr->components.GetDense().version) {
                this.sparseVersion = this.itemPtr->components.GetSparse().version;
                this.denseVersion = this.itemPtr->components.GetDense().version;
                if (this.itemPtr->components.Length > 0) {
                    if (this.itemPtr->components.GetSparse().isCreated == true) this.sparsePtr = (int*)this.itemPtr->components.GetSparse().GetUnsafePtr(in state.allocator);
                    if (this.itemPtr->components.GetDense().isCreated == true) this.densePtr = (Component<T>*)this.itemPtr->components.GetDense().GetUnsafePtr(in state.allocator);
                }
            }
            
        }

        [INLINE(256)]
        public ref T Value(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) return ref AllComponentTypes<T>.defaultRef;
            //return ref this.itemPtr->components.Get(ref state.allocator, id).data;
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return ref AllComponentTypes<T>.defaultRef;
            return ref (this.densePtr + idx)->data;
        }

        [INLINE(256)]
        public bool Has(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) return false;
            //return this.itemPtr->components.Get(ref state.allocator, id).state > 0;
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return false;
            return (this.densePtr + idx)->state > 0;
        }

    }

    public struct RefRW<T> where T : unmanaged, IComponent {

        private MemPtr safePtr;

        public RefRW(Entity entity) {

            E.HAS_COMPONENT<T>(entity);
            
            ref var allocator = ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data;
            if (allocator.isValid == true) {
                
                this = default;
                this.Init(entity, in allocator);
                
            } else {

                this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

            }

        }

        public RefRW(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {

            E.HAS_COMPONENT<T>(entity);

            this = default;
            this.Init(entity, in allocator);

        }

        private void Init(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {
            
            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);

        }

        public readonly ref T Value(in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) => ref allocator.Ref<T>(this.safePtr);

        public readonly ref T value => ref Worlds.current.currentState.allocator.Ref<T>(this.safePtr);
        
        public static implicit operator RefRW<T>(Entity ent) {
            return new RefRW<T>(ent);
        }
        
    }

    public struct RefRO<T> where T : unmanaged, IComponent {

        private MemPtr safePtr;

        public RefRO(Entity entity) {

            E.HAS_COMPONENT<T>(entity);

            ref var allocator = ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data;
            if (allocator.isValid == true) {
                
                this = default;
                this.Init(entity, in allocator);
                
            } else {

                this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

            }

        }

        public RefRO(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {

            E.HAS_COMPONENT<T>(entity);

            this = default;
            this.Init(entity, in allocator);

        }

        private void Init(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {
            
            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);

        }

        public readonly ref T Value(in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) => ref allocator.Ref<T>(this.safePtr);

        public readonly ref readonly T value => ref Worlds.current.currentState.allocator.Ref<T>(this.safePtr);

        public static implicit operator RefRO<T>(Entity ent) {
            return new RefRO<T>(ent);
        }

    }

    public struct RefWO<T> where T : unmanaged, IComponent {

        private MemPtr safePtr;

        public RefWO(Entity entity) {

            E.HAS_COMPONENT<T>(entity);

            ref var allocator = ref ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data;
            if (allocator.isValid == true) {
                
                this = default;
                this.Init(entity, in allocator);
                
            } else {

                this.safePtr = Worlds.current.ReadDataPtr<T>(entity);

            }

        }

        public RefWO(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {

            E.HAS_COMPONENT<T>(entity);

            this = default;
            this.Init(entity, in allocator);

        }

        private void Init(Entity entity, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator) {
            
            ref var reg = ref allocator.Ref<UnmanagedComponentsStorage.Item<T>>(AllComponentTypes<T>.burstTypeStorageDirectRef.Data);
            this.safePtr = reg.components.ReadPtr(in allocator, entity.id);

        }

        public readonly void Value(in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator allocator, T value) {
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