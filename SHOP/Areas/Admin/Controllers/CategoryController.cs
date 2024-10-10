using Microsoft.AspNetCore.Mvc;
using SHOPDataAccessLayer.Data;
using SHOPDataAccessLayer.Implementation;
using SHOPModels.Models;
using SHOPModels.Repositories;

namespace SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
                        /* in this controller we can convert all code to repository and Unit of work to be clean and maintainable   */

        //private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(/*ApplicationDbContext applicationDbContext,*/IUnitOfWork unitOfWork)
        {
            //_context = applicationDbContext;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           //var myCategories = _context.Categories.ToList();
            var myCategories = _unitOfWork.CategoryRepository.GetAll();
            return View(myCategories);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                //_context.Categories.Add(category);
                //_context.SaveChanges();
                _unitOfWork.CategoryRepository.Add(category);
                _unitOfWork.Complete();
                TempData["ToastrMSG"] = "Saved Successfully Thanks";
                return RedirectToAction("Index");
            }
            else
                return View("Create", category);

        }
        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //return Content(string.Format($"Not found category with that id : {id} "));
            else
            {
                //var existCategory = _context.Categories.Find(id);
                var existCategory = _unitOfWork.CategoryRepository.GetById(res => res.Id ==id, null );
                return View("Edit", existCategory);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                //_context.Categories.Update(category);
                //_context.SaveChanges();
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            else
                return View("Edit", category);
        }

        [HttpGet]
        public IActionResult DeleteCategory([FromRoute] int? myID)
        {
            //var id =  Request.RouteValues["myID"].ToString();
            // var category = _context.Categories.Find(myID);
            var category = _unitOfWork.CategoryRepository.GetById(res => res.Id == myID, null);
            if (category != null)
            {
                //_context.Categories.Remove(category);
                //_context.SaveChanges();
                _unitOfWork.CategoryRepository.Remove(category);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
        
            return Content(string.Format($"Not found category with that id :  "));

        }
    }

}
