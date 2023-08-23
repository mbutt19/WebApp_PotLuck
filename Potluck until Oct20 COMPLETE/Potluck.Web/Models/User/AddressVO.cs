using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Address Values Object
    /// </summary>
    public class AddressVO
    {
        public string addressId { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressName { get; set; }
        public string city { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string postalCode { get; set; }
        public string stateCountryId { get; set; }
        public bool mainAddress { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddressVO()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">AddressDTO</param>
        public AddressVO(AddressDTO address)
        {
            addressId = address.addressId;
            addressLine1 = address.addressLine1;
            addressLine2 = address.addressLine2;
            addressName = address.addressName;
            city = address.city;
            latitude = address.latitude;
            longitude = address.longitude;
            postalCode = address.postalCode;
            stateCountryId = "1";
            mainAddress = address.mainAddress;
        }
    }
}
