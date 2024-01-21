using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    public partial class ModifyProductWindow : Window
    {
        public BindingList<Part> NewParts = new();
        Product oldProduct;
        Inventory inv = new();

        public ModifyProductWindow()
        {
            InitializeComponent();

            AllPartsDataGrid.ItemsSource = Inventory.allParts;
        }

        public ModifyProductWindow(Product product)
        {
            InitializeComponent();
            
            AllPartsDataGrid.ItemsSource = Inventory.allParts;

            if (product.associatedPart.Count.Equals(0))
            {
                product.InitAsp(product);
            }

            ProductDataGrid.ItemsSource = product.associatedPart;

            IdTextBox.Text = product.ProductID.ToString();
            NameTextBox.Text = product.Name;
            InventoryTextBox.Text = product.Instock.ToString();
            PriceTextBox.Text = product.Price.ToString();
            Date_Picker.SelectedDate = product.MadeOn;
            timeTextBox.Text = product.MadeOn.ToShortTimeString();

            oldProduct = product;
            NewParts = product.associatedPart;
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
            string name;
            decimal price;
            DateTime date;

            id = int.Parse(IdTextBox.Text);

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

            date = (DateTime)Date_Picker.SelectedDate;

            
            Product product = new(id, name, instock, price, date);
            inv.updateProduct(product, NewParts);
            Inventory.products.Add(product);
            Inventory.products.Remove(oldProduct);

            foreach (Part part in NewParts)
            {
                product.addAssociatedPart(part);
            }

            MessageBox.Show("Product has been modified.");
            Close();
        }

        private void AllPartsAddButton_Click(object sender, RoutedEventArgs e)
        {

            if (AllPartsDataGrid.SelectedItem != null)
            {
                Part addPart = ((Part)AllPartsDataGrid.SelectedItem);
                NewParts.Add(addPart);

                ProductDataGrid.ItemsSource = NewParts;
            }
            else
            {
                MessageBox.Show("Select a part to add it to your product.");
            }
        }

        private void ProductSearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (int.TryParse(ProductSearchTextBox.Text, out int search) && search >= 1)
            {

                Part match = oldProduct.LookUpAssociatedPart(search);

                try
                {
                    ProductDataGrid.ScrollIntoView(match);
                    ProductDataGrid.SelectedItem = match;
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

        private void ProductDeleteButton_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this part from this product? " +
                "This action cannot be undone.", "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                oldProduct.removeAssociatedPart((Part)ProductDataGrid.SelectedItem);
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
