using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConcursBaschet.domain;
using ConsoleApplication1;
using Services;

namespace Networking.rpProtocols
{
    public class ClientRpcWorker : ObserverInterface
    {
        private ServiceInterface server;
        private TcpClient connection;
        private NetworkStream stream;
        private IFormatter formatter;
        private volatile bool connected;

        public ClientRpcWorker(ServiceInterface server, TcpClient connection)
        {
            this.server = server;
            this.connection = connection;
            try
            {
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static Response okResponse = new Response.Builder().type(ResponseType.OK).build();

        private object handleRequest(Request request)
        {
            if (request.type == RequestType.LOGIN)
            {
                
                UserDTO userDTO = (UserDTO)request.data;
                Console.WriteLine("Client RPC Login request ..."+userDTO);
                try
                {
                    User user = server.logIn(userDTO, this);
                    return new Response.Builder().type(ResponseType.OK).data(user).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }

            if (request.type == RequestType.LOGOUT)
            {
                User user = (User)request.data;
                try
                {
                    server.logOut(user, this);
                    connected = false;
                    return okResponse;
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }

            if (request.type == RequestType.GET_MECIURI)
            {
                try
                {
                    IEnumerable<Match> matches = server.getAllMatches(this);
                    return new Response.Builder().type(ResponseType.OK).data(matches).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }

            if (request.type == RequestType.GET_TICKETS)
            {
                try
                {
                    IEnumerable<Ticket> tickets = server.getAllTickets();
                    return new Response.Builder().type(ResponseType.OK).data(tickets).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }

            if (request.type == RequestType.BUY_TICKET)
            {
                TicketDTO ticketDto = (TicketDTO)request.data;
                Console.WriteLine(ticketDto);
                try
                {
                    server.buyTickets(ticketDto, this);
                    Console.WriteLine("Client RPC OK RESPONSE ...");
                    return okResponse;
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }

            if (request.type == RequestType.GET_USERS)
            {
                try
                {
                    IEnumerable<User> users = server.getAllUsers();
                    return new Response.Builder().type(ResponseType.OK).data(users).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
            }
            
            if (request.type == RequestType.CHECK_SEATS)
            {
                MatchNoTicketsDTO match = (MatchNoTicketsDTO)request.data;
                try
                {
                    int seats = server.CheckAvailableSeats(match);
                    return new Response.Builder().type(ResponseType.OK).data(seats).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }
                
            }
            if(request.type == RequestType.FIND_MECI)
            {
                int matchid = (int)request.data;
                try
                {
                    Match match = server.findOneMatch(matchid);
                    return new Response.Builder().type(ResponseType.OK).data(match).build();
                }
                catch (Exception e)
                {
                    return new Response.Builder().type(ResponseType.ERROR).data(e.Message).build();
                }

            }
            return null;
        }
        
        public virtual void run()
        {
            while (connected)
            {
                try
                {
                    object request = formatter.Deserialize(stream);
                    object response = handleRequest((Request)request);
                    if (response != null)
                    {
                        sendResponse((Response)response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            try
            {
                stream.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e);
            }
        }
        


        private void sendResponse(Response response)
        {
            lock (stream)
            {
                formatter.Serialize(stream, response);
                stream.Flush();
            }
        }

        public void updateTickets()
        {
            Response response = new Response.Builder().type(ResponseType.UPDATE_TICKETS).data(server.getAllTickets()).build();
            try
            {
                sendResponse(response);
            }
            catch (Exception e)
            {
                throw new Exception("Sending error: " + e);
            }
        }

        public void updateMatches()
        {
            Response response = new Response.Builder().type(ResponseType.UPDATE_MECIURI).data(server.getAllMatches(this)).build();
            try
            {
                sendResponse(response);
            }
            catch (Exception e)
            {
                throw new Exception("Sending error: " + e);
            }
        }
    }
}