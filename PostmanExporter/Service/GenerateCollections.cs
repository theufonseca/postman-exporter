using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PostmanExporter.Models;
using System.Reflection;

namespace PostmanExporter.Service
{
    public static class PostmanExporterServiceExtensions
    {
        public static IServiceCollection AddPostmanExporter(this IServiceCollection services, Assembly[] Assemblies)
        {
            var types = Assemblies.SelectMany(a => a.GetTypes()).ToArray();
            var controllers = GeneratorHelper.GetControllers(types);

            if (!controllers.Any())
                return services;

            var rootObject = new RootObject();
            rootObject.info.schema = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json";
            rootObject.info.name = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "Any Name";

            foreach (var controller in controllers)
            {
                var controllerName = GeneratorHelper.GetControllerName(controller);
                var actions = GeneratorHelper.GetActions(controller);
                var controllerPath = GeneratorHelper.GetControllerPath(controller, controllerName);

                foreach (var action in actions)
                {
                    string body = string.Empty;
                    var methodName = GeneratorHelper.GetMethodName(action);
                    var httpMethod = GeneratorHelper.GetHttpMethod(action);
                    var methodPath = GeneratorHelper.GetMethodPath(action);

                    if (httpMethod == "POST" || httpMethod == "PUT")
                        body = GeneratorHelper.GetBody(action);

                    var name = $"{controllerName} - {methodName}";
                    var fullPath = $"{controllerPath}/{methodPath}";

                    rootObject.AddItem(name, fullPath, httpMethod, body);
                }
            }

            services.AddSingleton(rootObject);

            return services;
        }
    }
}
