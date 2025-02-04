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
                
                string username = txtUsername.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (AuthenticateUser(username, password))
                {
                   
                    {
                        MessageBox.Show("Welcome, Admin!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        AdminDashboard adminDashboard = new AdminDashboard();
                        adminDashboard.Show();
                    }
                    
                    this.Close(); 
                }
                else
                {
                    txtMessage.Text = "Invalid username, password";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            bool isValid = false;
            string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password ";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); 
                   

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
