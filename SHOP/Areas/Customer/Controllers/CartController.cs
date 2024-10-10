using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SHOPDataAccessLayer.Implementation;
using SHOPModels.Models;
using SHOPModels.Repositories;
using SHOPModels.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using Utilities;

namespace SHOP.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult AllProductsCart()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var CurrentUser = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            string uid = CurrentUser.Value;

            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

            shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid, new[] { "Product", "ApplicationUser" }).ToList();//Where(a => a.UserId == uid).ToList();
            ViewBag.CartCount = shoppingCarts.Count();
            ViewBag.Total = 0;
         
            foreach (var cart in shoppingCarts)
            {
                ViewBag.Total += cart.Count * cart.Product.Price;
            }

            return View(shoppingCarts);
        }


        [HttpGet]
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var CurrentUser = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            string uid = CurrentUser.Value;



            //List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();

            //shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid, new[] { "Product", "ApplicationUser" }).ToList();//Where(a => a.UserId == uid).ToList();

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartList = _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid, new[] { "Product", "ApplicationUser" }).ToList(),
                OrderHeader = new OrderHeader()
            };

            ViewBag.Total = 0;

            foreach (var cart in shoppingCartVM.CartList)
            {
                ViewBag.Total += cart.Count * cart.Product.Price;
            }

            foreach (var cart in shoppingCartVM.CartList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += cart.Count * cart.Product.Price;
            }

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ShoppingCartRepository.GetById(a=>a.UserId == uid).ApplicationUser;

            shoppingCartVM.OrderHeader.CustmerName = shoppingCartVM.OrderHeader.ApplicationUser.FullUserName;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.Address = shoppingCartVM.OrderHeader.ApplicationUser.Address;
            shoppingCartVM.OrderHeader.Phone = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            return View(shoppingCartVM);
        }


        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult PostSummary(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var CurrentUser = claimIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            string uid = CurrentUser.Value;

            shoppingCartVM.CartList = _unitOfWork.ShoppingCartRepository.GetAll(a => a.UserId == uid, new[] { "Product", "ApplicationUser" }).ToList();
               
            
            shoppingCartVM.OrderHeader.OrderStatus = StatusType.Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = StatusType.Pending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = uid;
        
            foreach (var cart in shoppingCartVM.CartList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += cart.Count * cart.Product.Price;
            }


            _unitOfWork.OrderHeaderRepository.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Complete();

            foreach (var item in shoppingCartVM.CartList) 
            {
                OrderDetails od = new OrderDetails();
                od.Price = (int)item.Product.Price;
                od.Count = item.Count;
                od.OrderHeaderId = shoppingCartVM.OrderHeader.Id;
                od.ProductId = item.Product.Id;


                _unitOfWork.OrderDetailsRepository.Add(od);
                _unitOfWork.Complete();
            }






            string myDomain = "http://localhost:5184/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                //static data
                #region
                //{
                //    new Stripe.Checkout.SessionLineItemOptions
                //    {
                //        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                //        {
                //            Currency = "usd",
                //            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                //            {
                //                Name = "T-shirt",
                //            },
                //            UnitAmount = 2000,
                //        },
                //        Quantity = 1,
                //    },
                //},
                #endregion

                Mode = "payment",
                SuccessUrl = myDomain + $"Customer/Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = myDomain + $"Customer/Home/Index",
            };


            foreach (var item in shoppingCartVM.CartList) 
            {
                var sessionLineItemOptions = new Stripe.Checkout.SessionLineItemOptions()
                {
                    Quantity = item.Count,
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                        UnitAmount =  (long)item.Product.Price,
                    },
                };
                options.LineItems.Add(sessionLineItemOptions);
            }


            
            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id; 
           
            _unitOfWork.Complete();
          
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        
        }

        public IActionResult OrderConfirmation(int id) 
        {
      

            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetById(id);

            if (orderHeader != null) 
            {
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid") 
                {
                    _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id,StatusType.Approved ,StatusType.Approved);
				    orderHeader.PaymentIntendedId = session.PaymentIntentId;
					_unitOfWork.Complete();
                }
            }

            List<ShoppingCart> shoppingCart = _unitOfWork.ShoppingCartRepository.GetAll(a=>a.UserId == orderHeader.ApplicationUserId).ToList();
            foreach (var item in shoppingCart)
            {
                _unitOfWork.ShoppingCartRepository.Remove(item);
                _unitOfWork.Complete();
            }

            return View(id);
        }


        public IActionResult Plus(int cartId)
        {
            var existCart = _unitOfWork.ShoppingCartRepository.GetById(cartId);
            _unitOfWork.ShoppingCartRepository.IncreaseCart(existCart, 1);
            _unitOfWork.Complete();
            int countOFCart = HttpContext.Session.GetInt32("Cart").Value;
            countOFCart += 1;
            HttpContext.Session.SetInt32("Cart", countOFCart);
            return RedirectToAction(nameof(AllProductsCart));
        }
        public IActionResult Minus(int cartId)
        {
            var existCart = _unitOfWork.ShoppingCartRepository.GetById(cartId);

            int countOFCart = HttpContext.Session.GetInt32("Cart").Value;
            countOFCart -= 1;
            HttpContext.Session.SetInt32("Cart", countOFCart);

            if (existCart.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(existCart);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index), "Home");
            }

            _unitOfWork.ShoppingCartRepository.DecreaseCart(existCart, 1);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(AllProductsCart));
        }
        public IActionResult Delete(int cartId)
        {
            var existCart = _unitOfWork.ShoppingCartRepository.GetById(cartId);
            _unitOfWork.ShoppingCartRepository.Remove(existCart);
            _unitOfWork.Complete();

            int countOFCart = HttpContext.Session.GetInt32("Cart").Value;
            countOFCart -= existCart.Count;
            HttpContext.Session.SetInt32("Cart", countOFCart);

            return RedirectToAction(nameof(AllProductsCart));
        }


        /// <summary>
        ///Manipulating  the OrderHeader Functions
        /// </summary>
        /// <returns></returns>

    
    }
}
