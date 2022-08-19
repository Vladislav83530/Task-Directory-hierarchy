using Microsoft.AspNetCore.Mvc;
using TestTask.Data;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}