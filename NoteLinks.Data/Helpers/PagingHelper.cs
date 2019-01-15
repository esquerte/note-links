using NoteLinks.Data.Entities;
using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Helpers
{
    public static class PagingHelper
    {
        public static IQueryable<Note> PagingNotes(ref IQueryable<Note> query, PageInfo pageInfo)
        {
            if (pageInfo != null)
            {
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

                query = query.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
            }

            return query;
        }

    }
}
