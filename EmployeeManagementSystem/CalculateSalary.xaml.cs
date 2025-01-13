using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace EmployeeManagementSystem
{
    /// <summary>
    /// Interaction logic for CalculateSalary.xaml
    /// </summary>
    public partial class CalculateSalary : UserControl
    {
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";
        public CalculateSalary()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please enter an Employee ID.");
                return;
            }

            string employeeID = txtEmployeeID.Text;
            CalculateTotalSalary(employeeID);
        }

        private void CalculateTotalSalary(string employeeID)
        {
            decimal baseSalary = 0;
            int presentDays = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Get the base salary of the employee
                string salaryQuery = "SELECT Salary FROM Employee WHERE EmployeeID = @EmployeeID";
                SqlCommand salaryCommand = new SqlCommand(salaryQuery, connection);
                salaryCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                connection.Open();

                object result = salaryCommand.ExecuteScalar();
                if (result != null)
                {
                    baseSalary = Convert.ToDecimal(result);
                }
                else
                {
                    MessageBox.Show("Employee not found.");
                    return;
                }

                connection.Close();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Count the number of present days from the attendance records
                string attendanceQuery = "SELECT COUNT(*) FROM Attendance WHERE EmployeeID = @EmployeeID AND Status = 'Present'";
                SqlCommand attendanceCommand = new SqlCommand(attendanceQuery, connection);
                attendanceCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                connection.Open();

                presentDays = (int)attendanceCommand.ExecuteScalar();
                connection.Close();
            }

            decimal dailySalary = baseSalary / 30; // Assuming 30 days in a month
            decimal totalSalary = dailySalary * presentDays;

            txtResult.Text = $"Employee ID: {employeeID}\nBase Salary: {baseSalary:C}\nDays Present: {presentDays}\nTotal Salary: {totalSalary:C}";
        }
    }
}
   
