using SHOPDataAccessLayer.Data;
using SHOPModels.Models;
using SHOPModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPDataAccessLayer.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var existedProduct = _context.Products.FirstOrDefault(a => a.Id == product.Id);
            if (existedProduct != null)
            {
                existedProduct.Name = product.Name;
                existedProduct.Description = product.Description;
                existedProduct.Price = product.Price;
                existedProduct.ImageUrl = product.ImageUrl;
                existedProduct.CategoryId = product.CategoryId;

            }
        }
    }
}
