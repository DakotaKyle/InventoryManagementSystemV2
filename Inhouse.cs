using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
   public class Inhouse : Part
    {
        public int InhousePart { get; set; }

        public Inhouse(int partID, string name, int instock, decimal price, int max, int min, int machineID)
        {
            PartID = partID;
            Name = name;
            Instock = instock;
            Price = price;
            Max = max;
            Min = min;
            InhousePart = machineID;
        }

        public Inhouse() { }
    }
}
