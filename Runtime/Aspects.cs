
namespace ME.ECS {

    public interface IAspect : System.IDisposable {

        public Entity entity { get; set; }

        void Initialize();

    }

    public interface IAspectFilter : IAspect {

        FilterBuilder Filter(FilterBuilder builder);

    }

    public static class AspectExt {

        public static T GetAspect<T>(this Entity entity) where T : unmanaged, IAspect {

            var aspect = new T() {
                entity = entity,
            };
            aspect.Initialize();
            return aspect;

        }

    }

}