using System;
using System.Linq;
using System.ComponentModel;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

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
            try
            {
                using (MySqlCommand partData = new MySqlCommand("SELECT * FROM parts", connection))
                {
                    using (DataTable partTable = new DataTable())
                    {
                        connection.Open();
                        partTable.Load(partData.ExecuteReader());

                        foreach (DataRow row in partTable.Rows)
                        {
                            int id = (int)row["part_id"];
                            string name = row["part_name"].ToString();
                            int instock = (int)(decimal)row["quantity"];
                            decimal price = (decimal)row["unit_cost"];
                            DateTime date = (DateTime)row["created_on"];

                            decimal total = calculate_total(instock, price);

                            if ((int)row["machine_id"] != 0)
                            {
                                int machine = (int)row["machine_id"];
                                Inhouse homemade = new(id, name, instock, total, date, machine);
                                AddPart(homemade);
                            }
                            else
                            {
                                string companyID = row["company_name"].ToString();
                                OutSourced source = new(id, name, instock, total, date, companyID);
                                AddPart(source);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public void initProduct()
        {
            try
            {
                using (MySqlCommand productData = new MySqlCommand("SELECT * FROM products", connection))
                {
                    using (DataTable productTable = new DataTable())
                    {
                        connection.Open();
                        productTable.Load(productData.ExecuteReader());

                        foreach (DataRow row in productTable.Rows)
                        {
                            int productid = (int)row["product_id"];
                            string name = row["product_name"].ToString();
                            int inventory = (int)(decimal)row["quantity"];
                            decimal price = (decimal)row["unit_cost"];
                            DateTime date = (DateTime)row["created_on"];

                            decimal total = calculate_total(inventory, price);

                            Product product = new Product(productid, name, inventory, price, date);
                            AddProduct(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                connection.Dispose();
            }
            finally
            {
                connection.Close();
            }
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

        public void DeletePart(Part part)
        {
            /*
             * Deletes parts from both the allParts binding list and the parts data table. 
             */
            try
            {
                id = part.PartID;
                string deletePart = "DELETE FROM parts WHERE part_id=@partId";

                connection.Open();

                using (MySqlCommand cmd = new(deletePart, connection))
                {
                    cmd.Parameters.Add("@partId", MySqlDbType.Int32).Value = id;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
                allParts.Remove(part);
            }
            catch (MySqlException)
            {
                MessageBox.Show("Part cannot be deleted while being assigned to a product.");
                connection.Dispose();
            }
        }

        private void DeleteProductFromDatabase(Product product)
        {
            id = product.ProductID;
            string deleteProduct = "DELETE FROM products WHERE product_id=@productId";

            connection.Open();

            using (MySqlCommand cmd = new(deleteProduct, connection))
            {
                cmd.Parameters.Add("@productId", MySqlDbType.Int32).Value = id;
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static Part LookupPart(int partID)
        {
            /*
             * Searches for parts based on part ID.
             */
            return allParts.FirstOrDefault(part => part.PartID == partID);
        }

        public static void AddProduct(Product product)
        {
            /*
             * Adds product to the products binding list.
             */
            products.Add(product);
        }

        public static bool removeProduct(Product product)
        {
            /*
             * Deletes product from products binding list.
             */
            Inventory inv = new();
            inv.DeleteProductFromDatabase(product);
            products.Remove(product);
            return true;
        }

        public static Product lookupProduct(int productID)
        {
            /*
             * Searches for product based on product ID.
             */
            return products.FirstOrDefault(product => product.ProductID == productID);
        }

        public void updateProduct(Product product, BindingList<Part> parts)
        {
            /*
             * This function updates the product table as well as the associated_part junction table.
             */
            string updatePart = "INSERT INTO associated_parts (product_id, part_id) VALUES (@productid, @partid)";
            string deletePart = "DELETE FROM associated_parts WHERE product_id = @productid AND part_id = @partid";
            string updateProduct = "UPDATE products SET product_name = @productname, quantity = @quantity, unit_cost = @unitcost, created_on = @created";
            MySqlCommand getIds = new("SELECT * FROM associated_parts", connection);
            DataTable asTable = new();

            connection.Open();

            asTable.Load(getIds.ExecuteReader());

            using (MySqlCommand cmd = new(updatePart, connection))//start by inserting new associated parts if they were added.
            {
                foreach (Part part in parts)
                {
                    /*
                     * If any part ids passed in the parameter do not match the part ids already in the junction table, then the new part needs to be added to the table.
                     */
                    if (!asTable.AsEnumerable().Any(row => row.Field<int>("part_id") == part.PartID))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@productid", MySqlDbType.Int32).Value = product.ProductID;
                        cmd.Parameters.Add("@partid", MySqlDbType.Int32).Value = part.PartID;
                        cmd.ExecuteNonQuery();
                    }
                }

                using (MySqlCommand command = new(deletePart, connection))
                {
                    /*
                     * If the junction table contains a partId that was not passed as a parameter, then the part needs to be deleted from the table.
                     */
                    foreach (DataRow row in asTable.Rows)
                    {
                        if (!parts.Any(part => part.PartID == row.Field<int>("part_id")))
                        {
                            command.Parameters.Clear();
                            command.Parameters.Add("@productid", MySqlDbType.Int32).Value = product.ProductID;
                            command.Parameters.Add("@partid", MySqlDbType.Int32).Value = row.Field<int>("part_id");
                            command.ExecuteNonQuery();
                        }
                    }
                }

                using (MySqlCommand update = new(updateProduct, connection))
                {
                    // Fetch current product details from the database
                    MySqlCommand fetchCurrentProduct = new MySqlCommand("SELECT * FROM products WHERE product_id = @productid", connection);
                    fetchCurrentProduct.Parameters.Add("@productid", MySqlDbType.Int32).Value = product.ProductID;
                    MySqlDataReader reader = fetchCurrentProduct.ExecuteReader();

                    if (reader.Read())
                    {
                        string currentProductName = reader["product_name"].ToString();
                        int currentQuantity = Convert.ToInt32(reader["quantity"]);
                        decimal currentUnitCost = Convert.ToDecimal(reader["unit_cost"]);
                        DateTime currentCreatedOn = Convert.ToDateTime(reader["created_on"]);

                        // Compare current product details with new values
                        if (currentProductName != product.Name || currentQuantity != product.Instock || currentUnitCost != product.Price || currentCreatedOn != product.MadeOn)
                        {
                            reader.Close();
                            // If they are different, proceed with the update
                            update.Parameters.Clear();
                            update.Parameters.Add("@productname", MySqlDbType.VarChar).Value = product.Name;
                            update.Parameters.Add("@quantity", MySqlDbType.Int32).Value = product.Instock;
                            update.Parameters.Add("@unitcost", MySqlDbType.Decimal).Value = product.Price;
                            update.Parameters.Add("@created", MySqlDbType.DateTime).Value = product.MadeOn;
                            update.ExecuteNonQuery();
                        }
                    }
                }
            }
                connection.Close();
        }
    }
}
