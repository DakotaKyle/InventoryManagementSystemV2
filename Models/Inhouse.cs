using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public class Inhouse : Part
    {
        public int InhousePart { get; set; }

        public Inhouse(int partID, string name, int instock, decimal price, DateTime madeOn, int machineID)
        {
            PartID = partID;
            Name = name;
            Instock = instock;
            Price = price;
            ArrivedOn = madeOn;
            InhousePart = machineID;
        }

        public Inhouse() { }
    }
}
