using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;

namespace WebMobileStore.Controllers
{
    [Route("/cate")]
    public class TestController : Controller
    {
        private MobileStoreContext db;

        public TestController(MobileStoreContext context) {
            db = context;
        }

        [Route("list")]
        public IActionResult Index()
        {
            //var categoryGroup = db.CategoryGroups.ToList();

                var categoryGroups = db.CategoryGroups
                    .Include(cg => cg.Categories) 
                    .ToList();

                return View(categoryGroups);
        }
    }
}
