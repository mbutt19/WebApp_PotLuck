using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Review Data Transfer Object
    /// </summary>
    public class ReviewDTO
    {
        public UserDTO buyer { get; set; }
        public int dateTimeCreated { get; set; }
        public int dateTimeModified { get; set; }
        public string message { get; set; }
        public string reviewId { get; set; }
        public UserDTO seller { get; set; }
        public int stars { get; set; }
        public string subject { get; set; }
    }
}
