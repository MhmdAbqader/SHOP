using SHOPDataAccessLayer.Data;
using SHOPModels.Models;
using SHOPModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SHOPDataAccessLayer.Implementation
{
    public class CategoryRepository : GenericRepository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context; 
        }
        public void Update(Category category)
        {
            //_context.Update(category);
            var existCategory = _context.Categories.FirstOrDefault(a => a.Id == category.Id);
            if (existCategory != null)
            {
                existCategory.Name = category.Name;
                existCategory.Description = category.Description;
                existCategory.CreatedTime = category.CreatedTime;
            }
        }
    }
}
