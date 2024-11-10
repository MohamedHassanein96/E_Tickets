using E_TicketsCore.IRepository;
using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsEF.Data;
using E_TicketsEF.Repository;
using Microsoft.AspNetCore.Identity;
using System;

namespace E_TicketsEF.UnitOfWorkRepository
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public IBaseRepository<Actor> Actors { get; private set; }
        public IBaseRepository<Movie> Movies { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<Cinema> Cinemas { get; private set; }
        public IBaseRepository<ActorMovie> ActorMovies { get; private set; }
        public IBaseRepository<Cart> Carts { get; private set; }
        public IBaseRepository<ApplicationUser> Users { get; private set; }
        public IBaseRepository<IdentityRole> Roles { get; private set; }
        public ITrackingSaleRepository TrackingSales { get; private set; } 

        private readonly ApplicationDbContext _context;

        public UnitOfWorkRepository(ApplicationDbContext context)
        {
            _context = context;
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            Actors = CreateRepository<Actor>();
            Movies = CreateRepository<Movie>();
            Categories = CreateRepository<Category>();
            Cinemas = CreateRepository<Cinema>();
            ActorMovies = CreateRepository<ActorMovie>();
            Carts = CreateRepository<Cart>();
            Users = CreateRepository<ApplicationUser>();
            Roles = CreateRepository<IdentityRole>();
            TrackingSales = new TrackingSaleRepository(_context); 
        }

        private IBaseRepository<T> CreateRepository<T>() where T : class
        {
            if (typeof(T) == typeof(ApplicationUser))
            {
                return (IBaseRepository<T>)new BaseRepository<ApplicationUser>(_context);
            }
            if (typeof(T) == typeof(IdentityRole))
            {
                return (IBaseRepository<T>)new BaseRepository<IdentityRole>(_context);
            }
            return new BaseRepository<T>(_context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
