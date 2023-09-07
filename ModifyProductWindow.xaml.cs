using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ModifyProductWindow : Window
    {
        public BindingList<Part> NewParts = new();
        Product oldProduct;

        public ModifyProductWindow()
        {
            InitializeComponent();

            AllPartsDataGrid.ItemsSource = Inventory.allParts;
        }

        public ModifyProductWindow(Product product)
        {
            InitializeComponent();
            
            AllPartsDataGrid.ItemsSource = Inventory.allParts;
            ProductDataGrid.ItemsSource = product.associatedPart;

            IdTextBox.Text = product.ProductID.ToString();
            NameTextBox.Text = product.Name;
            InventoryTextBox.Text = product.Instock.ToString();
            PriceTextBox.Text = product.Price.ToString();
            MaxTextBox.Text = product.Max.ToString();
            MinTextBox.Text = product.Min.ToString();

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
            int id, instock, min, max;
            string name;
            decimal price;

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

            if (int.TryParse(MinTextBox.Text, out int minVal) && minVal >= 1)
            {
                min = minVal;
            }
            else
            {
                MessageBox.Show("Minimum value field requires a positive number.");
                return;
            }

            if (int.TryParse(MaxTextBox.Text, out int maxVal) && maxVal >= 1)
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

            if (int.TryParse(InventoryTextBox.Text, out int invVal) && (invVal >= 1))
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

            Inventory.removeProduct(oldProduct);
            Product product = new(id, name, instock, price, max, min);
            Inventory.AddProduct(product);

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
    }
}
