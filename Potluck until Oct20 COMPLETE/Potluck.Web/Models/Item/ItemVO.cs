using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Item Values Object
    /// </summary>
    public class ItemVO
    {
        public string itemId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string picture { get; set; }
        public string calories { get; set; }
        public bool soldAlone { get; set; }
        public decimal price { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public bool enable { get; set; }
        public string categoryId { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ItemVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemResponse">ItemDTO</param>
        public ItemVO(ItemDTO itemResponse)
        {
            calories = itemResponse.calories;
            categoryId = itemResponse.category.id;
            dateTimeCreated = itemResponse.dateTimeCreated;
            dateTimeModified = itemResponse.dateTimeModified;
            description = itemResponse.description;
            enable = itemResponse.enable;
            itemId = itemResponse.itemId;
            name = itemResponse.name;
            picture = itemResponse.imageUrl;
            price = itemResponse.price;
            soldAlone = itemResponse.soldAlone;
        }
    }
}
