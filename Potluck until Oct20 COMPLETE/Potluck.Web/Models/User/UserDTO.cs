using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// User Data Transfer Object
    /// </summary>
    public class UserDTO
    {
        public string token { get; set; }
        public IList<AddressDTO> addresses { get; set; }
        public int amountStars { get; set; }
        public int coverageKmRadius { get; set; }
        public string email { get; set; }
        public bool emailVerified { get; set; }
        public List<SubItemDTO> favorites { get; set; }
        public string imageUrl { get; set; }
        public string name { get; set; }
        public string provider { get; set; }
        public int reviews { get; set; }
        public IList<Role> roles { get; set; }
        public string telephone { get; set; }
        public string userId { get; set; }
    }
}
