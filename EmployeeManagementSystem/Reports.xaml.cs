using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using OfficeOpenXml; 
using iTextSharp.text; 
using iTextSharp.text.pdf;
using System.Windows.Documents;
using System.Xml.Linq;
using EmployeeManagementSystem.Model;

namespace EmployeeManagementSystem
{
    public partial class Reports : UserControl
    {
        private string connectionString = @"Data Source=KAL\SQLEXPRESS;Initial Catalog=Employee;Integrated Security=True;Encrypt=False";

        public Reports()
        {
            InitializeComponent();
            LoadAllReports();
        }

        private void LoadAllReports()
        {
            try
            {
                List<Report> reportList = new List<Report>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    string query = @"
                SELECT 
                    E.EmployeeID, 
                    E.FullName, 
                    E.Department, 
                    E.Position, 
                    E.Salary, 
                    P.PresentDays, 
                    P.Bonus, 
                    P.Deductions, 
                    P.NetSalary
                FROM 
                    Employee E
                LEFT JOIN 
                    Payroll P ON E.EmployeeID = P.EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        reportList.Add(new Report
                        {
                            EmployeeID = reader["EmployeeID"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            Department = reader["Department"].ToString(),
                            Position = reader["Position"].ToString(),
                            Salary = Convert.ToDecimal(reader["Salary"]),
                            PresentDays = reader["PresentDays"] != DBNull.Value ? Convert.ToInt32(reader["PresentDays"]) : 0,
                            Bonus = reader["Bonus"] != DBNull.Value ? Convert.ToDecimal(reader["Bonus"]) : 0,
                            Deductions = reader["Deductions"] != DBNull.Value ? Convert.ToDecimal(reader["Deductions"]) : 0,
                            NetSalary = reader["NetSalary"] != DBNull.Value ? Convert.ToDecimal(reader["NetSalary"]) : 0
                        });
                    }
                }

                lvReports.ItemsSource = reportList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reports: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Properties.Title = "Employee Reports";
                    ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("Employees");

                    sheet.Cells[1, 1].Value = "Employee ID";
                    sheet.Cells[1, 2].Value = "Employee Name";
                    sheet.Cells[1, 3].Value = "Department";
                    sheet.Cells[1, 4].Value = "Position";
                    sheet.Cells[1, 5].Value = "Salary";
                    sheet.Cells[1, 6].Value = "Days Present";
                    sheet.Cells[1, 7].Value = "Bonus";
                    sheet.Cells[1, 8].Value = "Deductions";
                    sheet.Cells[1, 9].Value = "Net Salary";

                    int row = 2;
                    foreach (Report item in lvReports.Items)
                    {
                        sheet.Cells[row, 1].Value = item.EmployeeID;
                        sheet.Cells[row, 2].Value = item.FullName;
                        
                        sheet.Cells[row, 3].Value = item.Department;
                        sheet.Cells[row, 4].Value = item.Position;
                        sheet.Cells[row, 5].Value = item.Salary;
                        sheet.Cells[row, 6].Value = item.PresentDays;
                        sheet.Cells[row, 7].Value = item.Bonus;
                        sheet.Cells[row, 8].Value = item.Deductions;
                        sheet.Cells[row, 9].Value = item.NetSalary;
                        row++;
                    }

                    string filePath = "c://Users//kalki//Downloads/Employee_Report.xlsx";
                    File.WriteAllBytes(filePath, excel.GetAsByteArray());
                    MessageBox.Show("Excel file saved successfully: " + filePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Document doc = new Document();
                string filePath = "c://Users//kalki//Downloads/Employee_Report.pdf";
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

                doc.Open();
                doc.Add(new iTextSharp.text.Paragraph("Employee Report\n\n"));

                PdfPTable table = new PdfPTable(8);
                table.AddCell("Employee ID");
                table.AddCell("Employee Name");
               
                table.AddCell("Department");
                table.AddCell("Position");
                table.AddCell("Salary");
                table.AddCell("Days Present");
                table.AddCell("Bonus");
                table.AddCell("Deductions");
                table.AddCell("Net Salary");

                foreach (Report item in lvReports.Items)
                {
                    table.AddCell(item.EmployeeID);
                    table.AddCell(item.FullName);
                    
                    table.AddCell(item.Department);
                    table.AddCell(item.Position);
                    table.AddCell(item.Salary.ToString());
                    table.AddCell(item.PresentDays.ToString());
                    table.AddCell(item.Bonus.ToString());
                    table.AddCell(item.Deductions.ToString());
                    table.AddCell(item.NetSalary.ToString());
                }

                doc.Add(table);
                doc.Close();

                MessageBox.Show("PDF file saved successfully: " + filePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

   
}
