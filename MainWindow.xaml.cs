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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventoryManagementSystem
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            PartDataGrid.ItemsSource = Inventory.allParts;
            ProductDataGrid.ItemsSource = Inventory.products;
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            AddPartWindow newPart = new();
            newPart.ShowDialog();         
        }

        private void partSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(partSeachField.Text, out int search) && search >= 1)
            {
                Part match = Inventory.LookupPart(search);

                try
                {
                    PartDataGrid.ScrollIntoView(match);
                    PartDataGrid.SelectedItem = match;
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

        private void ProductSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SearchProductField.Text, out int search) && search >= 1)
            {
                Product match = Inventory.lookupProduct(search);

                try
                {
                    ProductDataGrid.ScrollIntoView(match);
                    ProductDataGrid.SelectedItem = match;
                }
                catch (Exception)
                {
                    MessageBox.Show("Product ID not found.");
                }
            }
            else
            {
                MessageBox.Show("Enter Product ID");
                return;
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow newProduct = new();
            newProduct.ShowDialog();
        }

        private void ProductModifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDataGrid.SelectedItem != null)
            {
                Product modify = (Product)ProductDataGrid.SelectedItem;

                ModifyProductWindow modifyProduct = new(modify);
                modifyProduct.ShowDialog();
            }
            else
            {
                MessageBox.Show("Select a Product to modify it.");
            }
        }

        private void ProductDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this part? This action cannot be undone.",
                "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Product product = (Product)ProductDataGrid.SelectedItem;

                if (product.associatedPart.Count > 0)
                {
                    MessageBox.Show("Product cannot be deleted with Parts attached. Please remove parts and try again.");
                    return; 
                }
                else
                {
                    Inventory.removeProduct(product);
                }
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeletePartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this part? This action cannot be undone.",
                "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Inventory.DeletePart((Part)PartDataGrid.SelectedItem);
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }            
        }

        private void ModifyPartButton_Click(object sender, RoutedEventArgs e)
        {

            if (PartDataGrid.SelectedItem != null)
            {
                Part modify = ((Part)PartDataGrid.SelectedItem);

                if (modify.GetType().ToString() == "InventoryManagementSystem.Inhouse")
                {
                    Inhouse inhouse = (Inhouse)modify;
                    ModifyPartWindow modifyPart = new(inhouse);

                    modifyPart.ShowDialog();

                }
                else if (modify.GetType().ToString() == "InventoryManagementSystem.OutSourced")
                {
                    OutSourced outsource = (OutSourced)modify;
                    ModifyPartWindow modifyPart = new(outsource);

                    modifyPart.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Select a part to modify it.");
            }
        }

        private void PartSeachField_GotFocus(object sender, RoutedEventArgs e)
        {
            partSeachField.Clear();
        }

        private void SearchProductField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchProductField.Clear();
        }
    }
}
