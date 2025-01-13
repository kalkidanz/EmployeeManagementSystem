using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected role
                string role = ((ComboBoxItem)cmbRole.SelectedItem)?.Content?.ToString();
                string username = txtUsername.Text;
                string password = txtPassword.Password;

                // Validate inputs
                if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Authenticate the user
                if (AuthenticateUser(username, password, role))
                {
                    if (role == "Admin")
                    {
                        MessageBox.Show("Welcome, Admin!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        AdminDashboard adminDashboard = new AdminDashboard();
                        adminDashboard.Show();
                        Application.Current.MainWindow = adminDashboard; // Set new main window
                    }
                    else if (role == "User")
                    {
                        MessageBox.Show("Welcome, User!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        UserDashboard userDashboard = new UserDashboard();
                        userDashboard.Show();
                        Application.Current.MainWindow = userDashboard; // Set new main window
                    }
                    this.Close(); // Close current window
                }
                else
                {
                    txtMessage.Text = "Invalid username, password, or role!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string username, string password, string role)
        {
            bool isValid = false;
            string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password AND Role = @Role";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Replace with hashed password in production
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    isValid = userCount > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isValid;
        }
    }
}
