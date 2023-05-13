using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using ConcursBaschet.domain;
using ConsoleApplication1;
using Services;

namespace Networking.rpProtocols
{
    public class ServiceRpcProxy : ServiceInterface
    {
        private string host;
        private int port;

        private ObserverInterface client;
        private NetworkStream stream;
        private IFormatter formatter;
        private TcpClient connection;

        private Queue<Response> responses;
        private volatile bool finished;
        private EventWaitHandle _waitHandle;
        
        public ServiceRpcProxy(string host, int port)
        {
            this.host = host;
            this.port = port;
            responses = new Queue<Response>();
        }
        
        private void initializeConnection()
        {
            try
            {
                connection = new TcpClient(host, port);
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                finished = false;
                _waitHandle = new AutoResetEvent(false);
                startReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private void startReader()
        {
            Thread tw = new Thread(run);
            tw.Start();
        }
        
        private bool isUpdate(Response response)
        {
            return response.type == ResponseType.UPDATE_TICKETS || response.type == ResponseType.UPDATE_MECIURI;
        }

        public virtual void run()
        {
            while (!finished)
            {
                try
                {
                    object response = (Response)formatter.Deserialize(stream);
                    if (isUpdate((Response)response))
                    {
                        handleUpdate((Response)response);
                    }
                    else
                    {
                        lock (responses)
                        {
                            responses.Enqueue((Response)response);
                        }
                        _waitHandle.Set();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reading error " + ex);
                }
            }
        }
        
        private void handleUpdate(Response response)
        {
            if (response.type == ResponseType.UPDATE_TICKETS)
            {
                try
                {
                    client.updateTickets();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            if (response.type == ResponseType.UPDATE_MECIURI)
            {
                try
                {
                    client.updateMatches();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        
        private Response readResponse()
        {
            Response response = null;
            try
            {
                _waitHandle.WaitOne();
                lock (responses)
                {
                    response = responses.Dequeue();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return response;
        }


        private void sendRequest(Request request)
        {
            try
            {
                formatter.Serialize(stream, request);
                stream.Flush();
            }
            catch (Exception e)
            {
                throw new Exception("Error sending object " + e);
            }
        }

        private void closeConnection()
        {
            finished = true;
            try
            {
                stream.Close();
                connection.Close();
                _waitHandle.Close();
                client = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public User getUser(UserDTO userDTO)
        {
            initializeConnection();
            int id = CheckifUserExists(userDTO);
            User user = findOneUser(id);
            Request request = new Request.Builder().type(RequestType.LOGIN).data(user).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                this.client = client;
                return (User)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return null;
        }

        public User logIn(UserDTO userDTO, ObserverInterface client)
        {
            
            initializeConnection();
            // int id = CheckifUserExists(userDTO);
            // User user = findOneUser(id);
            // Console.WriteLine("User: " + user);
            Request request = new Request.Builder().type(RequestType.LOGIN).data(userDTO).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                this.client = client;
                return (User)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return null;
        }

        public void logOut(User user, ObserverInterface client)
        {
            Request request = new Request.Builder().type(RequestType.LOGOUT).data(user).build();
            sendRequest(request);
            Response response = readResponse();
            closeConnection();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
            MessageBox.Show("Logout successful!");
        }

        public void buyTickets(TicketDTO ticketDto, ObserverInterface client)
        {
            Request request = new Request.Builder().type(RequestType.BUY_TICKET).data(ticketDto).build();
            Console.WriteLine("Request: " + request);
            sendRequest(request);
            Response response = readResponse();
            Console.WriteLine("Response: " + response +" ------------------");
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
            MessageBox.Show("Tickets bought successfully!");
        }


        public void addTicket(int id, string clientName, int numberOfSeats)
        {
            
            Ticket ticket = new Ticket(id, clientName, numberOfSeats);
            Request request = new Request.Builder().type(RequestType.ADD_TICKET).data(ticket).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
        }

        public void addUser(int id, string username, string password)
        {
            throw new NotImplementedException();
        }

        public void addMatch(int id, string team1, string team2, double ticketPrice, int soldSeats)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ticket> getAllTickets()
        {
            
            Request request = new Request.Builder().type(RequestType.GET_TICKETS).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
            return (IEnumerable<Ticket>)response.data;
        }

        public IEnumerable<User> getAllUsers()
        {
            
            Request request = new Request.Builder().type(RequestType.GET_USERS).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
            return (IEnumerable<User>)response.data;
        }

        public IEnumerable<Match> getAllMatches(ObserverInterface client)
        {
            
            Request request = new Request.Builder().type(RequestType.GET_MECIURI).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
            return (IEnumerable<Match>)response.data;
        }

        public void updateMatch(int id, string team1, string team2, double ticketPrice, int soldSeats)
        {
            
            Match match = new Match(id, team1, team2, ticketPrice, soldSeats);
            Request request = new Request.Builder().type(RequestType.UPDATE_MECI).data(match).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.ERROR)
            {
                string err = response.data.ToString();
                throw new Exception(err);
            }
        }

        public Ticket findOneTicket(int id)
        {
            throw new NotImplementedException();
        }

        public User findOneUser(int id)
        {
            
            Request request = new Request.Builder().type(RequestType.FIND_USER).data(id.ToString()).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                return (User)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return null;
        }

        public Match findOneMatch(int id)
        {
            Request request = new Request.Builder().type(RequestType.FIND_MECI).data(id).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                return (Match)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return null;
        }

        public int CheckifUserExists(UserDTO user)
        {
            
            Request request = new Request.Builder().type(RequestType.FIND_USER).data(user).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                return (int)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return -1;
        }

        public int CheckAvailableSeats(MatchNoTicketsDTO matchNoTicketsDto)
        {
            Request request = new Request.Builder().type(RequestType.CHECK_SEATS).data(matchNoTicketsDto).build();
            sendRequest(request);
            Response response = readResponse();
            if (response.type == ResponseType.OK)
            {
                return (int)response.data;
            }
            if (response.type == ResponseType.ERROR)
            {
                closeConnection();
                throw new Exception((string)response.data);
            }
            return -1;
        }
    }
}