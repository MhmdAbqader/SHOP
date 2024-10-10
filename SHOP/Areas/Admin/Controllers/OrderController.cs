using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SHOPModels.Repositories;
using SHOPModels.ViewModels;
using Stripe;
using Utilities;

namespace SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SDRoles.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var orders = _unitOfWork.OrderHeaderRepository.GetAll(a => a.Id > 0, new string[] { "ApplicationUser" }) ;

            return View(orders);
        }


        [HttpGet]
        public IActionResult GetDataByDatatable()
        {
            return Json(new { result = _unitOfWork.OrderHeaderRepository.GetAll(a => a.Id > 0, new string[] { "ApplicationUser" }) });
        }

        public IActionResult Details(int id) 
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.GetById(o => o.Id == id, "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetailsRepository.GetAll(od => od.OrderHeaderId == id, new[] { "Product"})
            };

            return View(orderVM);
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails(OrderVM oVM)
		{
            var orderDB = _unitOfWork.OrderHeaderRepository.GetById(oVM.OrderHeader.Id);

            if (orderDB != null) {
                orderDB.CustmerName = oVM.OrderHeader.CustmerName;
                orderDB.Phone = oVM.OrderHeader.Phone;
                orderDB.City = oVM.OrderHeader.City;
                orderDB.Address = oVM.OrderHeader.Address;
                if (oVM.OrderHeader.Carrier != null && oVM.OrderHeader.TrackingNo != null) 
                {
                    orderDB.Carrier = oVM.OrderHeader.Carrier;
                    orderDB.TrackingNo = oVM.OrderHeader.TrackingNo;
                }
            }
            _unitOfWork.OrderHeaderRepository.Update(orderDB);
            _unitOfWork.Complete();
			TempData["ToastrMSG"] = "Saved Successfully Thanks";
			return RedirectToAction("Details","Order",new { id = oVM.OrderHeader.Id});
 
		}


        // i will TEst bindProperty soon after finsh process and shipping inshaalah

		[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess(OrderVM oVM) 
        {
			 
			_unitOfWork.OrderHeaderRepository.UpdateOrderStatus(oVM.OrderHeader.Id, StatusType.Processing, null);
			_unitOfWork.Complete();
			TempData["ToastrMSG"] = "Saved Successfully Thanks";
			return RedirectToAction("Details", "Order", new { id = oVM.OrderHeader.Id });
            			 
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip(OrderVM oVM)
		{
			var orderDB = _unitOfWork.OrderHeaderRepository.GetById(oVM.OrderHeader.Id);

			if (orderDB != null)
			{	
                orderDB.Carrier = oVM.OrderHeader.Carrier;
				orderDB.TrackingNo = oVM.OrderHeader.TrackingNo;
                orderDB.ShipppingDate = DateTime.Now;                
                orderDB.OrderStatus = StatusType.Shipped;                
			}
			_unitOfWork.OrderHeaderRepository.Update(orderDB);
			_unitOfWork.Complete();
			TempData["ToastrMSG"] = "Saved Successfully Thanks";
			return RedirectToAction("Details", "Order", new { id = oVM.OrderHeader.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder(OrderVM oVM)
		{
			var orderDB = _unitOfWork.OrderHeaderRepository.GetById(oVM.OrderHeader.Id);

			if (orderDB != null)
			{
                if (orderDB.PaymentStatus == StatusType.Approved)
                {
                    var option = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderDB.PaymentIntendedId
                    };
                    var service = new RefundService();
                    Refund refund = service.Create(option);
                    _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderDB.Id, StatusType.Cancelled, StatusType.Refund);
                }
                else // if the order payment is Pending 
                {
					_unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderDB.Id,StatusType.Cancelled,StatusType.Cancelled);
				}
                _unitOfWork.Complete();
			}
			
			
			TempData["ToastrMSG"] = "Cancelled Order Successfully Thanks";
			return RedirectToAction("Details", "Order", new { id = oVM.OrderHeader.Id });
		}
	}
}
