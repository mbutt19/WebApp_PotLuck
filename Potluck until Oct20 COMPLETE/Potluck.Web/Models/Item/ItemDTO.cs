using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Models
{
    /// <summary>
    /// Item Data Transfer Object
    /// </summary>
    public class ItemDTO
    {
        public string itemId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string calories { get; set; }
        public bool soldAlone { get; set; }
        public decimal price { get; set; }
        public string dateTimeCreated { get; set; }
        public string dateTimeModified { get; set; }
        public bool enable { get; set; }
        public UserDTO user { get; set; }
        public Category category { get; set; }
        public List<CustomDTO> customs { get; set; }
    }
}

