using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject
{
    public class Image
    {

        // Returns the new image relative path
        public static string Save(int UserID, IFormFile Img, int ObjID, string TransactionType) // returns the relative image path (on the server)
        {
            string RelativeAddressOfNewFile;
            string newFileName;
            string PhysicalAddressOfSaveNewFile;

            switch (TransactionType)
            {
                case "Water":
                    newFileName = UserID.ToString() + '-' + ObjID.ToString() + Path.GetExtension(Img.FileName);
                    PhysicalAddressOfSaveNewFile = Path.Combine(Config.PhysicalWaterFilesPath, newFileName);
                    
                    using (var stream = new FileStream(PhysicalAddressOfSaveNewFile, FileMode.Create)) // Provides a convenient syntax that ensures the correct use of IDisposable objects
                    {
                        Img.CopyTo(stream);
                    }

                    RelativeAddressOfNewFile = Path.Combine(Config.RelativeWaterFilesPath, newFileName);
                    break;
                    
                case "Electricity":
                    newFileName = UserID.ToString() + '-' + ObjID.ToString() + Path.GetExtension(Img.FileName);
                    PhysicalAddressOfSaveNewFile = Path.Combine(Config.PhysicalElectricityFilesPath, newFileName);

                    using (var stream = new FileStream(PhysicalAddressOfSaveNewFile, FileMode.Create)) // Provides a convenient syntax that ensures the correct use of IDisposable objects
                    {
                        Img.CopyTo(stream);
                    }

                    RelativeAddressOfNewFile = Path.Combine(Config.RelativeElectricityFilesPath, newFileName);
                    break;

                case "PropertyTax":
                    newFileName = UserID.ToString() + '-' + ObjID.ToString() + Path.GetExtension(Img.FileName);
                    PhysicalAddressOfSaveNewFile = Path.Combine(Config.PhysicalPropertyTaxFilesPath, newFileName);

                    using (var stream = new FileStream(PhysicalAddressOfSaveNewFile, FileMode.Create)) // Provides a convenient syntax that ensures the correct use of IDisposable objects
                    {
                        Img.CopyTo(stream);
                    }

                    RelativeAddressOfNewFile = Path.Combine(Config.RelativePropertyTaxFilesPath, newFileName);
                    break;

                default:
                    RelativeAddressOfNewFile = null; // Error
                    break;
            }
            
            return RelativeAddressOfNewFile;
        }

        public static void Delete(string ImgRelativePath)
        {
            var relativePathSplitted = ImgRelativePath.Split('\\');
            var fileNameAndExtension = relativePathSplitted[relativePathSplitted.Length - 1];
            var transactionType = relativePathSplitted[relativePathSplitted.Length - 2];

            var fileToDeletePath = Path.Combine(Config.PhysicalUsersUploadsRootDirPath, transactionType, fileNameAndExtension);
            System.IO.File.Delete(fileToDeletePath);
        }

        //Returns the new image relative path (after the edit)
        public static string Edit(int UserID, string ImgPathBeforeEdit, IFormFile Img, int ObjID, string TransactionType)
        {
            // Image.Delete(ImgPathBeforeEdit); - Not needed: FileMode.Create overwrites if file with same name is exist already

            return Image.Save(UserID, Img, ObjID, TransactionType);
        }

        // Have not been checked:
        public static string Rename(string ImgPathBeforeChange, int UserID, int ObjID)
        {
            var relativePathSplittedBeforeChange = ImgPathBeforeChange.Split('\\');
            var fileNameAndExtension = relativePathSplittedBeforeChange[relativePathSplittedBeforeChange.Length - 1];
            var fileExtensionOnly = fileNameAndExtension.Split('.')[1];

            var fileNameAndExtensionAfterChange = UserID + '-' + ObjID + '.' + fileExtensionOnly;

            string relativePathAfterChange = "";

            for (int i=0; i<=relativePathSplittedBeforeChange.Length; i++)
            {
                if (i == (relativePathSplittedBeforeChange.Length - 1))
                {
                    relativePathAfterChange += fileNameAndExtensionAfterChange;
                    break;
                } else
                {
                    relativePathAfterChange = relativePathAfterChange + relativePathSplittedBeforeChange + "\\";
                }
            }

            System.IO.File.Move(ImgPathBeforeChange, relativePathAfterChange);

            return relativePathAfterChange;
        }
    }
}
