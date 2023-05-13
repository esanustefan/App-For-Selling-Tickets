using System.Collections.Generic;
using System;
using ConcursBaschet.domain;

public class RepositoryException:ApplicationException{
    public RepositoryException() { }
    public RepositoryException(String mess) : base(mess){}
    public RepositoryException(String mess, Exception e) : base(mess, e) { }
}

namespace ConcursBaschet.repo
{
    public interface IRepository<TId, TE> where TE : Entity<TId>
    {
        TE findOne(TId id);

        IEnumerable<TE> findAll();


        TE add(TE entity);


        TE delete(TId id);

        void update(TE entity);
    }
}