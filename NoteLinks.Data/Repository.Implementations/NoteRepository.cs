using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NoteLinks.Data.Helpers;

namespace NoteLinks.Data.Repository.Implementations
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        MainDataContext MainContext => Context as MainDataContext;

        public NoteRepository(DbContext context) : base(context)
        {
        }

        public Task<List<Note>> GetNotesAsync(Expression<Func<Note, bool>> predicate, Filter[] filters, PageInfo pageInfo)
        {
            var query = MainContext.Notes.Where(predicate);

            FilterHelper.FilterNotes(ref query, filters);
            PagingHelper.PagingNotes(ref query, pageInfo);

            return query.ToListAsync();
        }

        public Task<int> GetNotesCountAsync(Expression<Func<Note, bool>> predicate)
        {
            return MainContext.Notes.Where(predicate).CountAsync();
        }



    }
}
