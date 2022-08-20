using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestTask.Data;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUploadService _uploadService;

        public HomeController(ApplicationDbContext context, IUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Index(Guid Id)
        {
            var currentItem = _context.Catalogs.FirstOrDefault(y => y.id == Id);
            var items = _context.Catalogs.Where(x => x.ParentID == Id).ToList();
            
            IndexViewModel vm = new IndexViewModel()
            {
                Catalogs = items,
                CurrentCatalog = currentItem
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult TestData()
        {
            IndexViewModel ivm = new IndexViewModel()
            {
                Catalogs = _context.Catalogs.Where(x => x.ParentID == new Guid("00000000-0000-0000-0000-000000000000") & x.UserID == null).ToList(),
                CurrentCatalog = null
            };
            return View("Index", ivm);
        }

        [HttpGet]
        public IActionResult WelcomePage() => View();

        [HttpGet]
        public IActionResult Importer() => View();

        [HttpPost]
        public async Task<IActionResult> Upload(string json)
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrEmpty(json)) 
                return BadRequest();

             await _uploadService.Upload(json, UserID);

            return RedirectToAction("WelcomePage", "Home");
        }

        [HttpGet]
        public IActionResult GetUsersCatalogs()
        {
            if (string.IsNullOrWhiteSpace(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)) return BadRequest();
            var catalogs = _context.Catalogs
                .Where(y => y.ParentID == new Guid("00000000-0000-0000-0000-000000000000"))
                .Where(y => y.UserID == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            IndexViewModel ivm = new IndexViewModel()
            {
                Catalogs = catalogs,
                CurrentCatalog = null,
            };
            return View("Index", ivm);
        }

        [HttpGet]
        public IActionResult DeleteFolder(Guid id)
        {
            bool isUserFolder;
            Guid guid = DeleteFolderFromDB(id, out isUserFolder);
            return  RedirectToAction("WelcomePage");
        }

        private Guid DeleteFolderFromDB(Guid id, out bool isUserFolder)
        {
            var catalog = _context.Catalogs.FirstOrDefault(x => x.id == id);
            isUserFolder = false;
            bool zaglushka = true;
            if (catalog == null) return new Guid("00000000-0000-0000-0000-000000000000");
            while (_context.Catalogs.Any(x => x.ParentID == catalog.id))
            {
                List<Catalog> catalogs = _context.Catalogs.Where(x => x.ParentID == catalog.id).ToList();
                foreach (var item in catalogs)
                {
                    DeleteFolderFromDB(item.id, out zaglushka);
                }
            }
            isUserFolder = catalog.UserID == User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.Catalogs.Remove(catalog);
            _context.SaveChanges();
            return catalog.ParentID;
        }
    }
}