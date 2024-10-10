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
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        //private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public int DecreaseCart(ShoppingCart cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }

        public int IncreaseCart(ShoppingCart cart, int count)
        {
            cart.Count += count;
            return cart.Count; 
        }
    }
}
