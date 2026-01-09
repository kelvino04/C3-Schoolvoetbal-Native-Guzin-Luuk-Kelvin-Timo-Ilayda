using System.Collections.Generic;

namespace praC3.Models
{
    public class AppData
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Bet> BetHistory { get; set; } = new List<Bet>();
    }
}