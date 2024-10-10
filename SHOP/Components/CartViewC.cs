using Microsoft.AspNetCore.Mvc;
using SHOPDataAccessLayer.Implementation;
using SHOPModels.Repositories;
using System.Security.Claims;

namespace SHOP.Components
{
    public class CartViewCViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartViewCViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32("Cart") != null)
                {
                    return View(HttpContext.Session.GetInt32("Cart"));
                }
                else
                {
                    int count = 0;
                    foreach (var v in _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == claim.Value).ToList()) 
                    {
                       count += v.Count;
                    }
                   // HttpContext.Session.SetInt32("Cart", _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == claim.Value).ToList().Count());
                    HttpContext.Session.SetInt32("Cart", count);
                    return View(HttpContext.Session.GetInt32("Cart"));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
