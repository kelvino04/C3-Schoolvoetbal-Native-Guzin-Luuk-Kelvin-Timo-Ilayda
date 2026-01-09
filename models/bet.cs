using System;

namespace praC3.Models
{
    public class Bet
    {
        public string Username { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public string MatchName { get; set; }
        public string TeamName { get; set; }
        public int Amount { get; set; }
        public string Result { get; set; }
        public DateTime PlacedAt { get; set; }
    }
}
