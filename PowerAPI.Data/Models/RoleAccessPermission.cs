using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class RoleAccessPermission
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool? CanInsert { get; set; }
        public bool? CanUpdate { get; set; }
        public bool? CanDelete { get; set; }
    }
}
