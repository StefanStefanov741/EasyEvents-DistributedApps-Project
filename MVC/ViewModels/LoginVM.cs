using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class LoginVM
    {
        [Required]
        [StringLength(16)]
        [Display(Name = "Username")]
        public string username { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Password")]
        public string password { get; set; }

        public LoginVM() { }
        public LoginVM(UserDTO userDto)
        {
            username = userDto.username;
            password = userDto.password;
        }
    }
}