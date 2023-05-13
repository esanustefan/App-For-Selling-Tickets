using System;


namespace ConcursBaschet.domain
{
    [Serializable]
    public class User : Entity<int>
    {
        private string _username;
        private string _password;

        public User(int id, string username, string password) : base(id)
        {
            this._username = username;
            this._password = password;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public override string ToString()
        {
            return $"User{{ username='{_username}', password='{_password}' }}";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        protected bool Equals(User other)
        {
            return base.Equals(other) && _username == other._username && _password == other._password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _username, _password);
        }
    }
}