using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using ConcursBaschet.domain;
using ConcursBaschet.repo;
using log4net;

namespace ConcursBaschet.repo
{
    public class TicketDBRepo : TicketRepository
    {
        private static readonly ILog log = LogManager.GetLogger("TicketDbRepository");

        private string connectionString;

        public TicketDBRepo(string connectionString)
        {
            log.Info("Creating TicketDbRepository ");
            this.connectionString = connectionString;
        }


        public Ticket findOne(int id)
        {
            log.InfoFormat("Entering findOne with value {0}", id);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("select * from Tickets where id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int idV = reader.GetInt32(0);
                        string clientName = reader.GetString(1);
                        int numberOfSeats = reader.GetInt32(2);
                        Ticket ticket = new Ticket(idV, clientName, numberOfSeats);
                        reader.Close();

                        return ticket;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting findOne with value {0}", null);

            return null;
        }

        public IEnumerable<Ticket> findAll()
        {
            log.InfoFormat("Entering findAll");
            IList<Ticket> tickets = new List<Ticket>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("select * from Tickets", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int idV = reader.GetInt32(0);
                        string clientName = reader.GetString(1);
                        int numberOfSeats = reader.GetInt32(2);
                        Ticket ticket = new Ticket(idV, clientName, numberOfSeats);
                        tickets.Add(ticket);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting findAll with value {0}", tickets);
            return tickets;
        }

        public Ticket add(Ticket entity)
        {
            log.InfoFormat("Entering save with value {0}", entity);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("insert into Tickets (id, clientName, numberOfSeats) values (@id, @clientName, @numberOfSeats)", connection);
                    command.Parameters.AddWithValue("@id", entity.GetId());
                    command.Parameters.AddWithValue("@clientName", entity.ClientName);
                    command.Parameters.AddWithValue("@numberOfSeats", entity.NumberOfSeats);
                    int result = command.ExecuteNonQuery();
                    if (result == 1)
                    {
                        log.InfoFormat("Exiting save with value {0}", entity);
                        return entity;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting save with value {0}", null);
            return null;
        }

        public Ticket delete(int id)
        {
            throw new NotImplementedException();
        }

        public void update(Ticket entity)
        {
            throw new NotImplementedException();
        }
    }
}