using System;
using System.ComponentModel.Design;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InventoryManagementSystem
{
    public partial class ModifyPartWindow : Window
    {
        private static string connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);
        Part oldPart;

        int id, instock, machine;
        string name, companyID, timeString;
        decimal price, total;
        DateTime date;

        public ModifyPartWindow(Inhouse inhousePart)
        {
            InitializeComponent();
            inHouseButton.IsChecked = true;

            idTextBox.Text = inhousePart.PartID.ToString();
            nameTextBox.Text = inhousePart.Name;
            inventoryTextBox.Text = inhousePart.Instock.ToString();
            priceTextBox.Text = inhousePart.Price.ToString();
            Date_Picker.SelectedDate = inhousePart.ArrivedOn.Date;
            timeTextBox.Text = inhousePart.ArrivedOn.ToShortTimeString();
            machineTextBox.Text = inhousePart.InhousePart.ToString();

            oldPart = inhousePart;
        }

        public ModifyPartWindow(OutSourced outsourcePart)
        {
            InitializeComponent();
            outsourced.IsChecked = true;

            idTextBox.Text = outsourcePart.PartID.ToString();
            nameTextBox.Text = outsourcePart.Name;
            inventoryTextBox.Text = outsourcePart.Instock.ToString();
            priceTextBox.Text = outsourcePart.Price.ToString();
            Date_Picker.SelectedDate = outsourcePart.ArrivedOn.Date;
            timeTextBox.Text = outsourcePart.ArrivedOn.ToShortTimeString();
            machineTextBox.Text = outsourcePart.CompanyName;

            oldPart = outsourcePart;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            id = oldPart.PartID;
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
                    update_part();
                    Inventory.allParts.Remove(oldPart);
                    OutSourced source = new(id, name, instock, price, date, companyID);
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
                        update_part();
                        Inventory.allParts.Remove(oldPart);
                        Inhouse homemade = new(id, name, instock, price, date, machine);
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
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

        private void update_part()
        {
            string userData = "UPDATE parts SET part_name=@name, quantity=@instock, unit_cost=@price, created_on=@date, machine_id=@machine, company_name=@company WHERE part_id=@partId";

            using (MySqlConnection con = new(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand cmd = new(userData, connection))
                    {
                        cmd.Parameters.Add("@partId", MySqlDbType.Int32).Value = id;
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
                    }

                    connection.Close();
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
