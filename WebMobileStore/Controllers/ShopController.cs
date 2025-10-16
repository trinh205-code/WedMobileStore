using Microsoft.AspNetCore.Mvc;

namespace WebMobileStore.Controllers
{
    [Route("/Shop")]
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
