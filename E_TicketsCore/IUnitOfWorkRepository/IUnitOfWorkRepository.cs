using E_TicketsCore.IRepository;
using E_TicketsCore.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.IUnitOfWorkRepository
{
    public interface IUnitOfWorkRepository :IDisposable
    {
        IBaseRepository<Actor> Actors { get; }
        IBaseRepository<Movie> Movies { get; }
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<Cinema> Cinemas { get; }
        IBaseRepository<ActorMovie> ActorMovies { get; }
        IBaseRepository<Cart> Carts { get; }
        IBaseRepository<ApplicationUser> Users { get; }
        IBaseRepository<IdentityRole> Roles { get; }
        ITrackingSaleRepository TrackingSales { get; }

        void Complete();

    }
}
