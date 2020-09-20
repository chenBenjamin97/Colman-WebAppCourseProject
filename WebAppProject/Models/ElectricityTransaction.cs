using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebAppProject.Models
{
    public class ElectricityTransaction
    {
        [Display(Name = "Client ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Display(Name = "Last Read Date")]
        [DataType(DataType.Date)]
        public DateTime ElectricityMeterLastRead { get; set; }

        [Display(Name = "Electricity Meter ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        public int ElectricityMeterID { get; set; }

        [NotMapped] //Make this property not be written to the DB
        public IFormFile ElectricityMeterImg { get; set; }
        
        public string ImgPath { get; set; }

        public User User { get; set; }
    }
}
