using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace praC3.Models;

class Bet
{
    public string Username { get; set; }
    public int MatchId { get; set; }
    public string Prediction { get; set; }
    public int Amount { get; set; }
    public bool? Won { get; set; }
}
