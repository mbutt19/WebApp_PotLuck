using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class StateCountry
    {
        public string country { get; set; }
        public decimal deliveryFee { get; set; }
        public decimal serviceFee { get; set; }
        public string stateCountryId { get; set; }
        public string stateProvince { get; set; }
        public decimal tax { get; set; }
    }
}
