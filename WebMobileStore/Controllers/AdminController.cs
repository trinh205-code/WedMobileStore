using Microsoft.AspNetCore.Mvc;
using WebMobileStore.Models.Data;

namespace WebMobileStore.Controllers
{

    [Route("/Admin")]
    public class AdminController : Controller
    {
        private MobileStoreContext db;

        public AdminController(MobileStoreContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {


            return View();
        }
    }
}
