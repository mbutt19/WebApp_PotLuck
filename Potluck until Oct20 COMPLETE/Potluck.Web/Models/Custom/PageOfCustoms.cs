using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Page of Customs
    /// </summary>
    public class PageOfCustoms
    {
        /// <summary>
        /// 
        /// </summary>
        public PageOfCustoms()
        {
            content = new List<CustomDTO>();
        }
        public List<CustomDTO> content { get; set; }
        public Pageable pageable { get; set; }
        public long totalPages { get; set; }
        public long totalElements { get; set; }
        public bool first { get; set; }
        public bool last { get; set; }
    }
}
