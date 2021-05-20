using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Event : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string title { get; set; }
        [Required]
        [StringLength(2500)]
        public string description { get; set; }
        [Required]
        [StringLength(200)]
        public string location { get; set; }
        [Required]
        public int host_id { get; set; }
        public int likes { get; set; }
        public DateTime createdOn { get; set; }
        [Required]
        public DateTime begins { get; set; }
        [Required]
        public DateTime ends { get; set; }
        [Required]
        public bool ended { get; set; }
        public int participants { get; set; }
    }
}
