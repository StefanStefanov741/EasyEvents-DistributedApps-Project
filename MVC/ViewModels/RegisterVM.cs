using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVC.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [StringLength(16)]
        [Display(Name ="Username")]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
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
        [Required]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? birthday { get; set; }

        [Required]
        [Display(Name = "Male")]
        public bool gender { get; set; }

        public RegisterVM() {}
        public RegisterVM(UserDTO userDto) {

            username = userDto.username;
            password = userDto.password;
            displayName = userDto.displayName;
            email = userDto.email;
        }
    }
}