using SHOPModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Repositories
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncreaseCart(ShoppingCart cart , int count);
        int DecreaseCart(ShoppingCart cart , int count);
    }
}
