using Microsoft.AspNetCore.Mvc;
using PostmanExporter.Models;
using System.Reflection;

namespace PostmanExporter.Controllers
{
    [Route("PostmanExporter")]
    [ApiController]
    public class PostmanExporterController : ControllerBase
    {
        private readonly FileModel fileModel;

        public PostmanExporterController(FileModel fileModel)
        {
            this.fileModel = fileModel;
        }

        [HttpGet]
        public IActionResult DownloadFile()
        {
            // Crie um MemoryStream com o conteúdo do arquivo
            var memoryStream = fileModel.Stream;

            // Suponhamos que você já tenha os bytes do arquivo em memoryStream
            // Você pode preencher o memoryStream com os bytes do arquivo desejado
            
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
