using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class Cart
    {
        public int MovieId {  get; set; }
        [ValidateNever]
        public Movie Movie { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Range(1,100, ErrorMessage = "the Range must be greater than 1 and less than 101")]
        public int Count  { get; set; }

    }
}
