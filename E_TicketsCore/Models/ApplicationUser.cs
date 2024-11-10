
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string City { get; set; }

        [ValidateNever]

        public List<Cart> Carts { get; set; }
    }
}
