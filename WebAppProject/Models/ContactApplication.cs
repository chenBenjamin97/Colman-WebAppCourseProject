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
        [Display(Name = "Application ID")]
        public int ContactAppID { get; set; }

        [Display(Name ="Client ID")]
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        [Display(Name = "Transaction Type")]
        public Config.ContactAppType ContactType { get; set; } //The rellevant transaction type

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [NotMapped] //Make this property not be written to the DB
        [Display(Name = "Added Image")]
        public IFormFile Img { get; set; }

        [Display(Name = "Image Path")]
        public string ImgPath { get; set; }
        public Config.TransactionStatus Status { get; set; }

        public User User { get; set; }
    }
}
