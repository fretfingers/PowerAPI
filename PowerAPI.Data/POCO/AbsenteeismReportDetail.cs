using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
  public  partial class AbsenteeismReportDetail
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateAbsent { get; set; }
    }
}
