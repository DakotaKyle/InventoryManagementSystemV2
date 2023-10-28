using Org.BouncyCastle.Asn1.Cms;
using System;

namespace InventoryManagementSystem
{
    public abstract class Part
    {
        public int PartID { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Instock { get; set; }

        public DateTime ArrivedOn { get; set; }

    }
}
