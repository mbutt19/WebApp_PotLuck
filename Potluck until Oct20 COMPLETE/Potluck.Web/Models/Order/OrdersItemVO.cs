using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// OrderItem Values Object
    /// </summary>
    public class OrdersItemVO
    {
        public string itemId { get; set; }
        public int quantity { get; set; }
        public IList<OrdersItemsCustomVO> ordersItemsCustoms { get; set; }
        public string ordersItemsId { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrdersItemVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemResponse">ItemDTO</param>
        /// <param name="quantity">int</param>
        public OrdersItemVO(ItemDTO itemResponse, int quantity)
        {
            this.itemId = itemResponse.itemId;
            this.quantity = quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subItem">SubItemDTO</param>
        /// <param name="quantity">int</param>
        public OrdersItemVO(SubItemDTO subItem, int quantity)
        {
            this.itemId = subItem.itemId;
            this.quantity = quantity;
        }
    }
}
