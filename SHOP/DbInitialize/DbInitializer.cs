using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SHOPDataAccessLayer.Data;
using SHOPModels.Models;
using Utilities;

namespace SHOP.DbInitialize
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;
        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
			_roleManager = roleManager;
			_context = context;
        }
        public void Initialize()
		{
			try
			{
				// check if any migartion is Pending you do it defore building roles
				if (_context.Database.GetPendingMigrations().Count() > 0)
				{
					_context.Database.Migrate();
				}
			
			}
			catch (Exception ex) 
			{
				throw;
			}
			//Role 
			if (!_roleManager.RoleExistsAsync(SDRoles.AdminRole).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole() { Name = SDRoles.AdminRole }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SDRoles.EditorRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SDRoles.CustomerRole)).GetAwaiter().GetResult();

				ApplicationUser user = new ApplicationUser();
				user.FullUserName = "Mhmd AbQader";
				user.City = "Assiut";
				user.Address = "Assiut/dayrout";
				user.Gender = "M";
				user.PhoneNumber ="01126545601";
				user.Email = "Mhmd@SHOP.com";
				user.UserName = "Mhmd@SHOP.com";
				//await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);					

				var result =  _userManager.CreateAsync(user, "Asdasd!1").GetAwaiter().GetResult();
				if (result.Succeeded)
				{
					var AdminUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "Mhmd@SHOP.com");

					if (AdminUser != null)
					{
						_userManager.AddToRoleAsync(user, SDRoles.AdminRole).GetAwaiter().GetResult();
					}
				}
			}

			return;
		}
	}
}
