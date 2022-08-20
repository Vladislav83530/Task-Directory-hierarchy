using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using TestTask.Data;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class Import_ExportController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public Import_ExportController(IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _webHost = webHost;
            _context = context;
        }

        [HttpGet]
        public IActionResult ImportFile() => View();

        [HttpGet]
        public PhysicalFileResult ExportYourFolders()
        {
            var users_folders = _context.Catalogs.Where(x => x.UserID == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            string json = JsonSerializer.Serialize(users_folders);

            string path = Path.Combine(_webHost.ContentRootPath, @$"ImportFiles\{User.FindFirst(ClaimTypes.NameIdentifier).Value}.json");
            System.IO.File.Create(path).Close();
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(json);
            }
            return PhysicalFile(path, MediaTypeNames.Application.Json, "ImportYourFolders.json");
        }

        [HttpPost]
        public IActionResult ImportYourFolders(IFormFile json)
        {
            using (StreamReader sr = new StreamReader(json.OpenReadStream()))
            {
                var fileContent = sr.ReadToEnd();
                try
                {
                    var catalogs = JsonSerializer.Deserialize<List<Catalog>>(fileContent);
                    foreach (var catalog in catalogs)
                    {
                        if (!_context.Catalogs.Contains(catalog))
                        {
                            _context.Catalogs.Add(catalog);
                        }
                    }
                    _context.SaveChanges();
                    return RedirectToAction("WelcomePage", "Home");
                }
                catch (Exception)
                {
                    return BadRequest("Invalid JSON");
                }
            }
        }
    }
}
