using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Order Values Object
    /// </summary>
    public class OrderVO
    {
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public string deliveryType { get; set; }
        public string orderId { get; set; }
        public List<OrdersItemVO> ordersItems { get; set; }
    }
}
