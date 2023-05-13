using System;
using ConcursBaschet.domain;

namespace ConsoleApplication1
{
    [Serializable]
    public class MatchNoTicketsDTO
    {
        public Match match { get; set; }

        public int NoTickets { get; set; }
        

        public MatchNoTicketsDTO(Match match, int noTickets)
        {
            this.match = match;
            NoTickets = noTickets;
        }
    }
}