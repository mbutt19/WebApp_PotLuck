using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Schedule Data Transfer Object
    /// </summary>
    public class ScheduleDTO
    {
        public string dayOfWeek { get; set; }
        public IList<ItemDTO> itemList { get; set; }
        public string scheduleId { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        public UserDTO user { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScheduleDTO()
        {
            ;
        }
    }
}
