using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.Models
{
    public partial class EmployeeAttendance
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string FingerPrint { get; set; }
        public DateTime Time { get; set; }
    }
}
