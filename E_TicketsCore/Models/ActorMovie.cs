using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class ActorMovie
    {
        public int ActorId { get; set; }
        [ValidateNever]
        public Actor Actor { get; set; }


        public int MovieId { get; set; }
        [ValidateNever]

        public Movie Movie { get; set; }

    }
}
