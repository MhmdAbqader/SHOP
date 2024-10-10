using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Models
{
    public class OrderHeader
    {
        // Order Information
        public int Id { get; set; }
      //  public string  UserId { get; set; }


        [ForeignKey("ApplicationUser")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipppingDate { get; set; } // ميعاد التحرك بالاوردر
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public int? TrackingNo { get; set; }
        public string? Carrier { get; set; }
        public string? PaymentDate { get; set; }

        // Stripe
        public string? SessionId { get; set; }
        public string? PaymentIntendedId { get; set; }

        // Customer Info
        public string CustmerName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
