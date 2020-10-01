using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppProject
{
    public class ContactAppsTypesJSON
    {
        public string type;
        public int value;
    
        public ContactAppsTypesJSON(string typeInput, int valInput)
        {
            type = typeInput;
            value = valInput;
        }
    }
}
