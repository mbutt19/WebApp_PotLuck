using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Paging object
    /// Attached to "Pages" of objects
    /// specifies details about the page requeted
    /// </summary>
    public class Pageable
    {
        public Dictionary<string,bool> sort { get; set; }
        public long pageNumber { get; set; }
        public long pageSize { get; set; }
        public long offset { get; set; }
        public bool unpaged { get; set; }
        public bool paged { get; set; }
    }
}
