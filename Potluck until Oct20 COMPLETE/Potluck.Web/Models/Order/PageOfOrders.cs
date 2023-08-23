using System.Collections.Generic;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Page of Orders
    /// </summary>
    public class PageOfOrders
    {
        public List<OrderDTO> content { get; set; }
        public Pageable pageable { get; set; }
        public long totalPages { get; set; }
        public long totalElements { get; set; }
        public bool empty { get; set; }
        public bool first { get; set; }
        public bool last { get; set; }
    }
}
