using Services;
using System;
using System.Collections.Generic;
using ConcursBaschet.domain;
using ConcursBaschet.repo;
using ConsoleApplication1;


namespace Server
{
    public class Service: ServiceInterface
    {
        private MatchDBRepo matchDBRepository;
        private TicketDBRepo ticketDBRepository;
        private UserDBRepository userDBRepository;
        
        private readonly IDictionary<int, ObserverInterface> loggedClients;
        
        public Service(MatchDBRepo matchDBRepository, TicketDBRepo ticketDBRepository, UserDBRepository userDBRepository)
        {
            this.matchDBRepository = matchDBRepository;
            this.ticketDBRepository = ticketDBRepository;
            this.userDBRepository = userDBRepository;
            this.loggedClients = new Dictionary<int, ObserverInterface>();
        }
        
        public User getUser(UserDTO userDTO)
        {
            int id = userDBRepository.CheckifUserExists(userDTO.User, userDTO.Pass);
            if (id != -1)
            {
                return userDBRepository.findOne(id);
            }
            else
            {
                throw new Exception("User does not exist!");
            }
        }

        public User logIn(UserDTO userDTO, ObserverInterface client)
        {
            
            int userExists = userDBRepository.CheckifUserExists(userDTO.User, userDTO.Pass);
            if (userExists != -1)
            {
                User user = userDBRepository.findOne(userExists);
                if (loggedClients.ContainsKey(user.GetId()))
                {
                    throw new Exception("User already logged in!");
                }
                loggedClients[user.GetId()] = client;
                return user;
            }
            else
            {
                throw new Exception("Authentication failed!");
            }
        }
        
        public void logOut(User user, ObserverInterface client)
        {
            bool removed = loggedClients.Remove(user.GetId());
            if (!removed)
            {
                throw new Exception("User " + user.GetId() + " is not logged in!");
            }
        }
        
        
        public void buyTickets(TicketDTO ticketDto, ObserverInterface client)
        {
            Match match= ticketDto.matchNoTicketsDTO.match;
            Console.WriteLine("Match: " + match);
            int numberOfSeats = ticketDto.NumberOfSeats;
            Console.WriteLine("Number of seats: " + numberOfSeats);
            string nameOfClient = ticketDto.ClientName;
            Console.WriteLine("Name of client: " + nameOfClient);
            Console.WriteLine("Am ajuns aici");
            if (matchDBRepository.CheckAvailableSeats(match, numberOfSeats) > 0)
            {
                
                Console.WriteLine(matchDBRepository.CheckAvailableSeats(match, numberOfSeats) + "jwekfjwe");
                if (match.SoldSeats - numberOfSeats < 0)
                {
                    throw new Exception("Not enough seats!");
                }
                int newNumberOfSeats = match.SoldSeats - numberOfSeats;
                Console.WriteLine("New number of seats: " + newNumberOfSeats);
                int id = match.GetId();
                updateMatch(id, match.TeamA, match.TeamA, match.TicketPrice, newNumberOfSeats);
                //generate a random id for the ticket
                // Random rnd = new Random();
                // int id = rnd.Next(1, 100000);
                addTicket(id, nameOfClient, numberOfSeats);
                Console.WriteLine("Ticket with id "+ id + "added-------------------------------------------");
                // ticketDBRepository.add(ticket);
                notifyClients();
            }
            else
            {
                throw new Exception("Not enough seats!");
            }
        }
        
        public void addTicket(int id, string clientName, int numberOfSeats)
        {
            Ticket ticket = new Ticket(id, clientName, numberOfSeats);
            ticketDBRepository.add(ticket);
        }
        
        public void addUser(int id, string username, string password)
        {
            User user = new User(id, username, password);
            userDBRepository.add(user);
        }
        
        public void addMatch(int id, string team1, string team2, double ticketPrice, int soldSeats)
        {
            Match match = new Match(id, team1, team2, ticketPrice, soldSeats);
            matchDBRepository.add(match);
        }
        
        public IEnumerable<Ticket> getAllTickets()
        {
            return ticketDBRepository.findAll();
        }
        
        public IEnumerable<User> getAllUsers()
        {
            return userDBRepository.findAll();
        }
        
        public IEnumerable<Match> getAllMatches(ObserverInterface client)
        {
            return matchDBRepository.findAll();
        }
        
        public void updateMatch(int id, string team1, string team2, double ticketPrice, int soldSeats)
        {
            Match match = new Match(id, team1, team2, ticketPrice, soldSeats);
            Console.WriteLine("The new Match: " + match + "-----------------------------------------------");
            matchDBRepository.update(match);
        }
        
        public Ticket findOneTicket(int id)
        {
            return ticketDBRepository.findOne(id);
        }
        
        public User findOneUser(int id)
        {
            return userDBRepository.findOne(id);
        }
        
        public Match findOneMatch(int id)
        {
            return matchDBRepository.findOne(id);
        }
        
        public int CheckifUserExists(UserDTO userDTO)
        {
            return userDBRepository.CheckifUserExists(userDTO.User, userDTO.Pass);
        }

        public int CheckAvailableSeats(MatchNoTicketsDTO matchNoTicketsDTO)
        {
            return matchDBRepository.CheckAvailableSeats(matchNoTicketsDTO.match, matchNoTicketsDTO.NoTickets);
        }


        public void notifyClients()
        {
            foreach (ObserverInterface client in loggedClients.Values)
            {
                client.updateTickets();
                client.updateMatches();
            }
        }
    }
}