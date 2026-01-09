using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace praC3.Models
{
    // Represents a team from the API
    public class TeamInfo
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    // Represents a match from the API
    public class MatchInfo
    {
        public int match_id { get; set; }
        public TeamInfo team1 { get; set; }
        public TeamInfo team2 { get; set; }
        public int? score1 { get; set; }      // nullable for not-played matches
        public int? score2 { get; set; }
        public int? winner_team_id { get; set; }
    }
}
