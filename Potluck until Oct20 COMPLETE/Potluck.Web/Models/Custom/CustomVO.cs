using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomVO
    {
        public string customId { get; set; }
        public string customTitle { get; set; }
        public string customType { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public List<string> subItemIds { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custom">CustomDTO</param>
        public CustomVO(CustomDTO custom)
        {
            this.customId = custom.customId;
            this.customTitle = custom.customTitle;
            this.customType = custom.customType;
            this.dateTimeCreated = custom.dateTimeCreated;
            this.dateTimeModified = custom.dateTimeModified;
            this.subItemIds = new List<string>();
            foreach (SubItemDTO subItem in custom.subItems)
            {
                subItemIds.Add(subItem.itemId);
            }
        }
    }
}
