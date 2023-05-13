using System;

namespace ConsoleApplication1
{
    [Serializable]
    public class TicketDTO
    {
        public MatchNoTicketsDTO matchNoTicketsDTO { get; set; }
        public string ClientName { get; set; }
        public int NumberOfSeats { get; set; }
        
        public TicketDTO(MatchNoTicketsDTO matchNoTicketsDTO, string clientName, int numberOfSeats)
        {
            this.matchNoTicketsDTO = matchNoTicketsDTO;
            ClientName = clientName;
            NumberOfSeats = numberOfSeats;
        }
    }
}