using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class Roles
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string LockedBy { get; set; }
        public bool? LockTs { get; set; }
    }
}
