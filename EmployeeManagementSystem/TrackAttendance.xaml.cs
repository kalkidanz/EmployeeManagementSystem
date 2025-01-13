using EmployeeManagementSystem.Model;
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
    /// Interaction logic for TrackAttendance.xaml
    /// </summary>
    public partial class TrackAttendance : UserControl
    {
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";
        
        public TrackAttendance()
        {
            InitializeComponent();
            LoadAllAttendance();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please enter an Employee ID to search.");
                return;
            }

            LoadAttendance(txtEmployeeID.Text);
        }
        private void LoadAttendance(string employeeID)
        {
            List<Attendance> attendanceList = new List<Attendance>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeID ,Date, Status FROM Attendance WHERE EmployeeID = @EmployeeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    attendanceList.Add(new Attendance
                    {
                        EmployeeID = reader["txtEmployeeID"].ToString(),
                        Date = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd"),
                        Status = reader["Status"].ToString()
                    });
                }
            }
            if (attendanceList.Count > 0)
            {
                lvAttendance.ItemsSource = attendanceList;
            }
            else
            {
                MessageBox.Show("No attendance records found for this Employee ID.");
                lvAttendance.ItemsSource = null;
            }
   }
        private void LoadAllAttendance()
        {
            List<Attendance> attendanceList = new List<Attendance>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeID, Date, Status FROM Attendance";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    attendanceList.Add(new Attendance
                    {
                        EmployeeID = reader["EmployeeID"].ToString(),
                        Date = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd"),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            lvAttendance.ItemsSource = attendanceList;
        }
     
        private void MarkAttendance(string status)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please enter an Employee ID.");
                return;
            }

            string employeeID = txtEmployeeID.Text;
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Attendance (EmployeeID, Date, Status) VALUES (@EmployeeID, @Date, @Status)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                command.Parameters.AddWithValue("@Date", date);
                command.Parameters.AddWithValue("@Status", status);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show($"Marked as {status} successfully.");
                LoadAttendance(employeeID);
            }
        }
        private void btnMarkPresent_Click(object sender, RoutedEventArgs e)
        {
            MarkAttendance("Present");
        }

        private void btnMarkAbsent_Click(object sender, RoutedEventArgs e)
        {
            MarkAttendance("Absent");
        }
    }
}
