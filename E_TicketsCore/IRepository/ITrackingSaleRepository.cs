using E_TicketsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.IRepository
{
    public interface ITrackingSaleRepository :IBaseRepository<TrackingSale>
    {
        TrackingSale CreateSpecial(Cart cart);

    }
}
