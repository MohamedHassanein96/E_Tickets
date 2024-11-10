using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.Models
{
    public class TrackingSale
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string ApplicationUserId { get; set; }
        public int Count { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
