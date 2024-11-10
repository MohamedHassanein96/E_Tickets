using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(20, ErrorMessage = "the Length mustn't be greater than 20")]
        public string Name { get; set; }

        [ValidateNever]
        public ICollection<Movie> Movies { get; set; }

    }
}
