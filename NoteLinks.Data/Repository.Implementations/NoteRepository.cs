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

namespace NoteLinks.Data.Repository.Implementations
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        MainDataContext MainContext => Context as MainDataContext;

        public NoteRepository(DbContext context) : base(context)
        {
        }

        public Task<List<Note>> GetNotesAsync(Expression<Func<Note, bool>> predicate, PageInfo pageInfo)
        {
            var query = MainContext.Notes.Where(predicate);

            switch (pageInfo.OrderBy) 
            {
                case "Name":
                    query = pageInfo.Desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                    break;
                case "FromDate":
                    query = pageInfo.Desc ? query.OrderByDescending(x => x.FromDate) : query.OrderBy(x => x.FromDate);
                    break;
                case "ToDate":
                    query = pageInfo.Desc ? query.OrderByDescending(x => x.ToDate) : query.OrderBy(x => x.ToDate);
                    break;
                case "Text":
                    query = pageInfo.Desc ? query.OrderByDescending(x => x.Text) : query.OrderBy(x => x.Text);
                    break;
                default:
                    query = pageInfo.Desc ? query.OrderByDescending(x => x.FromDate) : query.OrderBy(x => x.FromDate);
                    break;
            }

            return query
                .Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize)
                .Take(pageInfo.PageSize)
                .ToListAsync();
        }

        public Task<int> GetNotesCountAsync(Expression<Func<Note, bool>> predicate)
        {
            return MainContext.Notes.Where(predicate).CountAsync();
        }

    }
}
