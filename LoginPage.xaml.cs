using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Globalization;
using System.IO;

namespace InventoryManagementSystem
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private static String connectionString = "Host=localhost;Port=3306;Database=duco_db;Username=root;Password=password";
        private MySqlConnection connection = new(connectionString);
        private string username, password;
        public static bool isvalid { get; set; }

        public LoginPage()
        {
            InitializeComponent();
        }

        private void authenticate()
        {

            int i = 0;
            DataTable userTable = new();
            String userName, Password;
            MySqlCommand userData = new("SELECT user_name, users_password FROM users", connection);

            try
            {
                connection.Open();

                userTable.Load(userData.ExecuteReader());

                connection.Close();

                foreach (DataRow row in userTable.Select().Where(e => e.ToString().Any()))//Lamba expression to get username and passwords from the data table.
                {
                    userName = userTable.Rows[i]["user_name"].ToString();
                    Password = userTable.Rows[i]["users_password"].ToString();

                    if (username == userName && password == Password)
                    {
                        isvalid = true;
                        return;
                    }
                    i++;
                }

                if (!isvalid)
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                connection.Dispose();
            }
            finally { connection.Close(); }
        }

        private void Loginbuton_Click(object sender, RoutedEventArgs e)
        {
            username = UsernameTextbox.Text;
            password = PasswordBox.Password.ToString();

            authenticate();

            if (isvalid)
            {
                Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
