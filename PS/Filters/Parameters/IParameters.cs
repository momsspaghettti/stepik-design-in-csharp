using System.Linq;

namespace MyPhotoshop {
    public interface IParameters { }

    public static class ParametersExtensions {
        public static ParameterInfo[] GetDescription(this IParameters parameters) {
            return parameters
                .GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes(false).OfType<ParameterInfo>().Any())
                .Select(p => p.GetCustomAttributes(false).OfType<ParameterInfo>().First())
                .ToArray();
        }

        public static void Parse(this IParameters parameters, double[] data) {
            int i = 0;
            foreach (
                var property in parameters
                    .GetType()
                    .GetProperties()
                    .Where(p => p.GetCustomAttributes(false).OfType<ParameterInfo>().Any())
            ) {
                property.SetValue(parameters, data[i++], new object[0]);
            }
        }
    }
}