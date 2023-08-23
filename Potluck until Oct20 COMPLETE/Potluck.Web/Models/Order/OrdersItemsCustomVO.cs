using System.Collections.Generic;

namespace Potluck.Web.Models
{
    /// <summary>
    /// OrderItemsCustom Values Object
    /// </summary>
    public class OrdersItemsCustomVO
    {
        public string customId { get; set; }
        public string ordersItemsCustomId { get; set; }
        public IList<OrderSubItemVO> subitems { get; set; }

    }
}