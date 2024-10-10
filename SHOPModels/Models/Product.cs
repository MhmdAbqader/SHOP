using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [ValidateNever] // to remove the validation from on it 
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        [DisplayName("Categor")]
        [ForeignKey("Category")]
        [Required]
        public int CategoryId { get; set; }

        [ValidateNever] // to remove the validation from on it 
        public Category  Category { get; set; }


    }
}
