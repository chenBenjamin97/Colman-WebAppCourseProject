using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject
{
    public class Config
    {
        private static string StaticImagesDirName = "UsersUploads";

        private static string PhysicalWebServerRootDirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); //No class need access to this folder (they use only sub-folders) 
        public static string PhysicalElectricityFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "Elecrticity");
        public static string PhysicalWaterFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "Water");
        public static string PhysicalPropertyTaxFilesPath = Path.Combine(PhysicalWebServerRootDirPath, StaticImagesDirName, "PropertyTax");


        public static string RelativeElectricityFilesPath = Path.Combine(StaticImagesDirName, "Elecrticity"); //Used when client looks for file via http request to the web server
        public static string RelativeWaterFilesPath = Path.Combine(StaticImagesDirName, "Water"); //Used when client looks for file via http request to the web server
        public static string RelativePropertyTaxFilesPath = Path.Combine(StaticImagesDirName, "PropertyTax"); //Used when client looks for file via http request to the web server


    }
}
