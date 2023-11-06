using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace QuickGrid.Crud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : Controller
    {
        [HttpGet("{fileName}/{fileType}")]
        public async Task<IActionResult> Export(string fileName, string fileType)
        {
            var diretorio = "wwwroot";

            // Verifica se o tipo de arquivo é suportado
            if (fileType != "xlsx" && fileType != "csv" && fileType != "txt" && fileType != "xml")
            {
                return BadRequest("Tipo de arquivo não suportado.");
            }

            // Caminho do arquivo.
            var filePath = Path.Combine(diretorio, $"{fileName}.{fileType}");

            // Verifique se o arquivo existe.
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Leia o arquivo em memória.
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            // Exclua o arquivo após a leitura.
            System.IO.File.Delete(filePath);

            // Configure a resposta.
            string mimeType = fileType switch
            {
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "txt" => "text/plain",
                "xml" => "application/xml",
                _ => "application/octet-stream"
            };

            memory.Position = 0;
            return File(memory, mimeType, $"{fileName}.{fileType}");
        }
    }
}
