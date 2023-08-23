using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Object that represent the role a user possesses
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Constructs Role object from input parameters
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="authority">String</param>
        public Role(int id, string authority)
        {
            this.id = id;
            this.authority = authority;
        }
        public string authority { get; set; }
        public long id { get; set; }
    }
}
