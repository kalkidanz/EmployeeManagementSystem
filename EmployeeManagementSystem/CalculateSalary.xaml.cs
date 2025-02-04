using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ClosedXML.Excel;
using EmployeeManagementSystem.Model;
using DocumentFormat.OpenXml.Drawing.Diagrams;


namespace EmployeeManagementSystem
{
    public partial class CalculateSalary : UserControl
    {
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";

        public CalculateSalary()
        {
            InitializeComponent();
            LoadEmployeeIDs();


            LoadPayrollSummary();
        }
        private void LoadEmployeeIDs()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT EmployeeID FROM Employee";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        cmbEmployeeID.Items.Add(reader["EmployeeID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Employee IDs: " + ex.Message);
            }
        }
        private void cmbEmployeeID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEmployeeID.SelectedItem != null)
            {
                FetchEmployeeData(cmbEmployeeID.SelectedItem.ToString());
            }
        }


        private void LoadPayrollSummary()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Payroll", connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                PayrollSummary.ItemsSource = dt.DefaultView;
            }
        }

        private void FetchEmployeeData(string employeeID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string salaryQuery = "SELECT Salary FROM Employee WHERE EmployeeID = @EmployeeID";
                    SqlCommand salaryCmd = new SqlCommand(salaryQuery, connection);
                    salaryCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    object salaryResult = salaryCmd.ExecuteScalar();

                    if (salaryResult == null)
                    {
                        MessageBox.Show("Employee not found.");
                        ClearInputs();
                        return;
                    }

                    decimal baseSalary = Convert.ToDecimal(salaryResult);
                    txtSalary.Text = baseSalary.ToString();


                    string presentDaysQuery = "SELECT COUNT(*) FROM Attendance WHERE EmployeeID = @EmployeeID AND Status = 'Present'";
                    SqlCommand presentCmd = new SqlCommand(presentDaysQuery, connection);
                    presentCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    int presentDays = (int)presentCmd.ExecuteScalar();
                    txtPresentDays.Text = presentDays.ToString();

                    if (baseSalary > 5000)
                    {
                        decimal bonus = baseSalary * 0.05m;
                        decimal deductions = baseSalary * 0.07m;


                        txtBonus.Text = bonus.ToString();
                        txtDeductions.Text = deductions.ToString();
                    }
                    else
                    {
                        decimal bonus = baseSalary * 0.07m;
                        decimal deductions = baseSalary * 0.03m;


                        txtBonus.Text = bonus.ToString();
                        txtDeductions.Text = deductions.ToString();

                    }
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtSalary.Text, out decimal baseSalary) ||
        !int.TryParse(txtPresentDays.Text, out int presentDays) ||
        !decimal.TryParse(txtBonus.Text, out decimal bonus) ||
        !decimal.TryParse(txtDeductions.Text, out decimal deductions))
            {
                MessageBox.Show("Please enter valid numbers.");
                return;
            }

            decimal dailySalary = baseSalary / 30;
            decimal totalSalary = (dailySalary * presentDays) + bonus - deductions;
            txtNetSalary.Text = totalSalary.ToString("F2");
        }

        private void btnSavePayroll_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtNetSalary.Text, out decimal netSalary))
            {
                MessageBox.Show("Calculate salary before saving.");
                return;
            }

            string employeeID = cmbEmployeeID.Text;
            int presentDays = int.Parse(txtPresentDays.Text);
            decimal baseSalary = decimal.Parse(txtSalary.Text);
            decimal bonus = decimal.Parse(txtBonus.Text);
            decimal deductions = decimal.Parse(txtDeductions.Text);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Payroll (EmployeeID, Salary, PresentDays, Bonus, Deductions, NetSalary) VALUES (@EmployeeID, @Salary, @PresentDays, @Bonus, @Deductions, @NetSalary)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                command.Parameters.AddWithValue("@Salary", baseSalary);
                command.Parameters.AddWithValue("@PresentDays", presentDays);
                command.Parameters.AddWithValue("@Bonus", bonus);
                command.Parameters.AddWithValue("@Deductions", deductions);
                command.Parameters.AddWithValue("@NetSalary", netSalary);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Payroll saved successfully!");
                ClearInputs();
                LoadPayrollSummary();
            }
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = GetPayrollDataTable();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data available for export.");
                    return;
                }

                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(dt, "Payroll Summary");
                    worksheet.Columns().AdjustToContents(); 

                   
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx",
                        Title = "Save Excel File",
                        FileName = "PayrollSummary.xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Payroll data exported to Excel successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message);
            }


        }

        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = GetPayrollDataTable();
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data available for export.");
                    return;
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Save PDF File",
                    FileName = "PayrollSummary.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document document = new Document(PageSize.A4);
                        PdfWriter.GetInstance(document, stream);
                        document.Open();

                       
                        Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                        Paragraph title = new Paragraph("Payroll Summary", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 10
                        };
                        document.Add(title);

                        PdfPTable table = new PdfPTable(dt.Columns.Count);
                        table.WidthPercentage = 100;

                        
                        foreach (DataColumn column in dt.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
                            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(cell);
                        }

                        
                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (var item in row.ItemArray)
                            {
                                table.AddCell(new Phrase(item.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                            }
                        }

                        document.Add(table);
                        document.Close();
                    }

                    MessageBox.Show("Payroll data exported to PDF successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to PDF: " + ex.Message);
            }

        }
        private DataTable GetPayrollDataTable()
        {
            DataTable dt = new DataTable();

            foreach (DataGridColumn column in PayrollSummary.Columns)
            {
                dt.Columns.Add(column.Header.ToString());
            }

            foreach (var item in PayrollSummary.Items)
            {
                if (item is DataRowView rowView)
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        row[i] = rowView.Row[i].ToString();
                    }
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
        private void ClearInputs()
        {
            cmbEmployeeID.SelectedIndex = -1;
            txtSalary.Clear();
            txtPresentDays.Clear();
            txtBonus.Clear();
            txtDeductions.Clear();
            txtNetSalary.Clear();
        }

    }
}
