using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Schedule Values Object
    /// </summary>
    public class ScheduleVO
    {
        public string dayOfWeek { get; set; }
        public List<SubItemVO> itemList { get; set; }
        public string scheduleId { get; set; }
        public TimeVO timeFrom { get; set; }
        public TimeVO timeTo { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScheduleVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule">ScheduleCreate</param>
        public ScheduleVO(ScheduleCreate schedule)
        {
            this.dayOfWeek = schedule.dayOfWeek;
            this.scheduleId = schedule.scheduleId;
            this.timeFrom = new TimeVO(schedule.timeFrom);
            this.timeTo = new TimeVO(schedule.timeTo);

            this.itemList = new List<SubItemVO>();
            SubItemVO subItem = new SubItemVO(schedule.itemId);
            this.itemList.Add(subItem);
        }

        public ScheduleVO(ScheduleDTO schedule)
        {
            this.dayOfWeek = schedule.dayOfWeek;
            this.scheduleId = schedule.scheduleId;
            this.timeFrom = new TimeVO(schedule.timeFrom);
            this.timeTo = new TimeVO(schedule.timeTo);

            this.itemList = new List<SubItemVO>();
            if (schedule.itemList != null && schedule.itemList.Count > 0)
            {
                foreach (ItemDTO item in schedule.itemList)
                {
                    SubItemVO subItem = new SubItemVO(item.itemId);
                    this.itemList.Add(subItem);
                }
            }
        }
    }
}
