using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        private static string connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);

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
        public Product()
        {
            
        }

        public void InitAsp(Product prod)
        {
            MySqlCommand productData = new("SELECT Parts.* FROM parts INNER JOIN associated_parts ON parts.part_id = associated_parts.part_id WHERE associated_parts.product_id =@productId;", connection);
            DataTable productTable = new();

            int partid, inventory, machineID;
            int i = 0;
            string name, companyName;
            decimal price;
            DateTime date;

            try
            {
                connection.Open();

                productData.Parameters.Add("@productId", MySqlDbType.Int32).Value = prod.ProductID;
                productTable.Load(productData.ExecuteReader());
                connection.Close();

                foreach (DataRow row in productTable.Rows)
                {
                    partid = (int)productTable.Rows[i]["part_id"];
                    name = productTable.Rows[i]["part_name"].ToString();
                    inventory = (int)(decimal)productTable.Rows[i]["quantity"];
                    price = (decimal)productTable.Rows[i]["unit_cost"];
                    date = (DateTime)productTable.Rows[i]["created_on"];
                    machineID = (int)productTable.Rows[i]["machine_id"];
                    companyName = productTable.Rows[i]["Company_name"].ToString();

                    if ((int)productTable.Rows[i]["machine_id"] != 0)
                    {
                        machineID = (int)productTable.Rows[i]["machine_id"];
                        Inhouse homemade = new(partid, name, inventory, price, date, machineID);
                        addAssociatedPart(homemade);
                    }
                    else
                    {
                        companyName = productTable.Rows[i]["company_name"].ToString();

                        OutSourced source = new(partid, name, inventory, price, date, companyName);
                        addAssociatedPart(source);
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

        public void DeleteAsP(Part part)
        {
            /*
             * Deletes associated parts from both the 
             */
            Product prod = new();
            prod.DeleteAspFromDatabase(part);
            associatedPart.Remove(part);
        }

        public void DeleteAspFromDatabase(Part part)
        {
            /*
             * Deletes parts from the part data table.
             */
            int id = part.PartID;
            string deletePart = "DELETE FROM associated_parts WHERE part_id=@partId";

            connection.Open();

            using (MySqlCommand cmd = new(deletePart, connection))
            {
                cmd.Parameters.Add("@partId", MySqlDbType.Int32).Value = id;
                cmd.ExecuteNonQuery();
            }

            connection.Close();
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
