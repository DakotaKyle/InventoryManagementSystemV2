using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;

namespace InventoryManagementSystem
{
    class Inventory
    {
        public static BindingList<Part> allParts = new();
        public static BindingList<Product> products = new();

        public static void AddPart(Part part)
        {
            allParts.Add(part);
        }

        public static bool DeletePart(Part part)
        {
            allParts.Remove(part);
            return true;
        }

        public static Part LookupPart(int partID)
        {
            int i = 0;

            foreach (Part part in allParts)
            {

                if (part.PartID == partID)
                {
                    return allParts.ElementAt(i);
                }

                i++;
            }

            return null;
        }

        public static void UpdatePart(int partID, Part part)
        {
            LookupPart(partID);
            DeletePart(part);
        }

        public static void AddProduct(Product product)
        {
            products.Add(product);
        }

        public static bool removeProduct(int partID)
        {
            return true;
        }

        public static bool removeProduct(Product product)
        {
            products.Remove(product);
            return true;
        }

        public static Product lookupProduct(int productID)
        {
            int i = 0;

            foreach (Product product in products)
            {
                if (product.ProductID == productID)
                {
                    return products.ElementAt(i);
                }
            }

            return null;
        }

        public static void updateProduct(int productId, Product product)
        {
            lookupProduct(productId);
            removeProduct(product);
        }
    }
}
