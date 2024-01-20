using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InventoryManagementSystem
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        /*
         * Create the connection string.
         */
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
            /*
             * Authenticates the username and password.
             */
            int i = 0;
            DataTable userTable = new();
            String userName, Password;
            MySqlCommand userData = new("SELECT user_name, users_password FROM users", connection);

            try
            {
                /*
                 * open the connection and load the data into the userTable for validation.
                 */
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
            /*
             * Attempts to authenticate user when login button is clicked.
             */
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
