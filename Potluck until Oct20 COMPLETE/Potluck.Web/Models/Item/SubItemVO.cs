using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// SubItem Values Object
    /// </summary>
    public class SubItemVO
    {
        public string itemId { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SubItemVO()
        {
            ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subItem">String</param>
        public SubItemVO(SubItemDTO subItem)
        {
            this.itemId = subItem.itemId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId">String</param>
        public SubItemVO(string itemId)
        {
            this.itemId = itemId;
        }
    }
}
