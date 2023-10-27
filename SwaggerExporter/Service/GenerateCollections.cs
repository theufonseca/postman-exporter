using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SwaggerExporter.Models;
using System.Reflection;

namespace SwaggerExporter.Service
{
    public static class SwaggerExporterServiceExtensions
    {
        public static IServiceCollection AddSwaggerExporter(this IServiceCollection services, Assembly[] Assemblies)
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

            SaveFile(rootObject, services);

            return services;
        }

        private static void SaveFile(RootObject rootObject, IServiceCollection services)
        {
            var json = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(ms);
                writer.Write(json);
                writer.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                var fileModel = new FileModel
                {
                    Stream = new MemoryStream()
                };

                ms.CopyTo(fileModel.Stream);

                services.AddSingleton(fileModel);
            }
        }
    }
}
