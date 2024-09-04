using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public partial class DeductionSummary
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string MissedPeriod { get; set; }
        public int TotalAbsentDays { get; set; }
        public int TotalMinutes { get; set; }
    }
}
