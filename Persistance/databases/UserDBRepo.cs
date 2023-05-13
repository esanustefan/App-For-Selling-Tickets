using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using ConcursBaschet.domain;
using log4net;

namespace ConcursBaschet.repo
{
	public class UserDBRepository : IUserRepository
	{
		private static readonly ILog log = LogManager.GetLogger("UserDbRepository");

		private string connectionString;

		public UserDBRepository(string connectionString)
		{
			log.Info("Creating UserDbRepository ");
			this.connectionString = connectionString;
		}


		public User findOne(int id)
		{
			log.InfoFormat("Entering findOne with value {0}", id);
			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				try
				{
					connection.Open();
					SQLiteCommand command = new SQLiteCommand("select * from Users where id = @id", connection);
					command.Parameters.AddWithValue("@id", id);
					SQLiteDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						int idV = reader.GetInt32(0);
						string username = reader.GetString(1);
						string password = reader.GetString(2);
						User user = new User(idV, username, password);
						reader.Close();

						return user;
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

		public IEnumerable<User> findAll()
		{
			log.InfoFormat("Entering findAll");
			List<User> users = new List<User>();
			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				try
				{
					connection.Open();
					SQLiteCommand command = new SQLiteCommand("select * from Users", connection);
					SQLiteDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						string username = reader.GetString(1);
						string password = reader.GetString(2);
						User user = new User(id, username, password);
						users.Add(user);
					}
				}
				catch (Exception e)
				{
					log.Error(e.Message);
					MessageBox.Show(e.Message);
				}
			}
			log.InfoFormat("Exiting findAll with value {0}", users);
			return users;
		}

		public User add(User entity)
		{
			log.InfoFormat("Entering add with value {0}", entity);
			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				try
				{
					connection.Open();
					SQLiteCommand command = new SQLiteCommand("insert into Users (id, username, password) values (@id, @username, @password)", connection);
					command.Parameters.AddWithValue("@id", entity.GetId());
					command.Parameters.AddWithValue("@username", entity.Username);
					command.Parameters.AddWithValue("@password", entity.Password);
					int result = command.ExecuteNonQuery();
					if (result == 1)
					{
						log.InfoFormat("Exiting add with value {0}", entity);
						return entity;
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

		public User delete(int id)
		{
			throw new NotImplementedException();
		}

		public void update(User entity)
		{
			throw new NotImplementedException();
		}

		public int CheckifUserExists(string username, string password)
		{
			log.InfoFormat("Entering CheckifUserExists with value {0}", username);
			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				try
				{
					connection.Open();
					SQLiteCommand command = new SQLiteCommand("select * from Users where username = @username and password = @password", connection);
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);
					SQLiteDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						string usernameV = reader.GetString(1);
						string passwordV = reader.GetString(2);
						User user = new User(id, usernameV, passwordV);
						reader.Close();
						log.InfoFormat("Exiting CheckifUserExists with value {0}", true);
						return id;
					}
				}
				catch (Exception e)
				{
					log.Error(e.Message);
					MessageBox.Show(e.Message);
				}
			}
			log.InfoFormat("Exiting CheckifUserExists with value {0}", false);
			return -1;
		}
	}
}
