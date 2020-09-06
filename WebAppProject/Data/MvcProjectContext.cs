using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppProject.Models;

namespace WebAppProject.Data
{
    public class MvcProjectContext : DbContext
    {
        public MvcProjectContext(DbContextOptions<MvcProjectContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<WaterTransaction> WaterTransactions { get; set; }
        public DbSet<ElectricityTransaction> ElectricityTransactions { get; set; }
        public DbSet<WebAppProject.Models.ViewModel> ViewModel { get; set; }
    }
}
