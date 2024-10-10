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
            ViewBag.Users =8;
            ViewBag.Products = _unitOfWork.ProductRepository.GetAll().Count();
            return View();
        }
    }
}
