using ApplicationService.DTOs;
using System;
using System.ComponentModel.DataAnnotations;


namespace MVC.ViewModels
{
    public class UserDetailsVM
    {
        public int Id { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Name:")]
        public string displayName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Email:")]
        public string email { get; set; }
        [StringLength(15)]
        [Display(Name = "Phone:")]
        public string phone_number { get; set; }
        [StringLength(200)]
        [Display(Name = "Social link:")]
        public string socialLink { get; set; }
        [Display(Name = "Rating:")]
        public float rating { get; set; }
        [Display(Name = "Birthday:")]
        [DataType(DataType.Date)]
        public DateTime? birthday { get; set; }
        [Required]
        [Display(Name = "Gender:")]
        public string gender { get; set; }
        [StringLength(500)]
        [Display(Name = "Bio:")]
        public string bio { get; set; }

        public bool friends { get; set; }

        public bool sentrequest { get; set; }

        public UserDetailsVM() { }
        public UserDetailsVM(UserDTO userDto)
        {
            displayName = userDto.displayName;
            email = userDto.email;
            phone_number = userDto.phone_number;
            socialLink = userDto.socialLink;
            rating = userDto.rating;
            birthday = userDto.birthday;
            if (userDto.gender)
            {
                gender = "Male";
            }
            else {
                gender = "Female";
            }
            bio = userDto.bio;
        }
    }
}