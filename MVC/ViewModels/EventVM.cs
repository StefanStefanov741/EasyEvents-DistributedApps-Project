using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.ViewModels
{
    public class EventVM
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string title { get; set; }
        [Required]
        [StringLength(2500)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Location")]
        public string location { get; set; }
        [Required]
        public int host_id { get; set; }
        public DateTime createdOn { get; set; }
        [Required]
        [Display(Name = "Begins date")]
        [DataType(DataType.Date)]
        public DateTime begins_date { get; set; }
        [Required]
        [Display(Name = "Begins time")]
        [DataType(DataType.Time)]
        public DateTime begins_time { get; set; }
        [Required]
        [Display(Name = "Ends date")]
        [DataType(DataType.Date)]
        public DateTime ends_date { get; set; }
        [Required]
        [Display(Name = "Ends time")]
        [DataType(DataType.Time)]
        public DateTime ends_time { get; set; }
        public int participants { get; set; }

        public EventVM() { }
        public EventVM(EventDTO eventDto)
        {
            title = eventDto.title;
            description = eventDto.description;
            location = eventDto.location;
            host_id = eventDto.host_id;
            createdOn = eventDto.createdOn;
            begins_date = eventDto.begins.Date;
            begins_time = eventDto.begins;
            ends_date = eventDto.ends.Date;
            ends_time = eventDto.ends;
            participants = eventDto.participants;
        }
    }
}