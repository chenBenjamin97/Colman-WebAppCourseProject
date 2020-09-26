using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject.Models
{
    public class ContactApplication
    {
        [Key] //Primary Key
        [Display(Name = "Contact Application ID")]
        public int ContactAppID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        [Display(Name = "Contact Type")]
        public Config.ContactAppType ContactType { get; set; } //The rellevant transaction
        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Added Image")]
        [NotMapped] //Make this property not be written to the DB
        public IFormFile Img { get; set; }
        [Display(Name = "Image Path")]
        public string ImgPath { get; set; }
        public Config.TransactionStatus Status { get; set; }

        public User User { get; set; }
    }
}
