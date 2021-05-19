using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class ProfileVM
    {
        public int id { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name = "Username")]
        public string username { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Display name")]
        public string displayName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string email { get; set; }
        [StringLength(15)]
        [Display(Name = "Phone number")]
        public string phone_number { get; set; }
        [StringLength(500)]
        [Display(Name = "Bio")]
        [DataType(DataType.MultilineText)]
        public string bio { get; set; }
        [StringLength(200)]
        [Display(Name = "Social link")]
        public string socialLink { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public bool gender { get; set; }

        public ProfileVM() { }
        public ProfileVM(UserDTO userDto)
        {
            id = userDto.Id;
            username = userDto.username;
            password = userDto.password;
            email = userDto.email;
            displayName = userDto.displayName;
            phone_number = userDto.phone_number;
            bio = userDto.bio;
            socialLink = userDto.socialLink;
            gender = userDto.gender;
        }
    }
}