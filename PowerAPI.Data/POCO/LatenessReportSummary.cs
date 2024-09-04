using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public partial class LatenessReportSummary
    {
     public string EmployeeId { get; set; }
     public string Name { get; set; }
     public double Minutes { get; set; }
    }
}
