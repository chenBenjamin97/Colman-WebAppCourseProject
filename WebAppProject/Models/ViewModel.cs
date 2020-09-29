using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject.Models
{
    public class ViewModel
    {
        [ForeignKey("UserID")]
        public int ID { get; set; }

        [NotMapped]
        public List<User> Users { get; set; }
        
        [NotMapped]
        public List<WaterTransaction> WaterTransactions { get; set; }

        [NotMapped]
        public List<ElectricityTransaction> ElectricityTransaction { get; set; }

        [NotMapped]
        public List<PropertyTaxTransaction> PropertyTaxTransactions { get; set; }

        [NotMapped]
        public List<ContactApplication> ContactApplications { get; set; }
    }
}
