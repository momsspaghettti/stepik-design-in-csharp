namespace MyPhotoshop {
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters : IParameters, new() {
        public ParameterInfo[] GetParameters() {
            return new TParameters().GetDescription();
        }

        public Photo Process(Photo original, double[] parameterCoefficients) {
            var parameters = new TParameters();
            parameters.Parse(parameterCoefficients);
            return Process(original, parameters);
        }

        public abstract Photo Process(Photo original, TParameters parameters);
    }
}