using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace praC3.Models
{
    public class Bet
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string MatchName { get; set; }
        public string TeamName { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; } // Pending / Won / Lost
    }
}
