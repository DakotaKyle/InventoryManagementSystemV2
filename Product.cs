using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    public class Product
    {
        public BindingList<Part> associatedPart = new();
        
        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Instock { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public Product(int prodID, string name, int inventory, decimal price, int max, int min)
        {
            ProductID = prodID;
            Name = name;
            Instock = inventory;
            Price = price;
            Max = max;
            Min = min;
        }

        public void addAssociatedPart(Part part)
        {
            associatedPart.Add(part);
        }

        public bool removeAssociatedPart(Part part)
        {
            try
            {
                associatedPart.Remove(part);
                return true;

            } catch (Exception)
            {
                return false;
            }
            
        }

        public bool removeAssociatedPart(int partID)
        {
            return true;
        }

        public Part LookUpAssociatedPart(int partID)
        {
            int i = 0;

            foreach (Part part in associatedPart)
            {
                if (part.PartID == partID)
                {
                    return associatedPart.ElementAt(i);
                }

                i++;
            }
            return null;
        }
    }
}
