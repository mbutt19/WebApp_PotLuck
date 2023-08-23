using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ScheduleCreate
    {
        public string dayOfWeek { get; set; }
        public string itemId { get; set; }
        public string scheduleId { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScheduleCreate()
        {
            ;
        }
    }
}
