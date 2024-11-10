using E_TicketsCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_TicketsEF.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Actor>()
            .HasMany(a => a.Movies)
            .WithMany(m => m.Actors)


            .UsingEntity<ActorMovie>(
             j => j
                 .HasOne(am => am.Movie)
                 .WithMany(m => m.ActorMovies)
                 .HasForeignKey(am => am.MovieId),

             j => j
                 .HasOne(am => am.Actor)
                 .WithMany(a => a.ActorMovies)
                 .HasForeignKey(am => am.ActorId),

             j => j.
                  HasKey(am => new { am.ActorId, am.MovieId })
         );
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => new { c.MovieId, c.ApplicationUserId });
                entity.HasOne(c => c.Movie)
                .WithMany()
                .HasForeignKey(c => c.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
                });
            }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
        public DbSet<Cart> Carts { get; set; } 
        public DbSet<TrackingSale> TrackingSales { get; set; } 
    }
}
