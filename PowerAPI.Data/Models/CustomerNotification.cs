using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CustomerNotification
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CAgentName { get; set; }
        public string NotificationType { get; set; }
        public DateTime NotificationDate { get; set; }
        public TimeSpan? NotificationTime { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
