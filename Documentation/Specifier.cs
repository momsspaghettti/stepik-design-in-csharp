using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;


namespace Documentation {
    public class Specifier<T> : ISpecifier {
        public string GetApiDescription() {
            return typeof(T).GetCustomAttribute<ApiDescriptionAttribute>()?.Description;
        }

        public string[] GetApiMethodNames() {
            return typeof(T)
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ApiMethodAttribute>() != null)
                .Select(m => m.Name)
                .ToArray();
        }

        private MethodInfo GetMethod(string methodName) {
            return typeof(T).GetMethod(methodName);
        }

        public string GetApiMethodDescription(string methodName) {
            var method = GetMethod(methodName);
            if (method == null)
                return null;
            if (method.GetCustomAttribute<ApiMethodAttribute>() == null)
                return null;

            return method.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;
        }

        public string[] GetApiMethodParamNames(string methodName) {
            var method = GetMethod(methodName);
            if (method == null)
                return null;
            if (method.GetCustomAttribute<ApiMethodAttribute>() == null)
                return null;
            return method.GetParameters().Select(p => p.Name).ToArray();
        }

        private ParameterInfo GetApiMethodParamDesc(string methodName, string paramName) {
            var method = GetMethod(methodName);
            if (method == null)
                return null;
            if (method.GetCustomAttribute<ApiMethodAttribute>() == null)
                return null;
            var paramsWithName =
                method.GetParameters().Where(p => p.Name == paramName);
            if (!paramsWithName.Any())
                return null;
            return paramsWithName.First();
        }

        public string GetApiMethodParamDescription(string methodName, string paramName) {
            var paramWithName = GetApiMethodParamDesc(methodName, paramName);
            return paramWithName?.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;
        }

        private static void
            FillInApiParamDescription(ParameterInfo parameterInfo, ApiParamDescription paramDescription) {
            paramDescription.ParamDescription.Description =
                parameterInfo.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

            paramDescription.Required =
                parameterInfo.GetCustomAttribute<ApiRequiredAttribute>() != null &&
                parameterInfo.GetCustomAttribute<ApiRequiredAttribute>().Required;

            paramDescription.MaxValue = parameterInfo.GetCustomAttribute<ApiIntValidationAttribute>()?.MaxValue;
            paramDescription.MinValue = parameterInfo.GetCustomAttribute<ApiIntValidationAttribute>()?.MinValue;
        }

        public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName) {
            var paramWithName = GetApiMethodParamDesc(methodName, paramName);
            var result = new ApiParamDescription();
            result.ParamDescription = new CommonDescription(paramName);
            if (paramWithName == null)
                return result;

            FillInApiParamDescription(paramWithName, result);

            return result;
        }

        public ApiMethodDescription GetApiMethodFullDescription(string methodName) {
            var method = GetMethod(methodName);
            if (method == null)
                return null;
            if (method.GetCustomAttribute<ApiMethodAttribute>() == null)
                return null;

            var result = new ApiMethodDescription();
            result.MethodDescription = new CommonDescription(methodName);
            result.MethodDescription.Description =
                method.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

            result.ParamDescriptions = method.GetParameters()
                .Select(p => GetApiMethodParamFullDescription(methodName, p.Name))
                .ToArray();

            if (method.ReturnParameter != null && method.ReturnParameter.ParameterType == typeof(void))
                return result;

            result.ReturnDescription = new ApiParamDescription {ParamDescription = new CommonDescription()};
            FillInApiParamDescription(method.ReturnParameter, result.ReturnDescription);

            return result;
        }
    }
}