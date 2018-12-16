using Microsoft.EntityFrameworkCore;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class CalendarRepository : Repository<Calendar>, ICalendarRepository
    {
        MainDataContext MainContext => Context as MainDataContext;

        public CalendarRepository(DbContext context) : base(context)
        {
        }        
    }
}
