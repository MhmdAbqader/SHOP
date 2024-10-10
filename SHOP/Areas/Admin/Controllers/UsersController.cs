using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOPDataAccessLayer.Data;
using System.Security.Claims;
using Utilities;

namespace SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SDRoles.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public   UsersController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
    
        public IActionResult AllUsers()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUser = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string uid = currentUser.Value;
            ViewBag.NoOfUsers = _context.ApplicationUsers.ToList().Count();
            return View(_context.ApplicationUsers.Where(a=>a.Id != uid).ToList());
        }

        public IActionResult Lock_UnlockProfile(string id,char flag) 
        {
            var v =  HttpContext.Request;
            var user = _context.ApplicationUsers.Find(id);
            if (user != null ) 
            {
                // using this step no need to flag  
                //if ((user.LockoutEnd == null || user.LockoutEnd < DateTime.Now))       
                if (flag == 'c')
                    user.LockoutEnd = DateTimeOffset.Now.AddHours(1);
                else
                    user.LockoutEnd = DateTimeOffset.Now;


                _context.SaveChanges();
                return RedirectToAction("AllUsers", "Users");
            }
            else
                return NotFound();                       
        }
        //public IActionResult Lock_UnlockProfile(string id) 
        //{
        //    var user = _context.ApplicationUsers.Find(id);
        //    if (user != null)
        //    {
        //        user.LockoutEnd = DateTimeOffset.Now;
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("AllUsers", "Users");
        //}
    }
}
