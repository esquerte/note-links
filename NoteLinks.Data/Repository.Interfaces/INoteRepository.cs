using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<List<Note>> GetNotesAsync(Expression<Func<Note, bool>> predicate, Filter[] filters, PageInfo pageInfo);
        Task<int> GetNotesCountAsync(Expression<Func<Note, bool>> predicate);
    }
}
