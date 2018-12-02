using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Models
{
    public class PageInfo
    {
        private int _maxSize = 50; 
        private int _pageSize = 10;

        public int PageIndex { get; set; } = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value > _maxSize ? _maxSize : value;
            }
        }

        public string OrderBy { get; set; }
        public bool Desc { get; set; }
    }
}
