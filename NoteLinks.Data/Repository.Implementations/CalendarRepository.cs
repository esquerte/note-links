using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class CalendarRepository : Repository<Calendar>, ICalendarRepository
    {
        MainContext db => Context as MainContext;

        public CalendarRepository(DbContext context) : base(context)
        {
        }

        public Task<Calendar> GetCalendarWithCreator(Expression<Func<Calendar, bool>> predicate)
        {
            return db.Calendars
                .AsNoTracking()
                .Include(x => x.Creator)
                .SingleOrDefaultAsync(predicate);                
        }
    }
}
