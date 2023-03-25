using System.Collections.Generic;
using System.Linq;

namespace ME.ECS {

    public static class FilterBuilderExt {

        public static FilterBuilder With<T>(this FilterBuilder builder, RefRO<T> refObj) where T : unmanaged, IComponent => builder.With<T>();
        public static FilterBuilder Without<T>(this FilterBuilder builder, RefRO<T> refObj) where T : unmanaged, IComponent => builder.Without<T>();
        public static FilterBuilder With<T>(this FilterBuilder builder, RefRW<T> refObj) where T : unmanaged, IComponent => builder.With<T>();
        public static FilterBuilder Without<T>(this FilterBuilder builder, RefRW<T> refObj) where T : unmanaged, IComponent => builder.Without<T>();
        public static FilterBuilder With<T>(this FilterBuilder builder, RefWO<T> refObj) where T : unmanaged, IComponent => builder.With<T>();
        public static FilterBuilder Without<T>(this FilterBuilder builder, RefWO<T> refObj) where T : unmanaged, IComponent => builder.Without<T>();

        public static FilterBuilder WithAspect<T>(this FilterBuilder builder) where T : unmanaged, IAspectFilter {

            builder = default(T).Filter(builder);
            return builder;

        }

        public static FilterBuilder WithAspect(this FilterBuilder builder, System.Type aspectType) {
            
            var method = builder.GetType().GetMethod(nameof(FilterBuilder.With), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            FilterBuilderExt.RunMethod(ref builder, method, aspectType);

            return builder;

        }

        public static FilterBuilder WithoutAspect(this FilterBuilder builder, System.Type aspectType) {
            
            var method = builder.GetType().GetMethod(nameof(FilterBuilder.Without), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            FilterBuilderExt.RunMethod(ref builder, method, aspectType);

            return builder;

        }

        private static void RunMethod(ref FilterBuilder builder, System.Reflection.MethodInfo method, System.Type aspectType) {
            
            var fields = TypeHelper.GetFields(aspectType);
            foreach (var field in fields) {
                
                var compType = field.FieldType.GenericTypeArguments[0];
                var gMethod = method.MakeGenericMethod(compType);
                builder = (FilterBuilder)gMethod.Invoke(builder, null);
                
            }
            
        }

    }

    internal static class TypeHelper {

        private static Dictionary<System.Type, System.Reflection.FieldInfo[]> fields = new Dictionary<System.Type, System.Reflection.FieldInfo[]>();

        public static System.Reflection.FieldInfo[] GetFields(System.Type aspectType) {

            if (TypeHelper.fields.TryGetValue(aspectType, out var arr) == true) {
                return arr;
            }
            
            var fields = aspectType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            arr = fields.Where(x => {
                if (x.FieldType.IsGenericType == true && x.FieldType.GenericTypeArguments?.Length == 1) {
                    if (x.FieldType.Name == "RefRO`1" ||
                        x.FieldType.Name == "RefRW`1" ||
                        x.FieldType.Name == "RefWO`1") {
                        return true;
                    }

                    return false;
                }

                return false;

            }).ToArray();
            TypeHelper.fields.Add(aspectType, arr);

            return arr;

        }
        
    }

}