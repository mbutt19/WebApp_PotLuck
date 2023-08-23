using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Address Data Transfer Object
    /// </summary>
    public class AddressDTO
    {
        public string addressId { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressName { get; set; }
        public string city { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool mainAddress { get; set; }
        public string postalCode { get; set; }
        public StateCountry stateCountry { get; set; }
    }
}
