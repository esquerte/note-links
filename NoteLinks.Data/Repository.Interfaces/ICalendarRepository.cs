using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Interfaces
{
    public interface ICalendarRepository : IRepository<Calendar>
    {
        //Author GetAuthorWithCourses(int id);
    }
}
