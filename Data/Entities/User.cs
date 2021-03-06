using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(16)]
        public string username { get; set; }
        [Required]
        [StringLength(16)]
        public string password { get; set; }
        [Required]
        [StringLength(16)]
        public string displayName { get; set; }
        [Required]
        [StringLength(50)]
        public string email { get; set; }
        [StringLength(15)]
        public string phone_number { get; set; }
        [StringLength(500)]
        public string bio { get; set; }
        [StringLength(200)]
        public string socialLink { get; set; }
        public float rating { get; set; }
        public DateTime? birthday { get; set; }
        [Required]
        public bool gender { get; set; }
    }
}
