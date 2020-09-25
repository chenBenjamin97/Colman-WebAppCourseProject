using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        public int PropertyID { get; set; }
        
        public string ImgPath { get; set; }
        
        [NotMapped] //Make this property not be written to the DB
        public IFormFile PropertyTaxContractImg { get; set; }
        
        public Config.TransactionStatus Status { get; set; }

        public User User { get; set; }
    }
}
