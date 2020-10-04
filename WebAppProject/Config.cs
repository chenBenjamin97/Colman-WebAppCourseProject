using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAppProject
{
    public class Config
    {
        private static string StaticImagesDirName = "UsersUploads";

        private static string PhysicalWebServerRootDirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); // Server files root folder
        public static string PhysicalUsersUploadsRootDirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", StaticImagesDirName); // Server files uploaded by users root folder
        public static string PhysicalElectricityFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "Elecrticity");
        public static string PhysicalWaterFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "Water");
        public static string PhysicalPropertyTaxFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "PropertyTax");
        public static string PhysicalContactApplicationFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "ContactApplication");


        public static string RelativeElectricityFilesPath = Path.Combine(StaticImagesDirName, "Elecrticity"); //Used when client looks for file via http request to the web server
        public static string RelativeWaterFilesPath = Path.Combine(StaticImagesDirName, "Water"); //Used when client looks for file via http request to the web server
        public static string RelativePropertyTaxFilesPath = Path.Combine(StaticImagesDirName, "PropertyTax"); //Used when client looks for file via http request to the web server
        public static string RelativeContactApplicationFilesPath = Path.Combine(StaticImagesDirName, "ContactApplication"); //Used when client looks for file via http request to the web server

        public static string GoogleMapsAPIKey = "AIzaSyDlZpqRaWW6LOCh0xvTCONg9vaZ8pkH0X0";

        public static HttpClient APIHttpClient = new HttpClient();

        public enum TransactionStatus
        {
            Open,
            Closed
        }

        public enum ContactAppType
        {
            [Display(Name = "Water Transaction")]
            WaterTransaction,
            
            [Display(Name = "Electricity Transaction")]
            ElectricityTransaction,

            [Display(Name = "Property Tax Transaction")]
            PropertyTaxTransaction
        }
    }
}
