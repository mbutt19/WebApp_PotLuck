using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Time Values Object
    /// </summary>
    public class TimeVO
    {
        public string hours { get; set; }
        public string minutes { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        TimeVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">String</param>
        public TimeVO(string time)
        {
            string[] timeArray = time.Split(":");
            this.hours = timeArray[0];
            this.minutes = timeArray[1];
        }
    }
}
