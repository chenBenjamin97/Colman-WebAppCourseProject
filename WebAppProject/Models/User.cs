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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] // Primary Key
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
       
        [DataType(DataType.Date)]
        public DateTime EnteranceDate { get; set; }
        public string PropertyCity { get; set; }
        public string PropertyStreet { get; set; }
        public string PropertyStreetNumber { get; set; } // String and not int for cases like: street number 18א
        public int? ApartmentNumber { get; set; } //Not required, only if the property is a building
        public bool IsAdmin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ICollection<WaterTransaction> WaterTransactions { get; set; }
        public ICollection<ElectricityTransaction> ElectricityTransactions { get; set; }
        public ICollection<PropertyTax> propertyTaxes { get; set; }
    }
}
