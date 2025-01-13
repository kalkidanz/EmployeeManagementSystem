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
using System.Windows.Shapes;
using EmployeeManagementSystem.Model;

namespace EmployeeManagementSystem
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";

        private void btnManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            manageEmployee manageEmployeesControl = new manageEmployee();
            ContentArea.Children.Clear();
            ContentArea.Children.Add(manageEmployeesControl);

        }

        private void btnTrackAttendance_Click(object sender, RoutedEventArgs e)
        {
            TrackAttendance manageEmployeesControl = new TrackAttendance();
            ContentArea.Children.Clear();
            ContentArea.Children.Add(manageEmployeesControl);

        }

        private void btnCalculateSalaries_Click(object sender, RoutedEventArgs e)
        {
            CalculateSalary manageEmployeesControl = new CalculateSalary();
            ContentArea.Children.Clear();
            ContentArea.Children.Add(manageEmployeesControl);
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
