using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PostmanExporter.Service
{
    public class GeneratorHelper
    {
        public static Type[] GetControllers(Type[] types)
        {
            var controllers = types.Where(t => t.IsClass && t.IsPublic && t.Name.EndsWith("Controller") &&
                t.GetCustomAttribute<ApiControllerAttribute>() != null).ToArray();

            return controllers;
        }

        public static string GetControllerName(Type controller)
        {
            var controllerName = controller.Name.Replace("Controller", "");
            return controllerName;
        }

        public static MethodInfo[] GetActions(Type controller)
        {
            var attributesAccepteds = new string[] { "HttpPostAttribute", "HttpPutAttribute", "HttpDeleteAttribute", "HttpGetAttribute" };
            var methods = controller.GetMethods()
                .Where(m => m.IsPublic && m.GetCustomAttributes().Any(x => attributesAccepteds.Contains(x.GetType().Name)))
                .ToArray();
            return methods;
        }

        public static string GetMethodName(MethodInfo method)
        {
            var methodName = method.Name;
            return methodName;
        }

        public static string GetHttpMethod(MethodInfo method)
        {
            var attributesAccepteds = new string[] { "HttpPostAttribute", "HttpPutAttribute", "HttpDeleteAttribute", "HttpGetAttribute" };
            var httpMethodAttribute = method.GetCustomAttributes().Where(x => attributesAccepteds.Contains(x.GetType().Name)).FirstOrDefault();

            if (httpMethodAttribute is null)
                return string.Empty;

            var httpMethodName = httpMethodAttribute.GetType().Name.Replace("Attribute", "").Replace("Http", "").Trim().ToUpper();
            return httpMethodName;
        }

        public static string GetControllerPath(Type controller, string controllerName = "")
        {
            var controllerPath = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? string.Empty;

            if (!string.IsNullOrEmpty(controllerPath))
                controllerPath = controllerPath.Replace("[controller]", controllerName);

            return controllerPath.ToLower();
        }

        public static string GetMethodPath(MethodInfo method)
        {
            if (!string.IsNullOrEmpty(method.GetCustomAttribute<RouteAttribute>()?.Template))
                return method.GetCustomAttribute<RouteAttribute>()!.Template;

            if (!string.IsNullOrEmpty(method.GetCustomAttribute<HttpGetAttribute>()?.Template))
                return method.GetCustomAttribute<HttpGetAttribute>()!.Template;

            if (!string.IsNullOrEmpty(method.GetCustomAttribute<HttpPostAttribute>()?.Template))
                return method.GetCustomAttribute<HttpPostAttribute>()!.Template;

            if (!string.IsNullOrEmpty(method.GetCustomAttribute<HttpPutAttribute>()?.Template))
                return method.GetCustomAttribute<HttpPutAttribute>()!.Template;

            if (!string.IsNullOrEmpty(method.GetCustomAttribute<HttpDeleteAttribute>()?.Template))
                return method.GetCustomAttribute<HttpDeleteAttribute>()!.Template;

            return string.Empty;
        }

        public static string GetBody(MethodInfo method)
        {
            var body = string.Empty;

            if (method.GetCustomAttribute<HttpPostAttribute>() is null &&
                method.GetCustomAttribute<HttpPutAttribute>() is null)
                return body;

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                var parameter = parameters.FirstOrDefault(x => x.ParameterType.IsClass);

                if (parameter is not null)
                    body = GetBodyValue(parameter.ParameterType);
            }

            return body;
        }

        public static string GetBodyValue(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var body = string.Empty;

            if (!properties.Any())
                return string.Empty;

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                propertyName = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);

                var propertyType = property.PropertyType;

                if (IsPrimitive(propertyType) || propertyType.IsEnum)
                    body += $"\"{propertyName}\": {GenerateProtertyValue(propertyType)},";
                else
                    body += $"\"{propertyName}\": {GetBodyValue(propertyType)},";
            }

            body = $"{{ {body.TrimEnd(',')} }}";

            return body;
        }

        private static bool IsPrimitive(Type type)
        {
            if (type == typeof(string))
                return true;

            if (type == typeof(int))
                return true;

            if (type == typeof(long))
                return true;

            if (type == typeof(decimal))
                return true;

            if (type == typeof(short))
                return true;

            if (type == typeof(double))
                return true;

            if (type == typeof(float))
                return true;

            if (type == typeof(DateTime))
                return true;

            if (type == typeof(bool))
                return true;

            if (type == typeof(Guid))
                return true;

            return false;
        }

        private static string GenerateProtertyValue(Type type)
        {
            if (type == typeof(string))
                return "\"Any text\"";

            if (type == typeof(int))
                return "0";

            if (type == typeof(long))
                return "0";

            if (type == typeof(decimal))
                return "0";

            if (type == typeof(short))
                return "0";

            if (type == typeof(double))
                return "0";

            if (type == typeof(float))
                return "0";

            if (type == typeof(DateTime))
                return $"\"{DateTime.Now:s}\"";

            if (type == typeof(bool))
                return "false";

            if (type == typeof(Guid))
                return $"\"{Guid.NewGuid()}\"";

            if (type.IsEnum)
            {
                var enumValues = Enum.GetValues(type);
                var enumValue = enumValues.GetValue(0);

                if (enumValue is not null)
                    return $"{(int)enumValue}";

                return "0";
            }

            return string.Empty;
        }
    }
}
