using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3 ,ErrorMessage = "the Length must be greater than 2") ]
        [MaxLength(20 , ErrorMessage = "the Length mustn't be greater than 20")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(20, ErrorMessage = "the Length mustn't be greater than 20")]
        public string LastName { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(1000, ErrorMessage = "the Length mustn't be greater than 1000")]

        public string Bio { get; set; }
        [ValidateNever]
        public string ProfilePicture { get; set; }
        [Required]
        public string News { get; set; }

        [ValidateNever]
        public ICollection<Movie> Movies { get; set; }

        [ValidateNever]

        public List<ActorMovie> ActorMovies { get; set; }
    }
}
