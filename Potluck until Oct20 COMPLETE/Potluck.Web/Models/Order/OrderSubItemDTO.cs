using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Order Subitem Data Transfer Object
    /// </summary>
    public class OrderSubItemDTO
    {
        public string calories { get; set; }
        public Category category { get; set; }
        public bool enable { get; set; }
        public string itemId { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
    }
}
