using NoteLinks.Data.Context;

using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDataContext _context;

        public UnitOfWork(MainDataContext context)
        {
            _context = context;
            Calendars = new CalendarRepository(_context);
            Notes = new NoteRepository(_context);
        }

        public ICalendarRepository Calendars { get; private set; }
        public INoteRepository Notes { get; private set; }

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
