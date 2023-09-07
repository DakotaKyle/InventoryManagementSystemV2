using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
   public class OutSourced : Part
    {
        public String CompanyName { get; set; }

        public OutSourced(int partID, string name, int instock, decimal price, int max, int min, string company) 
        {
            PartID = partID;
            Name = name;
            Instock = instock;
            Price = price;
            Max = max;
            Min = min;
            CompanyName = company;
        }
    }
}
