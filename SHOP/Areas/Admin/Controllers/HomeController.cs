using Microsoft.AspNetCore.Mvc;

namespace SHOP.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("admin")] // add this line to be specified in route table and to be known this controller is under Admin Area 
                        // so that we must add this attribute 
        public IActionResult Index()
        {
            return View();
        }
    }
}
