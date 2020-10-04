using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAppProject.Data;

namespace WebAppProject.Models
{
    public class WaterTransaction
    {
        [Display(Name = "Client ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Display(Name = "Last Read Date")]
        [DataType(DataType.Date)]
        public DateTime WaterMeterLastReadDate { get; set; }

        [Display(Name = "Water Meter ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        [WaterMeterIDValidation]
        public int WaterMeterID { get; set; }

        [NotMapped] //Make this property not be written to the DB
        [Display(Name = "Added Image")]
        public IFormFile WaterMeterImg { get; set; }

        [Display(Name = "Image Path")]
        public string ImgPath { get; set; }
        public Config.TransactionStatus Status { get; set; }

        public User User { get; set; }
    }

    public class WaterMeterIDValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (MvcProjectContext)validationContext
                         .GetService(typeof(MvcProjectContext));

            var usersWithSameID = _context.WaterTransactions.Where(water => water.WaterMeterID.ToString().Equals(value.ToString()));

            if (usersWithSameID.Count() != 0)
            {
                return new ValidationResult("Another Transaction With This Water Meter ID Already Exists");
            }

            return ValidationResult.Success;
        }
    }
}
