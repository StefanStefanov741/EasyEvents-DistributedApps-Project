using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class EventListVM
    {
        public int id { get; set; }
        [StringLength(100)]
        [Display(Name = "Title")]
        public string title { get; set; }
        [StringLength(200)]
        [Display(Name = "Location")]
        public string location { get; set; }
        [Display(Name = "Begins at")]
        public DateTime begins { get; set; }
        [Display(Name = "Ends at")]
        public DateTime ends { get; set; }
        [Display(Name = "Ended")]
        public bool ended { get; set; }
        [Display(Name = "Likes")]
        public int likes { get; set; }
        [Display(Name = "Participants")]
        public int participants { get; set; }
        public bool likedByUser { get; set; }

        public EventListVM() { }

        public EventListVM(EventDTO eventDto) {
            id = eventDto.Id;
            title = eventDto.title;
            location = eventDto.location;
            begins = eventDto.begins;
            ends = eventDto.ends;
            ended = eventDto.ended;
            likes = eventDto.likes;
            participants = eventDto.participants;
        }
    }
}