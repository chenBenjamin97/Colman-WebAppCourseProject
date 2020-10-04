using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAppProject.Data;

namespace WebAppProject.Models
{
    public class PropertyTaxTransaction
    {
        [Display(Name = "Client ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Display(Name = "Property ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        [PropertyIDValidation]
        public int PropertyID { get; set; }

        [Display(Name = "Image Path")]
        public string ImgPath { get; set; }
        
        [NotMapped] //Make this property not be written to the DB
        [Display(Name = "Added Image")]
        public IFormFile PropertyTaxContractImg { get; set; }
        
        public Config.TransactionStatus Status { get; set; }

        public User User { get; set; }
    }

    public class PropertyIDValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (MvcProjectContext)validationContext
                         .GetService(typeof(MvcProjectContext));

            var usersWithSameID = _context.PropertyTaxTransactions.Where(property => property.PropertyID.ToString().Equals(value.ToString()));

            if (usersWithSameID.Count() != 0)
            {
                return new ValidationResult("Another Transaction With This Property ID Already Exists");
            }

            return ValidationResult.Success;
        }
    }
}
