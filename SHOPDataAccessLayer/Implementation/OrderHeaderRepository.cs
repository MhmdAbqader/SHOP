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
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader OrderHeader)
        {
            _context.OrderHeaders.Update(OrderHeader);
        }

        public void UpdateOrderStatus(int id, string oStatus, string paymentStatus)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault( a => a.Id == id );
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = oStatus;
                if (paymentStatus != null) 
                {
                    orderHeader.PaymentStatus = paymentStatus;
                    orderHeader.PaymentDate = DateTime.Now.ToShortDateString();
                }
            }
        }
    }
}
