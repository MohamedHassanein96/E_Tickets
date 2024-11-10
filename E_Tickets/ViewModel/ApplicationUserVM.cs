using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.ViewModel
{
    public class ApplicationUserVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [ValidateNever]
        public string EmailConfirmed { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirmed { get; set; }

        [Required]
        public string City { get; set; }
    }
}
