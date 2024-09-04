using DevExpress.Drawing.Internal.Fonts.Interop;
using PowerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.POCO
{
    public class ApiUser
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string CompanyId { get; set; } = string.Empty;
        public string DivisionId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string Email {get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string DefaultSite { get; set; } = string.Empty; // or Branch
        public string PasswordHash { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public bool EnforcePasswordReset { get; set; }
        public bool RestrictMultipleLogins { get; set; }
        public List<string> AccType { get; set; }
        public List<string> Scope { get; set; } // Module Access
        public List<int> Roles {get; set; }
        public List<string> Warehouses {get; set; }
        public string DefaultHomeScreen { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string BranchCode { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentDivision { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentBranch { get; set; }

        public ApiUser() {
            AccType = new List<string>();
            Scope = new List<string>();
            Roles = new List<int>();
            Warehouses = new List<string>();
        }
    }
}
