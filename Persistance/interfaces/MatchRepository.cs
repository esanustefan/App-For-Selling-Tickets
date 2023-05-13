using System;
using ConcursBaschet.domain;

namespace ConcursBaschet.repo
{
    public interface MatchRepository : IRepository<int, Match>
    {
        int CheckAvailableSeats(Match match, int numberOfSeats);
    }
}