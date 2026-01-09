using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace praC3.Models
{
    namespace praC3.Models
    {
        public class MatchResult
        {
            public int Match_Id { get; set; }
            public Team Team1 { get; set; }  // use existing Team class
            public Team Team2 { get; set; }
            public string Score { get; set; } // "5-1"
        }
    }


}
