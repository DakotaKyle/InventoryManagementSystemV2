using System;
using System.Windows;
using System.Windows.Controls;

namespace InventoryManagementSystem
{
    public partial class AddPartWindow : Window
    {

        public AddPartWindow()
        {
            InitializeComponent();
            Random ranID = new();
            idTextBox.Text = ranID.Next().ToString();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

            int id, instock, machine;
            String name, companyID, timeString;
            decimal price;
            DateTime date;

            id = int.Parse(idTextBox.Text);
            timeString = Date_Picker.Text + " " + timeTextBox.Text;

                if (nameTextBox.Text.Length != 0)
                {
                    name = nameTextBox.Text;    
                }
                else
                { 
                    MessageBox.Show("Name field cannot be empty.");
                    return;
                }              

                if (decimal.TryParse(priceTextBox.Text, out decimal priceVal) && priceVal > 0)
                {
                    price = priceVal;
                }
                else
                {
                    MessageBox.Show("Price field requires a positive number.");
                    return;
                }

                if (int.TryParse(inventoryTextBox.Text, out int invVal) && (invVal >= 1))
                {
                    instock = invVal;
                }
                else
                {
                    MessageBox.Show("Inventory field requires a positive whole number.");
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
    }
}
