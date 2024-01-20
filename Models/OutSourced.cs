using System;

namespace InventoryManagementSystem.Models
{
    public class OutSourced : Part
    {
        public string CompanyName { get; set; }

        public OutSourced(int partID, string name, int instock, decimal price, DateTime arrivedOn, string company)
        {
            PartID = partID;
            Name = name;
            Instock = instock;
            Price = price;
            ArrivedOn = arrivedOn;
            CompanyName = company;
        }
    }
}
