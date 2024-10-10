using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOPDataAccessLayer.Implementation;
using SHOPModels.Models;
using SHOPModels.Repositories;
using System.Security.Claims;
using X.PagedList;  //this package for Pagination 

namespace SHOP.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int ? page)
        {
            //  ----- Pagination used here -------  
            // ------------------------*******************Pagination**************-------------------------------
            var pageNo = page ?? 1;
            int pageSize = 8;
            var products = _unitOfWork.ProductRepository.GetAll().ToPagedList(pageNo,pageSize);
            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            ShoppingCart shopcart = new ShoppingCart()
            {
                Product = _unitOfWork.ProductRepository.GetById(a => a.Id == id, includeTable:"Category"  ),
                Count = 1 
            };
            return View(shopcart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Add2Cart(ShoppingCart cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var CurrentUser = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string uid = CurrentUser.Value;

            ShoppingCart shoppingCart = new ShoppingCart 
            {
                ProductId = cart.Product.Id,
                UserId = uid,
                Count=cart.Count,
                ShoppingCartDate = DateTime.Now,
            };
            var existShoppingCart = _unitOfWork.ShoppingCartRepository.GetById(c => c.UserId == uid && c.ProductId == cart.Product.Id);

            if (existShoppingCart != null)
            {
                existShoppingCart.Count = _unitOfWork.ShoppingCartRepository.IncreaseCart(existShoppingCart,cart.Count);
                _unitOfWork.Complete();

                int countOFCart = HttpContext.Session.GetInt32("Cart").Value;
                countOFCart += cart.Count;
                HttpContext.Session.SetInt32("Cart", countOFCart);
               // HttpContext.Session.SetInt32("Cart", _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid).Count());
            }
            else 
            {

                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Complete();
                if (HttpContext.Session.GetInt32("Cart") != null)
                {
                    int countOFCart = HttpContext.Session.GetInt32("Cart").Value;
                    countOFCart += cart.Count;
                    HttpContext.Session.SetInt32("Cart", countOFCart);
                }
                else
                {
                    HttpContext.Session.SetInt32("Cart", cart.Count);
                }
                //HttpContext.Session.SetInt32("Cart", _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid).Count());

            }
           
            return RedirectToAction("Index");
        }
    }
}
