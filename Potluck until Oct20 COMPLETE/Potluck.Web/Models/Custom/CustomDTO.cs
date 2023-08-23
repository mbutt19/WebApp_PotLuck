using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Custom Data Transfer Object
    /// </summary>
    public class CustomDTO
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomDTO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custom">CustomDTO</param>
        public CustomDTO(CustomDTO custom)
        {
            customId = custom.customId;
            customTitle = custom.customTitle;
            customType = custom.customType;
            dateTimeCreated = custom.dateTimeCreated;
            dateTimeModified = custom.dateTimeModified;
            subItems = custom.subItems;
        }

        public string customId { get; set; }
        public string customTitle { get; set; }
        public string customType { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public List<SubItemDTO> subItems { get; set; }
    }
}
