using System;
using System.Linq;
using System.ComponentModel;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.ComponentModel.Design;
using System.Reflection.PortableExecutable;

namespace InventoryManagementSystem.Database_Service
{
    /*
     * This class is responsible for helping with database functions and performing mathmatical calculations related to inventory management.
     */
    class Inventory
    {
        public static BindingList<Part> allParts = new(); // A binding list for the part data table.
        public static BindingList<Product> products = new(); // A binding list for the product data table.
        /*
         * Initialize the database connection.
         */
        private static String connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);
        private int id;

        public void initPart()
        {
            /*
            * Populate the allParts binding list with data from the part data table.
            */
            MySqlCommand partData = new("SELECT * FROM parts", connection);
            DataTable partTable = new();

            string name, companyID;
            int instock, machine;
            int i = 0;
            decimal price;
            DateTime date;

            try
            {
                connection.Open();
                partTable.Load(partData.ExecuteReader());
                connection.Close();

                foreach (DataRow row in partTable.Rows)
                {
                    id = (int)partTable.Rows[i]["part_id"];
                    name = partTable.Rows[i]["part_name"].ToString();
                    instock = (int)(decimal)partTable.Rows[i]["quantity"];
                    price = (decimal)partTable.Rows[i]["unit_cost"];
                    date = (DateTime)partTable.Rows[i]["created_on"];

                    decimal total = calculate_total(instock, price);

                    if ((int)partTable.Rows[i]["machine_id"] != 0)
                    {
                        machine = (int)partTable.Rows[i]["machine_id"];
                        Inhouse homemade = new(id, name, instock, total, date, machine);
                        AddPart(homemade);
                    }
                    else
                    {
                        companyID = partTable.Rows[i]["company_name"].ToString();

                        OutSourced source = new(id, name, instock, total, date, companyID);
                        AddPart(source);
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                connection.Dispose();
            }
            finally { connection.Close(); }
        }

        public void initProduct()
        {
            /*
            * Populate the products binding list with data from the products data table.
            */
            MySqlCommand productData = new("SELECT * FROM products", connection);
            DataTable productTable = new();

            int productid, inventory;
            int i = 0;
            string name;
            decimal price;
            DateTime date;

            try
            {
                connection.Open();
                productTable.Load(productData.ExecuteReader());
                connection.Close();

                foreach (DataRow row in productTable.Rows)
                {
                    productid = (int)productTable.Rows[i]["product_id"];
                    name = productTable.Rows[i]["product_name"].ToString();
                    inventory = (int)(decimal)productTable.Rows[i]["quantity"];
                    price = (decimal)productTable.Rows[i]["unit_cost"];
                    date = (DateTime)productTable.Rows[i]["created_on"];

                    decimal total = calculate_total(inventory, price);

                    Product product = new(productid, name, inventory, price, date);
                    AddProduct(product);
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                connection.Dispose();
            }
            finally { connection.Close(); }
        }

        public static decimal calculate_total(decimal cost, decimal units)
        {
            /*
             * Returns the total cost of a part or product.
             */
            return cost * units;
        }

        public static decimal calculate_unit_price(decimal cost, decimal units)
        {
            /*
             * Returns the unit price of a part or product.
             */
            return cost / units;
        }

        public static void AddPart(Part part)
        {
            /*
             * Adds parts to the allParts binding list.
             */
            allParts.Add(part);
        }

        public static void DeletePart(Part part)
        {
            /*
             * Deletes parts from both the allParts binding list and the parts data table. 
             */
            Inventory inv = new();
            inv.DeletePartFromDatabase(part);
            allParts.Remove(part);
        }

        private void DeletePartFromDatabase(Part part)
        {
            /*
             * Deletes parts from the part data table.
             */
            id = part.PartID;
            string deletePart = "DELETE FROM parts WHERE part_id=@partId";

            connection.Open();

            using (MySqlCommand cmd = new(deletePart, connection))
            {
                cmd.Parameters.Add("@partId", MySqlDbType.Int32).Value = id;
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public static Part LookupPart(int partID)
        {
            /*
             * Searches for parts based on part ID.
             */
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
            /*
             * Updates part based on part ID.
             */
            LookupPart(partID);
            DeletePart(part);
        }

        public static void AddProduct(Product product)
        {
            /*
             * Adds product to the products binding list.
             */
            products.Add(product);
        }

        public static bool removeProduct(int partID)
        {
            return true;
        }

        public static bool removeProduct(Product product)
        {
            /*
             * Deletes product from products binding list.
             */
            products.Remove(product);
            return true;
        }

        public static Product lookupProduct(int productID)
        {
            /*
             * Searches for product based on product ID.
             */
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
