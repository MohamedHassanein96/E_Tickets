using E_TicketsCore.IRepository;
using E_TicketsCore.Models;
using E_TicketsEF.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsEF.Repository
{
    public class TrackingSaleRepository : BaseRepository<TrackingSale>, ITrackingSaleRepository
    {
        private readonly ApplicationDbContext _context;
        
        public TrackingSaleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public TrackingSale CreateSpecial(Cart cart)
        {
            TrackingSale trackingSale = new()
            {
                ApplicationUserId = cart.ApplicationUserId,
                MovieId = cart.MovieId,

            };
            return trackingSale;
        
        }
    }
}
