using System;
using ConcursBaschet.domain;
using ConcursBaschet.repo;

namespace ConcursBaschet.repo
{
    public interface IUserRepository : IRepository<int, User>
    {
        int CheckifUserExists(string username, string password);

    }
}