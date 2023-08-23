using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Delivery Data Transfer Object
    /// </summary>
    public class DeliveryDTO
    {
        public string coverageKmRadius { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public string deliveryId { get; set; }
        public string feePercentage { get; set; }
        public string name { get; set; }
        public UserDTO user { get; set; }
    }
}
