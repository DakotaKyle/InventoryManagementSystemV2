using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InventoryManagementSystem
{
    public partial class ModifyPartWindow : Window
    {

        Part oldPart;

        public ModifyPartWindow(Inhouse inhousePart)
        {
            InitializeComponent();
            inHouseButton.IsChecked = true;

            idTextBox.Text = inhousePart.PartID.ToString();
            nameTextBox.Text = inhousePart.Name;
            inventoryTextBox.Text = inhousePart.Instock.ToString();
            priceTextBox.Text = inhousePart.Price.ToString();
            maxTextBox.Text = inhousePart.Max.ToString();
            minTextBox.Text = inhousePart.Min.ToString();
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
            maxTextBox.Text = outsourcePart.Max.ToString();
            minTextBox.Text = outsourcePart.Min.ToString();
            machineTextBox.Text = outsourcePart.CompanyName;

            oldPart = outsourcePart;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            int id, instock, min, max, machine;
            String name, companyID;
            decimal price;

            id = int.Parse(idTextBox.Text);

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

            if (int.TryParse(minTextBox.Text, out int minVal) && minVal >= 1)
            {
                min = minVal;
            }
            else
            {
                MessageBox.Show("Minimum value field requires a positive number.");
                return;
            }

            if (int.TryParse(maxTextBox.Text, out int maxVal) && maxVal >= 1)
            {
                if (maxVal >= minVal)
                {
                    max = maxVal;
                }
                else
                {
                    MessageBox.Show("Maximum value cannot be less than minimum.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Maximum value field requires a positive number.");
                return;
            }

            if (int.TryParse(inventoryTextBox.Text, out int invVal) && (invVal >= 1))
            {
                if (invVal > max)
                {
                    MessageBox.Show("Inventory cannot exceed maximum value.");
                    return;
                }
                else if (invVal < min)
                {
                    MessageBox.Show("Inventory cannot be less than minimum value.");
                    return;
                }

                instock = invVal;
            }
            else
            {
                MessageBox.Show("Inventory field requires a positive whole number.");
                return;
            }

            if ((bool)outsourced.IsChecked)
            {
                if (machineTextBox.Text.Length != 0)
                {
                    companyID = machineTextBox.Text;

                    Inventory.DeletePart(oldPart);
                    OutSourced source = new(id, name, instock, price, max, min, companyID);
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

                    Inventory.DeletePart(oldPart);
                    Inhouse homemade = new(id, name, instock, price, max, min, machine);
                    Inventory.AddPart(homemade);
                }
                else
                {
                    MessageBox.Show("Machine ID must be a postive number");
                    return;
                }
            }

            MessageBox.Show("Part has been modified.");
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
    }
}
