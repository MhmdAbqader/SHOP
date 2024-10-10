using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SHOP.CustomizationNeeds;
using SHOP.DbInitialize;
using SHOPDataAccessLayer.Data;
using SHOPDataAccessLayer.Implementation;
using SHOPModels.Models;
using SHOPModels.Repositories;
using Stripe;
using System.Text.Json;
using Utilities;

namespace SHOP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // custom route to send a character o or c as a flag to lock and unlock user profile
            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("char", typeof(CharRouteConstraint));
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddDbContext<ApplicationDbContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("_cn"));
            });

            //builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            // {
            //   options.SignIn.RequireConfirmedAccount = false;
            // })
            //     .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
            }).AddDefaultTokenProviders()
           .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI();

            // we are inject and mapping the api Stripe Keys on our class named StripePayment 
            builder.Services.Configure<StripePayment>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddSession();


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseWebSockets();
            //app.UseWebSocketHandler();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //SeedDefault data when i delivered to the client
            SeedDataDefault();

			app.UseSession();
            // Binding stripe apikey value from appsetting 

            //StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            // another way to get the value 

            StripeConfiguration.ApiKey = StripeApiKey();
            app.UseAuthorization();
            app.MapRazorPages(); // to read Identity Pages

            //app.UseEndpoints(endpoints =>
            //{
            app.MapControllerRoute(
              name: "areas",
              //pattern: "{area:exists}/{controller=Category}/{action=Index}/{myID?}"
              pattern: "{area:exists}/{controller=Category}/{action=Index}/{id?}/{flag:char?}"
            );
            //});
            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
           );
            app.MapControllerRoute(
                name: "default",
                //pattern: "{controller=category}/{action=Index}/{myID?}");         
                pattern: "{controller=Home}/{action=Index}/{myID?}");

            app.Run();



			void SeedDataDefault()
			{
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbInitializer.Initialize();
                }
		    }
		}

     

        static string StripeApiKey()
        {

            string jsonString = System.IO.File.ReadAllText("appsettings.json");

            // Parse JSON
            JsonDocument document = JsonDocument.Parse(jsonString);

            // Navigate to nested "stripe" object and its keys
            JsonElement root = document.RootElement;

            // Get "stripe" object
            if (root.TryGetProperty("Stripe", out JsonElement stripeElement))
            {
                // Get values from "stripe"
                return stripeElement.GetProperty("SecretKey").GetString();
                // string key2Value = stripeElement.GetProperty("key2").GetString();
            }
            return "";
        }

        // another way by using Newtonsoft.Json 
        // this the easy way to get any value of any key from appsettings.json 
        static string StripeApiKey2()
        {
            string jsonString = System.IO.File.ReadAllText("appsettings.json");

            // Parse JSON to JObject
            JObject jsonObject = JObject.Parse(jsonString);

            // Navigate to "stripe" object and access its keys
            var stripeObject = jsonObject["stripe"];

            if (stripeObject != null)
            {
                string SecretKey = stripeObject["SecretKey"]?.ToString();
                string key2Value = stripeObject["key2"]?.ToString();
            }

            return "";
        }
    }
    
}