using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Order Data Transfer Object
    /// </summary>
    public class OrderDTO
    {
        public UserDTO buyer { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public string deliveryType { get; set; }
        public string orderId { get; set; }
        public string orderStatus { get; set; }
        public IList<OrdersItemDTO> ordersItems { get; set; }
        public UserDTO seller { get; set; }
        public decimal serviceFee { get; set; }
        public decimal subTotal { get; set; }
        public decimal tax { get; set; }
        public decimal total { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrderDTO()
        {
            ;
        }
    }
}
