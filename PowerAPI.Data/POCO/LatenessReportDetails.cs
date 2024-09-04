using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public partial class LatenessReportDetails
    {
      public string EmployeeId { get; set; }
      public string Name { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public DateTime Date { get; set; }
      public double TimeInMinutes { get; set; }
    }
}
