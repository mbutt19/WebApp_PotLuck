using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Page of Deliveries
    /// </summary>
    public class PageOfDeliveries
    {
        public List<DeliveryDTO> content { get; set; }
        public Pageable pageable { get; set; }
        public long totalPages { get; set; }
        public long totalElements { get; set; }
        public bool empty { get; set; }
        public bool first { get; set; }
        public bool last { get; set; }
    }
}
