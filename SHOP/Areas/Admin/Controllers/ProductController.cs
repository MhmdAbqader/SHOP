using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SHOPModels.Models;
using SHOPModels.Repositories;
using SHOPModels.ViewModels;
using Utilities;

namespace SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize( Roles = SDRoles.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment )
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetDataByDatatable()
        {

            return Json(new { data = _unitOfWork.ProductRepository.GetAll(a=>a.Id > 0, new string[] { "Category"}) });
        }

        public IActionResult Create() 
        {
            ProductViewModel model = new ProductViewModel();
            model.CategoriesDDList = _unitOfWork.CategoryRepository.GetAll()
                                    .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productViewModel, IFormFile productImage)
        {

            if (ModelState.IsValid)
            {
                if (productImage != null)
                {
                    //string fileName = productImage.FileName;     
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(productImage.FileName);

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    //string fullPathUploaded = Path.Combine(wwwRootPath, @"\Images\Products");
                    string fullPathUploaded = wwwRootPath + @"\Images\Products";

                    using (var fileStream = new FileStream(Path.Combine(fullPathUploaded, fileName + extension), FileMode.Create))
                    {
                        productImage.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"Images\Products\" + fileName + extension;
                }
                _unitOfWork.ProductRepository.Add(productViewModel.Product);
                _unitOfWork.Complete();
                TempData["ToastrMSG"] = "Saved Successfully Thanks";
                return RedirectToAction("Index");
            }
            else
            {
                productViewModel.CategoriesDDList = _unitOfWork.CategoryRepository.GetAll()
                                   .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
                return View(productViewModel);
            }
        }


        public IActionResult Edit(int? id)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = _unitOfWork.ProductRepository.GetById(a=>a.Id == id);
            model.CategoriesDDList = _unitOfWork.CategoryRepository.GetAll()
                                    .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel productViewModel, IFormFile? productImage)
        {

            if (ModelState.IsValid)
            {
                if (productImage != null)
                {
                    //string fileName = productImage.FileName;     
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(productImage.FileName);

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    //string fullPathUploaded = Path.Combine(wwwRootPath, @"\Images\Products"); // don't work this code
                    string fullPathUploaded = wwwRootPath + @"\Images\Products";

                    string[] files = System.IO.Directory.GetFiles(fullPathUploaded);

                    if ( files.Any(a => a.Substring(a.LastIndexOf("\\")).Equals(productViewModel.Product.ImageUrl.Substring(productViewModel.Product.ImageUrl.LastIndexOf("\\"))))) 
                    {
                        System.IO.File.Delete(wwwRootPath + "\\"+ productViewModel.Product.ImageUrl);
                    }

                    using (var fileStream = new FileStream(Path.Combine(fullPathUploaded, fileName + extension), FileMode.Create))
                    {
                        productImage.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"Images\Products\" + fileName + extension;
                }
                _unitOfWork.ProductRepository.Update(productViewModel.Product);
                _unitOfWork.Complete();
                TempData["ToastrMSG"] = "Saved Successfully Thanks";
                return RedirectToAction("Index");
            }
            else
                return View(productViewModel);
        }

    }
}
