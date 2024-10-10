using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Models
{
    public class ShoppingCart
    {
        
        public int Id { get; set; }



        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Range(1, 10, ErrorMessage = "Must enter value between 1 and 10")]

        public int Count { get; set; }
        public DateTime ShoppingCartDate { get; set; } = DateTime.Now;

        //[ValidateNever]
        //public OrderHeader OrderHeader { get; set; }
    }
}
