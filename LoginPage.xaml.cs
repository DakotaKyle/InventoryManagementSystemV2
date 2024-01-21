using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
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
            DataTable userTable = new();
            String userName, Password, Salt;
            MySqlCommand userData = new("SELECT user_name, users_password, salt FROM users", connection);

            try
            {
                /*
                 * open the connection and load the data into the userTable for validation.
                 */
                connection.Open();

                userTable.Load(userData.ExecuteReader());

                connection.Close();

                foreach (DataRow row in userTable.Rows)
                {
                    userName = row["user_name"].ToString();
                    Password = row["users_password"].ToString();
                    Salt = row["salt"].ToString();

                    // Hash the provided password with the retrieved salt
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            builder.Append(bytes[i].ToString("x2"));
                        }
                        password = builder.ToString();
                    }

                    if (username == userName && password == Password)
                    {
                        isvalid = true;
                        return;
                    }
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
            finally
            {
                connection.Close();
            }
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
