using EmployeeManagementSystem.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using System;
using EmployeeManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeManagementSystem
{
    public partial class manageEmployee : UserControl
    {
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";


        public manageEmployee()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employee";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        EmployeeID = reader["EmployeeID"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        PhoneNo = reader["PhoneNo"].ToString(),
                        Department = reader["Department"].ToString(),
                        Position = reader["Position"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }
            }
            lvEmployees.ItemsSource = employees;
        }
        private void ClearInputs()
        {
            txtEmployeeID.Clear();
            txtFullName.Clear();
            rbMale.IsChecked = false;
            rbFemale.IsChecked = false;
            txtPhoneNumber.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbposition.SelectedIndex = -1;
            txtSalary.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmployeeID.Text) || string.IsNullOrWhiteSpace(txtFullName.Text)||
                     (rbMale.IsChecked == false && rbFemale.IsChecked == false)||
                      string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || cmbDepartment.SelectedItem == null||
                        string.IsNullOrWhiteSpace(cmbposition.Text) || string.IsNullOrWhiteSpace(txtSalary.Text))
                {

                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                string employeeID = txtEmployeeID.Text;
                string fullName = txtFullName.Text;
                string gender = rbMale.IsChecked == true ? "Male" : "Female";
                string phoneNo = txtPhoneNumber.Text;
                string department = ((ComboBoxItem)cmbDepartment.SelectedItem).Content.ToString();
                string position = cmbposition.Text;
                decimal salary;

                if (phoneNo.Length > 10 || phoneNo.Length < 10)
                {
                    MessageBox.Show("phoneNo length should be 10");
                    return;

                }
if (decimal.TryParse(txtSalary.Text, out salary))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Employee (EmployeeID, FullName, Gender, PhoneNo, Department, Position, Salary) VALUES (@EmployeeID, @FullName, @Gender, @PhoneNo, @Department, @Position, @Salary)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@Gender", gender);
                        command.Parameters.AddWithValue("@PhoneNo", phoneNo);
                        command.Parameters.AddWithValue("@Department", department);
                        command.Parameters.AddWithValue("@Position", position);
                        command.Parameters.AddWithValue("@Salary", salary);

                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Employee added successfully.");
                        LoadEmployees();
                        ClearInputs();
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid salary.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchId = txtEmployeeID.Text;
            if (!string.IsNullOrWhiteSpace(searchId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Employee WHERE EmployeeID = @EmployeeID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", searchId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        txtFullName.Text = reader["FullName"].ToString();
                        string gender = reader["Gender"].ToString();
                        if (gender == "Male") rbMale.IsChecked = true;
                        else rbFemale.IsChecked = true;
                        txtPhoneNumber.Text = reader["PhoneNo"].ToString();
                        cmbDepartment.Text = reader["Department"].ToString();
                        cmbposition.Text = reader["Position"].ToString();
                        txtSalary.Text = reader["Salary"].ToString();

                    }
                    else
                    {
                        MessageBox.Show("Employee not found.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter an Employee ID to search.");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
                {
                    MessageBox.Show("Please select an Employee ID to update.");
                    return;
                }
                string employeeID = txtEmployeeID.Text;
                string fullName = txtFullName.Text;
                string gender = rbMale.IsChecked == true ? "Male" : "Female";
                string phoneNo = txtPhoneNumber.Text;
                string department = ((ComboBoxItem)cmbDepartment.SelectedItem).Content.ToString();
                string position = cmbposition.Text;
                decimal salary;

if (decimal.TryParse(txtSalary.Text, out salary))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Employee SET FullName = @FullName, Gender = @Gender, PhoneNo = @PhoneNo, Department = @Department, Position = @Position, Salary = @Salary WHERE EmployeeID = @EmployeeID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@Gender", gender);
                        command.Parameters.AddWithValue("@PhoneNo", phoneNo);
                        command.Parameters.AddWithValue("@Department", department);
                        command.Parameters.AddWithValue("@Position", position);
                        command.Parameters.AddWithValue("@Salary", salary);

                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Employee updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                        ClearInputs();

                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid salary.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
                {
                    MessageBox.Show("Please enter an Employee ID to delete.");
                    return;
                }

                string employeeID = txtEmployeeID.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID ";
                    string deleteAttendQuery = "DELETE FROM Attendance WHERE EmployeeID = @EmployeeID";
                    SqlCommand deletePayrollCommand = new SqlCommand(deleteAttendQuery, connection);
                    deletePayrollCommand.Parameters.AddWithValue("@EmployeeID", employeeID);

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    connection.Open();
                    deletePayrollCommand.ExecuteNonQuery();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Employee ID not found.");
                    }
                    LoadEmployees();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
private void lvEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvEmployees.SelectedItem != null)
            {
                Employee selectedEmployee = lvEmployees.SelectedItem as Employee;
                txtEmployeeID.Text = selectedEmployee.EmployeeID;
                txtFullName.Text = selectedEmployee.FullName;
                if (selectedEmployee.Gender == "Male")
                {
                    rbMale.IsChecked = true;
                }
                else
                {
                    rbFemale.IsChecked = true;
                }
                txtPhoneNumber.Text = selectedEmployee.PhoneNo;
                cmbDepartment.Text = selectedEmployee.Department;
                cmbposition.Text = selectedEmployee.Position;
                txtSalary.Text = selectedEmployee.Salary.ToString();
            }
        }

        private void cmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbDepartment = sender as ComboBox;
            ComboBox cmbPosition = cmbposition;

            cmbPosition.Items.Clear();

            if (cmbDepartment.SelectedItem != null)
            {
                string selectedDepartment = (cmbDepartment.SelectedItem as ComboBoxItem).Content.ToString();
                switch (selectedDepartment)
                {

                    case "Marketing":
                        cmbPosition.Items.Add("Marketing Director");
                        cmbPosition.Items.Add("Marketing Manager");
                        cmbPosition.Items.Add("Social Media Coordinator");
                        cmbPosition.Items.Add("SEO Specialist");
                        cmbPosition.Items.Add("Content Writer");
                        cmbPosition.Items.Add("Digital Marketing Strategist");
                        break;
                    case "Finance":
                        cmbPosition.Items.Add("Chief Financial Officer (CFO)");
                        cmbPosition.Items.Add("Financial Analyst");
                        cmbPosition.Items.Add("Accounting Manager");
                        cmbPosition.Items.Add("Tax Specialist");
                        cmbPosition.Items.Add("Accounts Payable/Receivable Clerk");
                        cmbPosition.Items.Add("Payroll Coordinator");
                        break;
                    case "Information Technology (IT)":
                        cmbPosition.Items.Add("IT Manager");
                        cmbPosition.Items.Add("Systems Administrator");
                        cmbPosition.Items.Add("Network Engineer");
                        cmbPosition.Items.Add("Web Developer");
                        cmbPosition.Items.Add("IT Support Specialist");
                        cmbPosition.Items.Add("Cybersecurity Analyst");
                        break;
                    case "Sales":
                        cmbPosition.Items.Add("Sales Director");
                        cmbPosition.Items.Add("Account Executive");
                        cmbPosition.Items.Add("Sales Representative");
                        cmbPosition.Items.Add("Sales Coordinator");
                        cmbPosition.Items.Add("Business Development Manager");
                        cmbPosition.Items.Add("Sales Analyst");
                        break;
                    case "Operations":
                        cmbPosition.Items.Add("Operations Manager");
                        cmbPosition.Items.Add("Supply Chain Coordinator");
                        cmbPosition.Items.Add("Inventory Manager");
                        cmbPosition.Items.Add("Logistics Manager");
                        cmbPosition.Items.Add("Production Supervisor");
                        cmbPosition.Items.Add("Operations Analyst");
                        break;
                    case "Customer Service":
                        cmbPosition.Items.Add("Customer Service Manager");
                        cmbPosition.Items.Add("Customer Support Representative");
                        cmbPosition.Items.Add("Call Center Supervisor");
                        cmbPosition.Items.Add("Help Desk Technician");
                        cmbPosition.Items.Add("Client Relationship Manager");
                        cmbPosition.Items.Add("Customer Success Manager");
                        break;
                    case "Product Development":
                        cmbPosition.Items.Add("Product Manager");
                        cmbPosition.Items.Add("Product Designer");
                        cmbPosition.Items.Add("UX/UI Designer");
                        cmbPosition.Items.Add("Product Development Specialist");
                        cmbPosition.Items.Add("Quality Assurance (QA) Tester");
                        cmbPosition.Items.Add("R&D Engineer");
                        break;
                    case "Creative":
                        cmbPosition.Items.Add("Creative Director");
                        cmbPosition.Items.Add("Graphic Designer");
                        cmbPosition.Items.Add("Copywriter");
                        cmbPosition.Items.Add("Video Producer");
                        cmbPosition.Items.Add("Art Director");
                        cmbPosition.Items.Add("Photographer");
                        break;
                }
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }
    }
}