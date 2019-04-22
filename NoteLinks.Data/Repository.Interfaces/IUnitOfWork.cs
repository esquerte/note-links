using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICalendarRepository Calendars { get; }
        INoteRepository Notes { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
