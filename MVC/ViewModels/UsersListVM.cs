using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class UsersListVM
    {
        [Required]
        public int id { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Name")]
        public string displayName { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime? birthday { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string gender { get; set; }
        [Required]
        [Display(Name = "Rating")]
        public float rating { get; set; }
        public bool sentRequest { get; set; }

        public UsersListVM() { }
        public UsersListVM(UserDTO userDto) {
            id = userDto.Id;
            displayName = userDto.displayName;
            birthday = userDto.birthday;
            if (userDto.gender)
            {
                gender = "Male";
            }
            else {
                gender = "Female";
            }
            rating = userDto.rating;
        }
    }
}