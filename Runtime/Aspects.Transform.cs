#if FIXED_POINT_MATH
using ME.ECS.Mathematics;
using tfloat = sfloat;
#else
using Unity.Mathematics;
using tfloat = System.Single;
#endif

namespace ME.ECS {

    public readonly ref struct TransformAspect {

        private readonly Entity ent;
        
        private readonly RefRW<ME.ECS.Transform.Position> localPositionComponent;
        private readonly RefRW<ME.ECS.Transform.Rotation> localRotationComponent;
        private readonly RefRW<ME.ECS.Transform.Scale> localScaleComponent;

        public float3 position {
            get => this.ent.GetPosition();
            set => this.ent.SetPosition(value);
        }
        
        public quaternion rotation {
            get => this.ent.GetRotation();
            set => this.ent.SetRotation(value);
        }

        public float3 scale {
            get => this.ent.GetScale();
        }

        public ref float3 localPosition => ref this.localPositionComponent.value.value;
        public ref quaternion localRotation => ref this.localRotationComponent.value.value;
        public ref float3 localScale => ref this.localScaleComponent.value.value;

        private TransformAspect(Entity ent) {
            this.localPositionComponent = new RefRW<ME.ECS.Transform.Position>(ent);
            this.localRotationComponent = new RefRW<ME.ECS.Transform.Rotation>(ent);
            this.localScaleComponent = new RefRW<ME.ECS.Transform.Scale>(ent);
            this.ent = ent;
        }
        
        public static implicit operator TransformAspect(Entity ent) {
            return new TransformAspect(ent);
        }

        public void Dispose() => this.ent.SetDirty();

    }

    public readonly ref struct Transform2DAspect {

        private readonly Entity ent;
        
        private readonly RefRW<ME.ECS.Transform.Position2D> localPositionComponent;
        private readonly RefRW<ME.ECS.Transform.Rotation2D> localRotationComponent;
        private readonly RefRW<ME.ECS.Transform.Scale2D> localScaleComponent;

        public float2 position {
            get => this.ent.GetPosition2D();
            set => this.ent.SetPosition2D(value);
        }
        
        public tfloat rotation {
            get => this.ent.GetRotation2D();
            set => this.ent.SetRotation2D(value);
        }

        public float2 scale {
            get => this.ent.GetScale2D();
        }

        public ref float2 localPosition => ref this.localPositionComponent.value.value;
        public ref tfloat localRotation => ref this.localRotationComponent.value.value;
        public ref float2 localScale => ref this.localScaleComponent.value.value;

        private Transform2DAspect(Entity ent) {
            this.localPositionComponent = new RefRW<ME.ECS.Transform.Position2D>(ent);
            this.localRotationComponent = new RefRW<ME.ECS.Transform.Rotation2D>(ent);
            this.localScaleComponent = new RefRW<ME.ECS.Transform.Scale2D>(ent);
            this.ent = ent;
        }
        
        public static implicit operator Transform2DAspect(Entity ent) {
            return new Transform2DAspect(ent);
        }

        public void Dispose() => this.ent.SetDirty();

    }

}