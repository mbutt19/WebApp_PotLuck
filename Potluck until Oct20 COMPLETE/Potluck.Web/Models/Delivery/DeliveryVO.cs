using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Delivery Values Object
    /// </summary>
    public class DeliveryVO
    {
        public string coverageKmRadius { get; set; }
        public string deliveryId { get; set; }
        public string feePercentage { get; set; }
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">String</param>
        /// <param name="deliveryId">String</param>
        /// <param name="feePercentage">String</param>
        /// <param name="coverageRadius">String</param>
        public DeliveryVO(string name, string deliveryId, string feePercentage, string coverageRadius )
        {
            this.coverageKmRadius = coverageRadius;
            this.deliveryId = deliveryId;
            this.feePercentage = feePercentage;
            this.name = name;
        }
    }
}
