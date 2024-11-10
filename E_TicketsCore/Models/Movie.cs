
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class Movie
    {
       

        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(40, ErrorMessage = "the Length mustn't be greater than 40")]
        public string Name { get; set; }
        [Range(1, 100)]
        [Display(Name = "Available Tickets")]
        public int TicketsCount { get; set; } = 100;
        [Required]
        [MinLength(3, ErrorMessage = "the Length must be greater than 2")]
        [MaxLength(200, ErrorMessage = "the Length mustn't be greater than 200")]
        public string Description { get; set; }

        [Range(50,200, ErrorMessage = "the Range must be between than 50 to 200")]
        public decimal Price { get; set; }
        [ValidateNever]
        public string ImgUrl { get; set; }
        public string TrailerUrl { get; set; }


        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime EndDate { get; set; }
       
        public enum MovieStatus
        {
            NotAvailable = 0,
            Available = 1,
            Upcoming = 2
        }

        [ValidateNever]
        public MovieStatus Status { get; set; }


        [ValidateNever]
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Cinema Cinema { get; set; }
        public int CinemaId { get; set; }

        [ValidateNever]
        public ICollection<Actor> Actors { get; set; }
        [ValidateNever]
        public List<ActorMovie> ActorMovies { get; set; }
        [ValidateNever]

        public List<Cart> Carts { get; set; }

        internal void UpdateStatus()
        {
            var today = DateTime.Today;
            if (today < StartDate)
            {
                Status = MovieStatus.Upcoming;
            }
            else if (today >= StartDate && today <= EndDate)
            {
                Status = MovieStatus.Available;
            }
            else
            {
                Status = MovieStatus.NotAvailable;
            }
        }
    }
}
