using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Order Item Data Transfer Object
    /// </summary>
    public class OrdersItemDTO
    {
        public ItemVO item { get; set; }
        public IList<OrdersItemsCustomDTO> ordersItemsCustoms { get; set; }
        public string ordersItemsId { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        
    }
}
