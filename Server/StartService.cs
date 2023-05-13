using System;
using System.Configuration;
using System.Net.Sockets;
using System.Threading;
using ConcursBaschet.repo;
using Networking;
using Networking.rpProtocols;
using Services;

namespace Server
{
    public class StartService
    {
        public static void Main(string[] args)
        {
            MatchDBRepo matchDBRepository = new MatchDBRepo(GetConnectionStringByName("identifier"));
            TicketDBRepo ticketDBRepository = new TicketDBRepo(GetConnectionStringByName("identifier"));
            UserDBRepository userDBRepository = new UserDBRepository(GetConnectionStringByName("identifier"));
            Service serviceImpl = new Service(matchDBRepository, ticketDBRepository, userDBRepository);
            
            SerialServer server = new SerialServer("127.0.0.1", 55557, serviceImpl);
            server.Start();
        }
        
        public class SerialServer : ConcurrentAbstractServer
        {
            private ServiceInterface server;
            private ClientRpcWorker worker;
            
            public SerialServer(string host, int port, ServiceInterface server) : base(host, port)
            {
                this.server = server;
                Console.WriteLine("SerialServer...");
            }
            
            protected override Thread createWorker(TcpClient client)
            {
                worker = new ClientRpcWorker(server, client);
                return new Thread(new ThreadStart(worker.run));
            }
        }
        
        static string GetConnectionStringByName(string name)
        {
            // Assume failure.
            string returnValue = null;

            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string.
            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }
    }
}