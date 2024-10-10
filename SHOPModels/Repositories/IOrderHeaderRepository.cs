using SHOPModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Repositories
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void Update(OrderHeader OrderHeader);
        void UpdateOrderStatus( int id,string oStatus , string paymentStatus);
    }
}
