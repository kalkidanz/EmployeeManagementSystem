using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Model
{
    internal class Payroll
    {
        public string PayrollID { get; set; }
        public string PresentDays { get; set; }
        public string Bonus { get; set; }
        public string Deduction { get; set; }
        public string NetSalary { get; set; }
       
    }
}
