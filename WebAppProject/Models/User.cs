using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject.Models
{
    public class User
    {
        [Display(Name ="ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        public int UserID { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Entrance Date")]
        [DataType(DataType.Date)]
        public DateTime EnteranceDate { get; set; }

        [Display(Name = "Property City")]
        public string PropertyCity { get; set; }

        [Display(Name = "Property Street")]
        public string PropertyStreet { get; set; }

        [Display(Name = "Property Street Number")]
        public string PropertyStreetNumber { get; set; } // String and not int for cases like: street number 18א

        [Display(Name = "Property Apartment Number")]
        public int? ApartmentNumber { get; set; } //Not required, only if the property is a building

        [Display(Name = "Admin User")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        public ICollection<WaterTransaction> WaterTransactions { get; set; }
        public ICollection<ElectricityTransaction> ElectricityTransactions { get; set; }
        public ICollection<PropertyTaxTransaction> PropertyTaxTransactions { get; set; }
    }
}
