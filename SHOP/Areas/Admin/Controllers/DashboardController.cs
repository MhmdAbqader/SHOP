using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOPModels.Repositories;
using Utilities;

namespace SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SDRoles.AdminRole)]
    public class DashboardController : Controller
    {

        private IUnitOfWork _unitOfWork;
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ViewBag.Orders = _unitOfWork.OrderHeaderRepository.GetAll().Count();
            ViewBag.ApprovedOrders = _unitOfWork.OrderHeaderRepository.GetAll(x => x.OrderStatus == StatusType.Approved).Count();
            ViewBag.Users =7; // i will do repo to treate with User operation such as getall getbyid lock profile and etc....
            ViewBag.Products = _unitOfWork.ProductRepository.GetAll().Count();
            return View();
        }
    }
}
