using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Repository.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        //IEnumerable<Course> GetTopSellingCourses(int count);
        //IEnumerable<Course> GetCoursesWithAuthors(int pageIndex, int pageSize);
    }
}
