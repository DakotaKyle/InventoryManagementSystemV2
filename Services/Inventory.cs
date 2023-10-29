using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace InventoryManagementSystem.Database_Service
{
    class Inventory
    {
        public static BindingList<Part> allParts = new();
        public static BindingList<Product> products = new();

        private static String connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);

        public void initPart()
        {
            MySqlCommand partData = new("SELECT * FROM parts", connection);
            DataTable partTable = new();

            string name, companyID;
            int id, instock, machine;
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

        public static decimal calculate_total(decimal cost, decimal units)
        {
            return cost * units;
        }

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
