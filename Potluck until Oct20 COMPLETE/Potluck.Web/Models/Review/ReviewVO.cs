using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Review Values Object
    /// </summary>
    public class ReviewVO
    {
        public string message { get; set; }
        public string reviewId { get; set; }
        public string sellerEmail { get; set; }
        public int stars { get; set; }
        public string subject { get; set; }
    }
}
