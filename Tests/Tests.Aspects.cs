namespace ME.ECS.Tests {

    public class Tests_Aspects {

        public struct TestComponent : IComponent {

            public float value;

        }

        public struct TestComponent2 : IComponent {

            public float value;

        }

        public struct TestAspect3 {

            public Entity entity;

            private State state;
            private RegRefRW<TestComponent> test1Reg;
            private RegRefRW<TestComponent2> test2Reg;

            public ref TestComponent test1 => ref this.test1Reg.Value(this.entity.id, this.state);
            public ref TestComponent2 test2 => ref this.test2Reg.Value(this.entity.id, this.state);

            public void Initialize(State state) {

                this.state = state;
                this.test1Reg = new RegRefRW<TestComponent>(state);
                this.test2Reg = new RegRefRW<TestComponent2>(state);

            }

        }

        public readonly ref struct TestAspect2 {

            private readonly Entity ent;

            public float pos {
                get => this.ent.Read<TestComponent>().value;
                set => this.ent.Set(new TestComponent() { value = value });
            }
        
            public float rot {
                get => this.ent.Read<TestComponent2>().value;
                set => this.ent.Set(new TestComponent2() { value = value });
            }

            private TestAspect2(Entity ent) {
                this.ent = ent;
            }
        
            public static implicit operator TestAspect2(Entity ent) {
                return new TestAspect2(ent);
            }
            
            public void Dispose() {}

        }

        public readonly ref struct TestAspect {

            private readonly Entity ent;

            public readonly RefRW<TestComponent> pos;
            public readonly RefRW<TestComponent2> rot;
            
            private TestAspect(Entity ent) {
                this.pos = new RefRW<TestComponent>(ent);
                this.rot = new RefRW<TestComponent2>(ent);
                this.ent = ent;
            }
        
            public static implicit operator TestAspect(Entity ent) {
                return new TestAspect(ent);
            }

            public void Dispose() {
                this.ent.SetDirty();
            }

        }

        public readonly ref struct TestAspectBurst {

            private readonly Entity ent;

            public readonly RefRW<TestComponent> pos;
            public readonly RefRW<TestComponent2> rot;
            
            private TestAspectBurst(Entity ent) {
                this.pos = new RefRW<TestComponent>(ent, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data);
                this.rot = new RefRW<TestComponent2>(ent, in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data);
                this.ent = ent;
            }
        
            public static implicit operator TestAspectBurst(Entity ent) {
                return new TestAspectBurst(ent);
            }

            public void Dispose() {
                this.ent.SetDirty(in ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocatorContext.burstAllocator.Data);
            }

        }

        private class TestState : State {}
        
        private class TestStatesHistoryModule : ME.ECS.StatesHistory.StatesHistoryModule<TestState> {

        }

        private class TestNetworkModule : ME.ECS.Network.NetworkModule<TestState> {

            protected override ME.ECS.Network.NetworkType GetNetworkType() {
                return ME.ECS.Network.NetworkType.RunLocal | ME.ECS.Network.NetworkType.SendToNet;
            }


        }

        [NUnit.Framework.TestAttribute]
        public void RegAspect() {

            World world = null;
            WorldUtilities.CreateWorld<TestState>(ref world, 0.033f);
            {
                world.AddModule<TestStatesHistoryModule>();
                world.AddModule<TestNetworkModule>();
                world.SetState<TestState>(WorldUtilities.CreateState<TestState>());
                world.SetSeed(1u);
                {
                    WorldUtilities.InitComponentTypeId<TestComponent>(false);
                    WorldUtilities.InitComponentTypeId<TestComponent2>(false);
                    ComponentsInitializerWorld.Setup((e) => {
                
                        e.ValidateDataUnmanaged<TestComponent>();
                        e.ValidateDataUnmanaged<TestComponent2>();
                
                    });
                }
                {
                    world.SetEntitiesCapacity(1000);
                    
                    var list = new System.Collections.Generic.List<Entity>();
                    for (int i = 0; i < 1000; ++i) {
                        var ent = Entity.Create();
                        ent.Set(new TestComponent());
                        ent.Set(new TestComponent2());
                        list.Add(ent);
                    }

                    var aspect = new TestAspect3();
                    aspect.Initialize(world.currentState);
                    
                    {
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            var testAspect = aspect;
                            testAspect.entity = ent;
                            testAspect.test1.value = 1f;
                            testAspect.test2.value = 2f;
                        }
                        
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            var testAspect = aspect;
                            testAspect.entity = ent;
                            NUnit.Framework.Assert.AreEqual(1f, testAspect.test1.value);
                            NUnit.Framework.Assert.AreEqual(2f, testAspect.test2.value);
                        }
                    }
                    
                    for (int i = 0; i < list.Count; ++i) {
                        var ent = list[i];
                        ent.Set(new TestComponent());
                        ent.Set(new TestComponent2());
                    }

                    {
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            var testAspect = aspect;
                            testAspect.entity = ent;
                            testAspect.test1.value = 1f;
                            testAspect.test2.value = 2f;
                        }
                        
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            var testAspect = aspect;
                            testAspect.entity = ent;
                            NUnit.Framework.Assert.AreEqual(1f, testAspect.test1.value);
                            NUnit.Framework.Assert.AreEqual(2f, testAspect.test2.value);
                        }
                    }
                    
                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }
        
        [NUnit.Framework.TestAttribute]
        public void FillAspects() {

            World world = null;
            WorldUtilities.CreateWorld<TestState>(ref world, 0.033f);
            {
                world.AddModule<TestStatesHistoryModule>();
                world.AddModule<TestNetworkModule>();
                world.SetState<TestState>(WorldUtilities.CreateState<TestState>());
                world.SetSeed(1u);
                {
                    WorldUtilities.InitComponentTypeId<TestComponent>(false);
                    WorldUtilities.InitComponentTypeId<TestComponent2>(false);
                    ComponentsInitializerWorld.Setup((e) => {
                
                        e.ValidateDataUnmanaged<TestComponent>();
                        e.ValidateDataUnmanaged<TestComponent2>();
                
                    });
                }
                {
                    world.SetEntitiesCapacity(1000);
                    
                    var list = new System.Collections.Generic.List<Entity>();
                    for (int i = 0; i < 1000; ++i) {
                        var ent = Entity.Create();
                        ent.Set(new TestComponent());
                        ent.Set(new TestComponent2());
                        list.Add(ent);
                    }

                    {
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspect)ent;
                            testAspect.pos.value.value = 1f;
                            testAspect.rot.value.value = 2f;
                        }
                        
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspect)ent;
                            NUnit.Framework.Assert.AreEqual(1f, testAspect.pos.value.value);
                            NUnit.Framework.Assert.AreEqual(2f, testAspect.rot.value.value);
                        }
                    }
                    
                    for (int i = 0; i < list.Count; ++i) {
                        var ent = list[i];
                        ent.Set(new TestComponent());
                        ent.Set(new TestComponent2());
                    }

                    {
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspect2)ent;
                            testAspect.pos = 1f;
                            testAspect.rot = 2f;
                        }
                        
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspect2)ent;
                            NUnit.Framework.Assert.AreEqual(1f, testAspect.pos);
                            NUnit.Framework.Assert.AreEqual(2f, testAspect.rot);
                        }
                    }
                    
                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

        [NUnit.Framework.TestAttribute]
        public void FillAspectsBurst() {

            World world = null;
            WorldUtilities.CreateWorld<TestState>(ref world, 0.033f);
            {
                world.AddModule<TestStatesHistoryModule>();
                world.AddModule<TestNetworkModule>();
                world.SetState<TestState>(WorldUtilities.CreateState<TestState>());
                world.SetSeed(1u);
                {
                    WorldUtilities.InitComponentTypeId<TestComponent>(false);
                    WorldUtilities.InitComponentTypeId<TestComponent2>(false);
                    ComponentsInitializerWorld.Setup((e) => {
                
                        e.ValidateDataUnmanaged<TestComponent>();
                        e.ValidateDataUnmanaged<TestComponent2>();
                
                    });
                }
                {
                    world.SetEntitiesCapacity(1000);
                    
                    var list = new System.Collections.Generic.List<Entity>();
                    for (int i = 0; i < 1000; ++i) {
                        var ent = Entity.Create();
                        ent.Set(new TestComponent());
                        ent.Set(new TestComponent2());
                        list.Add(ent);
                    }

                    ref var allocator = ref world.currentState.allocator;
                    using var context = ME.ECS.Collections.LowLevel.Unsafe.MemoryAllocator.CreateContext();
                    {
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspectBurst)ent;
                            testAspect.pos.Value(in allocator).value = 1f;
                            testAspect.rot.Value(in allocator).value = 2f;
                        }
                        
                        for (int i = 0; i < list.Count; ++i) {
                            var ent = list[i];
                            using var testAspect = (TestAspectBurst)ent;
                            NUnit.Framework.Assert.AreEqual(1f, testAspect.pos.Value(in allocator).value);
                            NUnit.Framework.Assert.AreEqual(2f, testAspect.rot.Value(in allocator).value);
                        }
                    }

                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

    }

}