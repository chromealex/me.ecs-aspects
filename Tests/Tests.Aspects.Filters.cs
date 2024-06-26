namespace ME.ECS.Tests {

    public class Tests_Aspects_Filters {

        public struct TestComponent : IComponent {

            public float value;

        }

        public struct TestComponent2 : IComponent {

            public float value;

        }

        public struct TestAspectInterface : IAspectFilter {
            
            public Entity entity { get; set; }
            
            public RefRW<TestComponent> pos;
            public RefRW<TestComponent2> rot;

            public void Initialize() {
                this.pos = new RefRW<TestComponent>(this.entity);
                this.rot = new RefRW<TestComponent2>(this.entity);
            }

            public FilterBuilder Filter(FilterBuilder builder) {
                return builder.With(this.pos).With(this.rot);
            }

            public static implicit operator TestAspectInterface(Entity ent) {
                return ent.GetAspect<TestAspectInterface>();
            }
            
            public void Dispose() {
                this.entity.SetDirty();
            }

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

        private class TestState : State {}
        
        private class TestStatesHistoryModule : ME.ECS.StatesHistory.StatesHistoryModule<TestState> {

        }

        private class TestNetworkModule : ME.ECS.Network.NetworkModule<TestState> {

            protected override ME.ECS.Network.NetworkType GetNetworkType() {
                return ME.ECS.Network.NetworkType.RunLocal | ME.ECS.Network.NetworkType.SendToNet;
            }


        }

        [NUnit.Framework.TestAttribute]
        public void CreateFilterRefWith() {

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
                    
                    Filter.Create().WithAspect(typeof(TestAspect)).Push();
                    
                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            ComponentsInitializerWorld.Setup(null);
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

        [NUnit.Framework.TestAttribute]
        public void CreateFilterRefWithout() {

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
                    
                    Filter.Create().WithAspect(typeof(TestAspect)).Push();
                    
                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            ComponentsInitializerWorld.Setup(null);
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

        [NUnit.Framework.TestAttribute]
        public void CreateFilterIAspect() {

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
                    
                    var filter = Filter.Create().WithAspect<TestAspectInterface>().Push();

                    var ent = new Entity(EntityFlag.None);
                    ent.Set(new TestComponent());
                    ent.Set(new TestComponent2());

                    using TestAspectInterface aspect = ent;
                    aspect.pos.value.value = 1f;
                    aspect.rot.value.value = 2f;
                    NUnit.Framework.Assert.AreEqual(1, filter.Count);
                }
            }
            world.SaveResetState<TestState>();
            
            world.SetFromToTicks(0, 1);
            world.Update(1f);
            
            ComponentsInitializerWorld.Setup(null);
            WorldUtilities.ReleaseWorld<TestState>(ref world);

        }

    }

}