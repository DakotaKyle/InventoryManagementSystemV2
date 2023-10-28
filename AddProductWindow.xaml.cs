using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{

    public partial class AddProductWindow : Window
    {
        public BindingList<Part> NewParts = new();

        public AddProductWindow()
        {
            InitializeComponent();

            AllPartsDataGrid.ItemsSource = Inventory.allParts;
            Random ranID = new();
            IdTextBox.Text = ranID.Next().ToString();
        }

        private void AllPartSearchButton_Click(object sender, RoutedEventArgs e)
        {
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
            int id, instock;
            string name, timeString;
            decimal price;
            DateTime date;

            id = int.Parse(IdTextBox.Text);
            timeString = Date_Picker.Text + " " + timeTextBox.Text;

            if (NameTextBox.Text.Length != 0)
            {
                name = NameTextBox.Text;
            }
            else
            {
                MessageBox.Show("Name field cannot be empty.");
                return;
            }

            if (decimal.TryParse(PriceTextBox.Text, out decimal priceVal) && priceVal > 0)
            {
                price = priceVal;
            }
            else
            {
                MessageBox.Show("Price field requires a positive number.");
                return;
            }

            if (int.TryParse(InventoryTextBox.Text, out int invVal) && (invVal >= 1))
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

            Product product = new(id, name, instock, price, date);
            Inventory.AddProduct(product);

            foreach (Part part in NewParts)
            {
             //   product.addAssociatedPart(part);
            }

            MessageBox.Show("Product has been added to inventory.");
            Close();
        }

        private void AllPartsAddButton_Click(object sender, RoutedEventArgs e)
        {

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
