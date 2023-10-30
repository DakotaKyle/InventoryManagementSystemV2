using System;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem
{
    public partial class AddPartWindow : Window
    {
        private static String connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);
        string name, companyID, timeString;
        int id, instock, machine;
        decimal price, total;
        DateTime date;

        public AddPartWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

            if (nameTextBox.Text.Length != 0)
            {
                name = nameTextBox.Text;
            }
            else
            {
                MessageBox.Show("Enter a part name to continue.");
                return;
            }
            
            timeString = Date_Picker.Text + " " + timeTextBox.Text;

            if (decimal.TryParse(priceTextBox.Text, out decimal priceVal) && priceVal > 0)
            {
                price = priceVal;

                if (int.TryParse(inventoryTextBox.Text, out int invVal) && (invVal >= 1))
                {
                    instock = invVal;
                    total = Inventory.calculate_total(instock, price);
                }
                else
                {
                    MessageBox.Show("Quantity field requires a positive whole number.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Unit Cost field requires a positive number.");
                return;
            }

            if (DateTime.TryParse(timeString, out DateTime newTime))
            {
                date = newTime;
            }
            else
            {
                MessageBox.Show("Please pick a valid date and time.");
                return;
            }

            if ((bool)outsourced.IsChecked)
            {
                if (machineTextBox.Text.Length != 0)
                {
                    companyID = machineTextBox.Text;

                    add_part();
                    OutSourced source = new(id, name, instock, total, date, companyID);
                    Inventory.AddPart(source);
                }
                else
                {
                    MessageBox.Show("Please Enter Company Name");
                    return;
                }
            }
            else if ((bool)inHouseButton.IsChecked)
            {
                if (int.TryParse(machineTextBox.Text, out int machineID) && machineID > 0)
                {
                    machine = machineID;

                    add_part();
                    Inhouse homemade = new(id, name, instock, total, date, machine);
                    Inventory.AddPart(homemade);
                }
                else
                {
                    MessageBox.Show("Machine ID must be a postive number");
                    return;
                }
            }

            MessageBox.Show("Part has been added to inventory.");
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

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
            DateTime startTime = Date_Picker.SelectedDate.Value.ToUniversalTime();
            timeTextBox.Text = startTime.ToShortTimeString();
        }

        private void add_part()
        {
            string userData = "INSERT INTO parts (part_name, quantity, unit_cost, created_on, machine_id, company_name) VALUES (@name, @instock, @price, @date, @machine, @company)";
            MySqlCommand getPartId = new("SELECT part_id FROM parts ORDER BY part_id desc", connection);

            using (MySqlConnection con = new(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand cmd = new(userData, connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        cmd.Parameters.Add("@instock", MySqlDbType.Decimal).Value = (decimal)instock;
                        cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = price;
                        cmd.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;

                        if ((bool)inHouseButton.IsChecked)
                        {
                            cmd.Parameters.Add("@machine", MySqlDbType.Int32).Value = machine;
                            cmd.Parameters.Add("@company", MySqlDbType.VarChar).Value = "NA";
                        }
                        else if ((bool)outsourced.IsChecked)
                        {
                            cmd.Parameters.Add("@machine", MySqlDbType.Int32).Value = 0;
                            cmd.Parameters.Add("@company", MySqlDbType.VarChar).Value = companyID;
                        }
                        cmd.ExecuteNonQuery();

                        id = (int)getPartId.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    connection.Dispose();
                }
                finally { connection.Close(); }
            }
        }
    }
}
