using System;
using System.Windows;
using InventoryManagementSystem.Database_Service;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    /*
     * This class is responsible for ensuring user validation, initializing the part and product binding lists, and providing an interface to all other windows.
     */
    public partial class MainWindow : Window
    {
        private static readonly LoginPage login = new();
        private Inventory inv = new();
        private Product product = new();

        public MainWindow()
        {
            InitializeComponent();
            /*
             * initialize all binding lists and populate the main screen datagrids.
             */
            inv.initPart();
            inv.initProduct();
            PartDataGrid.ItemsSource = Inventory.allParts;
            ProductDataGrid.ItemsSource = Inventory.products;
            /*
             * Disable the main screen and load the login page.
             */
            try
            {
                while (!LoginPage.isvalid)
                {
                    Hide();
                    login.ShowDialog();

                    if (LoginPage.isvalid)
                    {
                        Show();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Close();
            }
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            //Creates an add part button
            AddPartWindow newPart = new();
            newPart.ShowDialog();         
        }

        private void partSearchButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * A search function for parts.
             */
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
            /*
             * A search function for products.
             */
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
            //an add product button
            AddProductWindow newProduct = new();
            newProduct.ShowDialog();
        }

        private void ProductModifyButton_Click(object sender, RoutedEventArgs e)
        {
            //a modify product button. The next screen will not load if a product is not selected.
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
            //This method deletes the product from the data table as long as no associated parts are attached.
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
            //an exit button.
            Close();
        }

        private void DeletePartButton_Click(object sender, RoutedEventArgs e)
        {
            //Deletes parts from inventory.
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this part? This action cannot be undone.",
                "", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                inv.DeletePart((Part)PartDataGrid.SelectedItem);
            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }            
        }

        private void ModifyPartButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * This method will open the modify part scrren as long as a part is selected.
             */

            if (PartDataGrid.SelectedItem != null)
            {
                Part modify = ((Part)PartDataGrid.SelectedItem);

                if (modify.GetType().ToString() == "InventoryManagementSystem.Models.Inhouse")
                {
                    Inhouse inhouse = (Inhouse)modify;
                    ModifyPartWindow modifyPart = new(inhouse);

                    modifyPart.ShowDialog();

                }
                else if (modify.GetType().ToString() == "InventoryManagementSystem.Models.OutSourced")
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
            //Clears the part field on focus
            partSeachField.Clear();
        }

        private void SearchProductField_GotFocus(object sender, RoutedEventArgs e)
        {
            //Clears the product field on focus.
            SearchProductField.Clear();
        }

        private void Report_Button_Click(object sender, RoutedEventArgs e)
        {
            //opens the report page.
            ReportWindow reports = new();
            reports.ShowDialog();
        }
    }
}
