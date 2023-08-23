using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// SubItem Data Transfer Object
    /// </summary>
    public class SubItemDTO
    {
        public string calories { get; set; }
        public Category category { get; set; }
        public bool enable { get; set; }
        public string itemId { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        SubItemDTO()
            {
                ;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemResponse">ItemDTO</param>
        SubItemDTO(ItemDTO itemResponse)
        {
            calories = itemResponse.calories;
            enable = itemResponse.enable;
            itemId = itemResponse.itemId;
            name = itemResponse.name;
            price = itemResponse.price;
        }


        
    }
}
