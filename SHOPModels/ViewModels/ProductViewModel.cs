using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SHOPModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        [ValidateNever] // to remove the validation from on it 
        public List<SelectListItem> CategoriesDDList { get; set; }
    }
}
