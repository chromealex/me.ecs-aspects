namespace ME.ECS {
    
    using ME.ECS.Collections.LowLevel.Unsafe;
    using INLINE = System.Runtime.CompilerServices.MethodImplAttribute;

    public unsafe struct RegRefRO<T> where T : unmanaged, IStructComponent {

        private MemPtr safePtr;
        private int sparseVersion;
        private int denseVersion;
        private int stateVersion;
        private int* sparsePtr;
        #if !SPARSESET_DENSE_SLICED
        private Component<T>* densePtr;
        #endif
        private UnmanagedComponentsStorage.Item<T>* itemPtr;

        [INLINE(256)]
        public RegRefRO(State state) {
            this.safePtr = state.structComponents.unmanagedComponentsStorage.GetRegistryPtr<T>(in state.allocator);
            var components = state.allocator.Ref<UnmanagedComponentsStorage.Item<T>>(this.safePtr).components;
            this.sparseVersion = components.GetSparse().version;
            this.denseVersion = components.GetDense().version;
            this.stateVersion = state.localVersion;
            this.sparsePtr = (int*)components.GetSparse().GetUnsafePtr(in state.allocator);
            #if !SPARSESET_DENSE_SLICED
            this.densePtr = (Component<T>*)components.GetDense().GetUnsafePtr(in state.allocator);
            #endif
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

            ref var sparse = ref this.itemPtr->components.sparse;
            ref var dense = ref this.itemPtr->components.dense;
            if (stateChanged == true ||
                this.sparseVersion != sparse.version ||
                this.denseVersion != dense.version) {
                if (sparse.isCreated == true) {
                    this.sparsePtr = (int*)sparse.GetUnsafePtr(in state.allocator);
                    this.sparseVersion = sparse.version;
                }
                #if !SPARSESET_DENSE_SLICED
                if (dense.isCreated == true) {
                    this.densePtr = (Component<T>*)dense.GetUnsafePtr(in state.allocator);
                    this.denseVersion = dense.version;
                }
                #endif
            }
            
        }

        [INLINE(256)]
        public T Value(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) {
                return Worlds.current.ReadData<T>(state.storage.cache[in state.allocator, id]);
            }
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return default;
            #if SPARSESET_DENSE_SLICED
            var component = this.itemPtr->components.GetDense()[in Worlds.current.currentState.allocator, idx];
            if (component.state == 0) {
                return default;
            }
            return component.data;
            #else
            return (this.densePtr + idx)->data;
            #endif
        }

        [INLINE(256)]
        public bool Has(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) return false;
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return false;
            #if SPARSESET_DENSE_SLICED
            return this.itemPtr->components.GetDense()[in Worlds.current.currentState.allocator, idx].state > 0;
            #else
            return (this.densePtr + idx)->state > 0;
            #endif
        }

    }

    public unsafe struct RegRefRW<T> where T : unmanaged, IStructComponent {

        private MemPtr safePtr;
        private int sparseVersion;
        private int denseVersion;
        private int stateVersion;
        private int* sparsePtr;
        #if !SPARSESET_DENSE_SLICED
        private Component<T>* densePtr;
        #endif
        private UnmanagedComponentsStorage.Item<T>* itemPtr;

        [INLINE(256)]
        public RegRefRW(State state) {
            this.safePtr = state.structComponents.unmanagedComponentsStorage.GetRegistryPtr<T>(in state.allocator);
            var components = state.allocator.Ref<UnmanagedComponentsStorage.Item<T>>(this.safePtr).components;
            this.sparseVersion = components.GetSparse().version;
            this.denseVersion = components.GetDense().version;
            this.stateVersion = state.localVersion;
            this.sparsePtr = (int*)components.GetSparse().GetUnsafePtr(in state.allocator);
            #if !SPARSESET_DENSE_SLICED
            this.densePtr = (Component<T>*)components.GetDense().GetUnsafePtr(in state.allocator);
            #endif
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

            ref var sparse = ref this.itemPtr->components.sparse;
            ref var dense = ref this.itemPtr->components.dense;
            if (stateChanged == true ||
                this.sparseVersion != sparse.version ||
                this.denseVersion != dense.version) {
                if (sparse.isCreated == true) {
                    this.sparsePtr = (int*)sparse.GetUnsafePtr(in state.allocator);
                    this.sparseVersion = sparse.version;
                }
                #if !SPARSESET_DENSE_SLICED
                if (dense.isCreated == true) {
                    this.densePtr = (Component<T>*)dense.GetUnsafePtr(in state.allocator);
                    this.denseVersion = dense.version;
                }
                #endif
            }
            
        }

        [INLINE(256)]
        public ref T Value(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) {
                return ref Worlds.current.GetData<T>(state.storage.cache[in state.allocator, id]);
            }
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return ref Worlds.current.GetData<T>(state.storage.cache[in state.allocator, id]);
            #if SPARSESET_DENSE_SLICED
            ref var component = ref this.itemPtr->components.GetDense()[in Worlds.current.currentState.allocator, idx];
            if (component.state == 0) {
                return ref Worlds.current.GetData<T>(state.storage.cache[in state.allocator, id]);
            }
            return ref component.data;
            #else
            return ref (this.densePtr + idx)->data;
            #endif
        }

        [INLINE(256)]
        public bool Has(int id, State state) {
            this.ValidatePointers(state);
            if (id >= this.itemPtr->components.Length) return false;
            var idx = *(this.sparsePtr + id);
            if (idx == 0) return false;
            #if SPARSESET_DENSE_SLICED
            return this.itemPtr->components.GetDense()[in Worlds.current.currentState.allocator, idx].state > 0;
            #else
            return (this.densePtr + idx)->state > 0;
            #endif
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