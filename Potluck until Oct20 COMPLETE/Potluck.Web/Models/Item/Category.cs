using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Category()
        {
            ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">String</param>
        /// <param name="name">String</param>
        public Category(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
