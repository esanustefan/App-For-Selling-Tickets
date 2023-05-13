using System;
using System.Collections.Generic;
using ConcursBaschet.domain;
using ConsoleApplication1;

namespace Services
{
    public interface ServiceInterface
    {
        User getUser(UserDTO userDTO);
        
        User logIn(UserDTO userDTO,  ObserverInterface client);
        
        void logOut(User user, ObserverInterface client);
        
        void buyTickets(TicketDTO ticketDto,  ObserverInterface client);

        void addTicket(int id, string clientName, int numberOfSeats);

        void addUser(int id, string username, string password);
        
        void addMatch(int id, string team1, string team2, double ticketPrice, int soldSeats);


        IEnumerable<Ticket> getAllTickets();

        IEnumerable<User> getAllUsers();

        IEnumerable<Match> getAllMatches(ObserverInterface client);

        void updateMatch(int id, string team1, string team2, double ticketPrice, int soldSeats);

        Ticket findOneTicket(int id);

        User findOneUser(int id);

        Match findOneMatch(int id);

        int CheckifUserExists(UserDTO userDTO);

        int CheckAvailableSeats(MatchNoTicketsDTO matchNoTicketsDTO);

    }
}