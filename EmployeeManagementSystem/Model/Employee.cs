using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Model
{
    internal class Employee
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNo { get; set; }
        public string Department { get; set; }

        public string Position { get; set; }
        public decimal Salary { get; set; }
    }
}
