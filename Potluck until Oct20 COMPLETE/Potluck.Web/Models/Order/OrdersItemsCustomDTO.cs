using System.Collections.Generic;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Orer Item Csutom Data Transfer Object
    /// </summary>
    public class OrdersItemsCustomDTO
    {
        public string ordersItemsCustomId { get; set; }
        public IList<OrderSubItemDTO> subitems { get; set; }

    }
}