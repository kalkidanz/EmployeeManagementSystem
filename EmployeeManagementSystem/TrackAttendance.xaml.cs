using EmployeeManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeManagementSystem
{
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
            if (!EmployeeExists(employeeID))
            {
                MessageBox.Show("Employee ID not found. Please enter a valid Employee ID.");
                lvAttendance.ItemsSource = null;
                LoadAllAttendance();
                txtEmployeeID.Clear();
                return;
            }

            List<Attendance> attendanceList = new List<Attendance>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeID, Date, Status FROM Attendance WHERE EmployeeID = @EmployeeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
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

            lvAttendance.ItemsSource = attendanceList.Count > 0 ? attendanceList : new List<Attendance>();
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

            lvAttendance.ItemsSource = attendanceList.Count > 0 ? attendanceList : new List<Attendance>();
        }

        private bool EmployeeExists(string employeeID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Employee WHERE EmployeeID = @EmployeeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private void MarkAttendance(string status)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please enter an Employee ID.");
                return;
            }

            string employeeID = txtEmployeeID.Text;
            if (!EmployeeExists(employeeID))
            {
                MessageBox.Show("Employee ID not found. Cannot mark attendance.");
                return;
            }

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM Attendance WHERE EmployeeID = @EmployeeID AND Date = @Date";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                checkCommand.Parameters.AddWithValue("@Date", date);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                
                    string updateQuery = "UPDATE Attendance SET Status = @Status WHERE EmployeeID = @EmployeeID AND Date = @Date";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                    updateCommand.Parameters.AddWithValue("@Date", date);
                    updateCommand.Parameters.AddWithValue("@Status", status);
                    updateCommand.ExecuteNonQuery();

                    MessageBox.Show($"Attendance updated to {status} successfully.");
                }
                else
                {
                    string insertQuery = "INSERT INTO Attendance (EmployeeID, Date, Status) VALUES (@EmployeeID, @Date, @Status)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                    insertCommand.Parameters.AddWithValue("@Date", date);
                    insertCommand.Parameters.AddWithValue("@Status", status);
                    insertCommand.ExecuteNonQuery();

                    MessageBox.Show($"Marked as {status} successfully.");
                }

                LoadAttendance(employeeID);
                LoadAllAttendance();
                txtEmployeeID.Clear();
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
