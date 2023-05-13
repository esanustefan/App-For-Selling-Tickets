using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using ConcursBaschet.domain;
using log4net;


namespace ConcursBaschet.repo
{
    public class MatchDBRepo : MatchRepository
    {
        private static readonly ILog log = LogManager.GetLogger("MatchDbRepository");

        private string connectionString;

        public MatchDBRepo(string connectionString)
        {
            log.Info("Creating MatchDbRepository");
            this.connectionString = connectionString;
        }


        public Match findOne(int id)
        {
            log.InfoFormat("Entering findOne with value {0}", id);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("select * from Matches where id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int idV = reader.GetInt32(0);
                        String teamA = reader.GetString(1);
                        String teamB = reader.GetString(2);
                        double ticketPrice = reader.GetDouble(3);
                        int soldSeats = reader.GetInt32(4);
                        Match match = new Match(idV, teamA, teamB, ticketPrice, soldSeats);
                        reader.Close();

                        return match;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
                log.InfoFormat("Exiting findOne with value {0}", null);
                return null;
            }
        }

        public IEnumerable<Match> findAll()
        {
            log.InfoFormat("Entering findAll");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                List<Match> matches = new List<Match>();
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("select * from Matches", connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        String teamA = reader.GetString(1);
                        String teamB = reader.GetString(2);
                        double ticketPrice = reader.GetDouble(3);
                        int soldSeats = reader.GetInt32(4);
                        Match match = new Match(id, teamA, teamB, ticketPrice, soldSeats);
                        matches.Add(match);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
                log.InfoFormat("Exiting findAll with value {0}", matches);
                return matches;
            }
        }

        public Match add(Match entity)
        {
            log.InfoFormat("Entering add with value {0}", entity);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("insert into Matches (id, teamA, teamB, ticketPrice, soldSeats) values (@id, @teamA, @teamB, @ticketPrice, @soldSeats)", connection);
                    command.Parameters.AddWithValue("@id", entity.GetId());
                    command.Parameters.AddWithValue("@teamA", entity.TeamA);
                    command.Parameters.AddWithValue("@teamB", entity.TeamB);
                    command.Parameters.AddWithValue("@ticketPrice", entity.TicketPrice);
                    command.Parameters.AddWithValue("@soldSeats", entity.SoldSeats);
                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        log.InfoFormat("Exiting add with value {0}", null);
                        return null;
                    }
                    else
                    {
                        Match match = new Match(entity.GetId(), entity.TeamA, entity.TeamB, entity.TicketPrice, entity.SoldSeats);
                        log.InfoFormat("Exiting add with value {0}", match);
                        return match;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting add with value {0}", null);
            return null;
        }

        public Match delete(int id)
        {
            log.InfoFormat("Entering delete with value {0}", id);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("delete from Matches where id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        log.InfoFormat("Exiting delete with value {0}", null);
                        return null;
                    }
                    else
                    {
                        Match match = new Match(id, "", "", 0, 0);
                        log.InfoFormat("Exiting delete with value {0}", match);
                        return match;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting delete with value {0}", null);
            return null;
        }

        public void update(Match entity)
        {
            log.InfoFormat("Entering update with value {0}", entity);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("update Matches set teamA = @teamA, teamB = @teamB, ticketPrice = @ticketPrice, soldSeats = @soldSeats where id = @id", connection);
                    command.Parameters.AddWithValue("@id", entity.GetId());
                    command.Parameters.AddWithValue("@teamA", entity.TeamA);
                    command.Parameters.AddWithValue("@teamB", entity.TeamB);
                    command.Parameters.AddWithValue("@ticketPrice", entity.TicketPrice);
                    command.Parameters.AddWithValue("@soldSeats", entity.SoldSeats);
                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        log.InfoFormat("Exiting update with value {0}", null);
                        Console.WriteLine("No match updated!");
                        return;
                    }

                    log.InfoFormat("Exiting update with value {0}", entity);
                    Console.WriteLine("Match updated!");
                    return;
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting update with value {0}", null);
        }

        public int CheckAvailableSeats(Match match, int numberOfSeats)
        {
            log.InfoFormat("Entering CheckAvailableSeats with value {0}", match);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("select soldSeats from Matches where id = @id", connection);
                    command.Parameters.AddWithValue("@id", match.GetId());
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int soldSeats = reader.GetInt32(0);
                        reader.Close();
                        if(numberOfSeats == 0 || numberOfSeats > 0)
                        {
                            log.InfoFormat("Exiting CheckAvailableSeats with value {0}", 0);
                            return soldSeats;
                        }
                        else if (soldSeats - numberOfSeats < 0)
                        {
                            log.InfoFormat("Exiting CheckAvailableSeats with value {0}", 1);
                            return -1;
                        }
                        else
                        {
                            log.InfoFormat("Exiting CheckAvailableSeats with value {0}", 0);
                            return soldSeats;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    MessageBox.Show(e.Message);
                }
            }

            log.InfoFormat("Exiting CheckAvailableSeats with value {0}", 0);
            return 0;
        }
    }
}