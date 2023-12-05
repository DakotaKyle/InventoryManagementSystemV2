using System;
using System.ComponentModel;
using System.Linq;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public BindingList<Part> associatedPart = new(); // Each part that is associated with each product.

        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Instock { get; set; }
        public DateTime MadeOn { get; set; }

        public Product(int prodID, string name, int inventory, decimal price, DateTime madeOn)
        {
            ProductID = prodID;
            Name = name;
            Instock = inventory;
            Price = price;
            MadeOn = madeOn;
        }

        public void addAssociatedPart(Part part)
        {
            associatedPart.Add(part); //add the part to the binding list.
        }

        public bool removeAssociatedPart(Part part)
        {
            /*
             * removes part from binding list.
             */
            try
            {
                associatedPart.Remove(part);
                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public Part LookUpAssociatedPart(int partID)
        {
            /*
             * Searches part by ID.
             */
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
