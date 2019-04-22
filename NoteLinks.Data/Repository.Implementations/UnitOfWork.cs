using Microsoft.AspNetCore.Identity;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainContext _context;
        //private readonly UserManager<User> _userManager;

        public UnitOfWork(MainContext context/*, UserManager<User> userManager*/)
        {
            _context = context;
            //_userManager = userManager;

            Calendars = new CalendarRepository(_context);
            Notes = new NoteRepository(_context);
            Users = new UserRepository(_context/*, _userManager*/);
        }

        public ICalendarRepository Calendars { get; private set; }
        public INoteRepository Notes { get; private set; }
        public IUserRepository Users { get; private set; }

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
