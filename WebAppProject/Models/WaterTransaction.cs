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
    public class WaterTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [DataType(DataType.Date)]
        public DateTime WaterMeterLastReadDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        public int WaterMeterID { get; set; }

        [NotMapped] //Make this property not be written to the DB
        public IFormFile WaterMeterImg { get; set; }
        public string ImgPath { get; set; }
        public User User { get; set; }
    }
}
