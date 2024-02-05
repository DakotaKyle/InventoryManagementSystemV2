using InventoryManagementSystem.Database_Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventoryManagementSystem
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            IDColumn.Header = "IDColumn";
        }

        Dictionary<string, string> shiftTimes = new Dictionary<string, string> // A dictionary to hold time slots based on the selected shift.
        {
            { "1st", "6:00am - 2:00pm" },
            { "2nd", "2:00pm - 10:00pm" },
            { "3rd", "10:00pm - 6:00am" }
        };

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected shift
            var selectedShift = (ShiftComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // Look up the time slot
            if (shiftTimes.TryGetValue(selectedShift, out string timeSlot))
            {
                // Split the timeSlot into start and end times
                string[] times = timeSlot.Split('-');
                DateTime startDate = ReportDatePicker.SelectedDate.Value.ToUniversalTime();
                DateTime endDate = startDate;

                // Parse the times to a DateTime object and then format it to "HH:mm:ss"
                DateTime startTimeObj = DateTime.Parse(times[0].Trim());
                DateTime endTimeObj = DateTime.Parse(times[1].Trim());

                // If the end time is less than the start time, it means the shift passed midnight and we should add a day to the end date
                if (endTimeObj < startTimeObj)
                {
                    endDate = endDate.AddDays(1);
                }

                DateTime startTime = startDate.Date + startTimeObj.TimeOfDay;
                DateTime endTime = endDate.Date + endTimeObj.TimeOfDay;
                endTime = endTime.AddSeconds(1);

                if (QueryComboBox.SelectedIndex == 0)
                {
                    // Query the allParts list
                    var parts = Inventory.allParts.Where(part => part.ArrivedOn >= startTime && part.ArrivedOn < endTime).ToList();

                    // Update the data grid
                    ReportsDataGrid.ItemsSource = parts;
                }
                else if (QueryComboBox.SelectedIndex == 1)
                {
                    // Query the products list
                    var products = Inventory.products.Where(part => part.MadeOn >= startTime && part.MadeOn < endTime).ToList();

                    // Update the data grid
                    ReportsDataGrid.ItemsSource = products;
                }
            }
        }

        private void QueryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IDColumn != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)QueryComboBox.SelectedItem;
                if (selectedItem.Content.ToString() == "Product")
                {
                    IDColumn.Header = "Product ID";
                    IDColumn.Binding = new Binding("ProductID");
                    dt.Binding = new Binding("MadeOn");
                    ReportsDataGrid.ItemsSource = Inventory.products;
                }
                else if (selectedItem.Content.ToString() == "Part")
                {
                    IDColumn.Header = "Part ID";
                    IDColumn.Binding = new Binding("PartID");
                    dt.Binding = new Binding("ArrivedOn");
                    ReportsDataGrid.ItemsSource = Inventory.allParts;
                }
            }
        }
    }
}
