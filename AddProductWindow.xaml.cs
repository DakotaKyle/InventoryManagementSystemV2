using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem
{

    public partial class AddProductWindow : Window
    {
        /*
         * This class is responsible for product validation, adding the new product to the product binding list
         * as well as the associated parts to the associated parts bindling list. Finally, it will add the new product to the product data table and associated parts data table.
         */

        public BindingList<Part> NewParts = new(); // A temporary binding list for part data.

        // Create the database connection
        private static string connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);

        //Product fields
        int productid, instock;
        string name, timeString;
        decimal price;
        DateTime date;

        public AddProductWindow()
        {
            /*
             * Populate the parts datagrid.
             */
            InitializeComponent();
            AllPartsDataGrid.ItemsSource = Inventory.allParts;
        }

        private void AllPartSearchButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * A search function for parts.
             */
            if (int.TryParse(AllPartsTextBox.Text, out int search) && search >= 1)
            {
                Part match = Inventory.LookupPart(search);

                try
                {
                    AllPartsDataGrid.ScrollIntoView(match);
                    AllPartsDataGrid.SelectedItem = match;
                }
                catch (Exception)
                {
                    MessageBox.Show("Part ID not found.");
                }
            }
            else
            {
                MessageBox.Show("Enter Part ID");
                return;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * This method validates the fields, adds the product to the producting binding list as well as the product data table.
             */

            timeString = Date_Picker.Text + " " + timeTextBox.Text;

            if (NameTextBox.Text.Length != 0) //validates name
            {
                name = NameTextBox.Text;
            }
            else
            {
                MessageBox.Show("Name field cannot be empty.");
                return;
            }

            if (decimal.TryParse(PriceTextBox.Text, out decimal priceVal) && priceVal > 0) // validates price
            {
                price = priceVal;
            }
            else
            {
                MessageBox.Show("Price field requires a positive number.");
                return;
            }

            if (int.TryParse(InventoryTextBox.Text, out int invVal) && (invVal >= 1)) // validates inventory count
            {
                instock = invVal;
            }
            else
            {
                MessageBox.Show("Inventory field requires a positive whole number.");
                return;
            }
            if (DateTime.TryParse(timeString, out DateTime newTime)) //validates date
            {
                date = newTime;
            }
            else
            {
                MessageBox.Show("Please pick a valid date and time.");
                return;
            }

            add_product(); //adds product to the products data table

            Product product = new(productid, name, instock, price, date);
            Inventory.AddProduct(product); //adds product to the product binding list

            foreach (Part part in NewParts) //adds each part to the associated part binding list
            {
                product.addAssociatedPart(part);
                add_associated_parts(part);
            }

            MessageBox.Show("Product has been added to inventory.");
            Close();
        }

        private void AllPartsAddButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * This method adds the parts from the allparts datagrid to the prodcut datagrid
             */

            if (AllPartsDataGrid.SelectedItem != null)
            {
                Part addPart = (Part)AllPartsDataGrid.SelectedItem;
                NewParts.Add(addPart);

                ProductDataGrid.ItemsSource = NewParts; 
            }
            else
            {
                MessageBox.Show("Select a part to add it to your new product.");
            }
        }

        private void ProductDeleteButton_Click(object sender, RoutedEventArgs e)
        {

            /*
             * This method removes a part from the product datagrid.
             */

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this part from this product? " +
                "This action cannot be undone.", "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                NewParts.Remove((Part)ProductDataGrid.SelectedItem);
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * This method closes the current window.
             */
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to exit? Any unsaved progess will be lost.",
                "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Close();
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
             * This method prepares the timeString for being added to the data table.
             */
            DateTime startTime = Date_Picker.SelectedDate.Value.ToUniversalTime();
            timeTextBox.Text = startTime.ToShortTimeString();
        }

        private void add_product()
        {
            /*
             * This method adds the new product to the product data table.
             */

            string productData = "INSERT INTO products (product_name, quantity, unit_cost, created_on) VALUES (@name, @count, @unit_cost, @created_on)";
            MySqlCommand getProductId = new("SELECT product_id FROM products ORDER BY product_id desc", connection);

            using MySqlConnection con = new(connectionString);
            try
            {
                connection.Open();

                using (MySqlCommand cmd = new(productData, connection))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@count", MySqlDbType.Decimal).Value = (decimal)instock;
                    cmd.Parameters.Add("@unit_cost", MySqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@created_on", MySqlDbType.DateTime).Value = date;
                    cmd.ExecuteNonQuery();

                    productid = (int)getProductId.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                connection.Dispose();
            }
            finally
            {
                connection.Close();
            }
        }

        private void add_associated_parts(Part part)
        {
            /*
             * This method adds the new product's associated parts to the associated_parts data table.
             */

            string productData = "INSERT INTO associated_parts (product_id, part_id) VALUES (@product_id, @part_id)";

            using (MySqlConnection con = new(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand cmd = new(productData, connection))
                    {
                        cmd.Parameters.Add("@product_id", MySqlDbType.VarChar).Value = productid;
                        cmd.Parameters.Add("@part_id", MySqlDbType.Int32).Value = part.PartID;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    connection.Dispose();
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
