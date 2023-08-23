using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// User Values Object
    /// </summary>
    public class UserVO
    {
        public string picture { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public int coverageKmRadius { get; set; }
        public int amountStars { get; set; }
        public int reviews { get; set; }
        public string userId { get; set; }

        public UserVO()
        {
            ;
        }

        public UserVO(UserDTO user)
        {
            picture = user.imageUrl;
            name = user.name;
            email = user.email;
            telephone = user.telephone;
            coverageKmRadius = user.coverageKmRadius;
            amountStars = user.amountStars;
            reviews = user.reviews;
            userId = user.userId;
        }
    }
}
