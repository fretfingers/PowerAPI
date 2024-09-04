//using Microsoft.AspNet.Identity;
using PowerAPI.Data.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace PowerAPI.Service.IdentityLibrary
{
    /// <summary>
    /// The custom implementation of <see cref="IdentityUser{TKey}"/> which uses a string as a primary key.
    /// </summary>
    public class CustomIdentityUser: IdentityUser<string>
    {
        /// <summary>
        /// Initializes a new instance of CustomIdentityUser/>.
        /// </summary>
        public CustomIdentityUser()
        {

            
        }

        /// <summary>
        /// Initializes a new instance of CustomIdentityUser />.
        /// </summary>
        public CustomIdentityUser(string company, string division, string department, string userName) : this()
        {
            Id = company + "_" + division + "_" + department + "_" + userName;
            UserName = company + "_" + division + "_" + department + "_" + userName;
            NormalizedUserName = UserName.ToUpper();
        }

        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string unAppendedUsername { get; set; } = string.Empty;
        public string DefaultSite { get; set; } = string.Empty; // or Branch
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public bool RestrictMultipleLogins { get; set; }
        public List<string> AccType { get; set; }
        public List<string> Scope { get; set; } // Accessible Modules
        public List<int> Roles { get; set; }
        public List<string> Warehouses { get; set; }
        public string DefaultHomeScreen { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool EnforcePasswordReset { get; set; }
        public string BranchCode { get; set; }

        public string CurrentCompany { get; set; }
        public string CurrentDivision { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentBranch {  get; set; }


    }
}
