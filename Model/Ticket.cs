using System;


namespace ConcursBaschet.domain
{
    [Serializable]
    public class Ticket : Entity<int>
    {
        private string _clientName;
        private int _numberOfSeats;

        public Ticket(int id, string clientName, int numberOfSeats) : base(id)
        {
            this._clientName = clientName;
            this._numberOfSeats = numberOfSeats;
        }

        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; }
        }

        public int NumberOfSeats
        {
            get { return _numberOfSeats; }
            set { _numberOfSeats = value; }
        }

        public override string ToString()
        {
            return "Ticket{" +
                   "clientName='" + _clientName + '\'' +
                   ", numberOfSeats=" + _numberOfSeats +
                   '}';
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        protected bool Equals(Ticket other)
        {
            return base.Equals(other) && _clientName == other._clientName && _numberOfSeats == other._numberOfSeats;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _clientName, _numberOfSeats);
        }
    }
}