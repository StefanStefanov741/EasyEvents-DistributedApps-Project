using ApplicationService.DTOs;
using System;
using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class FriendsVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "Name")]
        public string displayName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string email { get; set; }
        [Required]
        [Display(Name = "Rating")]
        public float rating { get; set; }
        [Required]
        [Display(Name = "Friendship tier")]
        public string friendshipTier { get; set; }
        public bool sentpending { get; set; }
        public bool recievedpending { get; set; }

        public FriendsVM() { }
        public FriendsVM(UserDTO userDto)
        {
            Id = userDto.Id;
            displayName = userDto.displayName;
            email = userDto.email;
            rating = userDto.rating;
            friendshipTier = "Acquaintance";
        }
    }
}