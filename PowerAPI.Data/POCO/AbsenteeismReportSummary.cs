using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public partial class AbsenteeismReportSummary
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public int TotalAbsentDays { get; set; }
    }
}
