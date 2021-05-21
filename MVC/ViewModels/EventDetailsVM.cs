using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class EventDetailsVM
    {
        public int id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Title: ")]
        public string title { get; set; }
        [Required]
        [StringLength(2500)]
        [Display(Name = "Description: ")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Location: ")]
        public string location { get; set; }
        [Required]
        [Display(Name = "Host: ")]
        public string host_name { get; set; }
        [Required]
        [Display(Name = "Created on: ")]
        public DateTime createdOn { get; set; }
        [Required]
        [Display(Name = "Event begins at:" )]
        public DateTime begins { get; set; }
        [Required]
        [Display(Name = "Event ends at: ")]
        public DateTime ends { get; set; }
        [Display(Name = "Total participants: ")]
        public int participants { get; set; }
        [Display(Name = "Friends going: ")]
        public int friendsgoing { get; set; }
        public bool joined { get; set; }
        public bool hosting { get; set; }
        public EventDetailsVM() { }
        public EventDetailsVM(EventDTO eventDto)
        {
            id = eventDto.Id;
            title = eventDto.title;
            description = eventDto.description;
            location = eventDto.location;
            createdOn = eventDto.createdOn;
            begins = eventDto.begins;
            ends = eventDto.ends;
            participants = eventDto.participants;
        }
    }
}