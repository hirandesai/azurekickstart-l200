using System;
using System.ComponentModel.DataAnnotations;

namespace AzureKickStart.Models
{
    public class Person
    {
        public int ID { get; set; }
        public string FullName { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Photo")]
        public string ImageURL { get; set; }
    }
}