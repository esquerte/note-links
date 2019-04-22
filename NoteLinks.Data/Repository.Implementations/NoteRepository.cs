using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Extensions;
using NoteLinks.Data.Models;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        MainContext db => Context as MainContext;

        public NoteRepository(DbContext context) : base(context)
        {
        }

        public Task<List<Note>> GetNotesAsync(Expression<Func<Note, bool>> predicate, Filter[] filters, PageInfo pageInfo)
        {
            var query = db.Notes
                .AsNoTracking()
                .Where(predicate)
                .Filter(filters)
                .Paginate(pageInfo);

            return query.ToListAsync();
        }

        public Task<int> GetNotesCountAsync(Expression<Func<Note, bool>> predicate, Filter[] filters)
        {
            return db.Notes
                .AsNoTracking()
                .Where(predicate)
                .Filter(filters)
                .CountAsync();
        }

    }
}
