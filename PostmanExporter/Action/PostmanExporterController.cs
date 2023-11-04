using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PostmanExporter.Models;
using System.IO;
using System.Reflection;

namespace PostmanExporter.Controllers
{
    [Route("PostmanExporter")]
    [ApiController]
    public class PostmanExporterController : ControllerBase
    {
        private readonly RootObject rootObject;

        public PostmanExporterController(RootObject rootObject)
        {
            this.rootObject = rootObject;
        }

        [HttpGet]
        public IActionResult DownloadFile()
        {
            var json = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(memoryStream);
                writer.Write(json);
                writer.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                //Get current assembly name
                var assemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "any_name";

                // Defina o nome do arquivo para o download
                string fileName = $"{assemblyName.ToLower()}.postman_collection.json";

                // Defina o tipo de conteúdo para o download (no caso de um arquivo json)
                string contentType = "application/json";

                // Posicione o ponteiro do MemoryStream no início
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Retorne o arquivo como um FileResult para download
                return File(memoryStream, contentType, fileName);
            }
        }
    }
}
