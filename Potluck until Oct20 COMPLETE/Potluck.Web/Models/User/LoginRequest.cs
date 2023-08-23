using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
