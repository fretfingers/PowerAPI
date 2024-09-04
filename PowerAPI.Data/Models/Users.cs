using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class Users
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string UserGroupId { get; set; }
        public int? RoleId { get; set; }
        public bool? Active { get; set; }
        public bool? PasswordExpires { get; set; }
        public DateTime? PasswordExpiresDate { get; set; }
        public string Signature { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string RefreshToken { get; set; }
        public DateTime?  RefreshTokenExpiry { get; set; }
        public string CurrentCompany { get; set; }
        public string CurrentDivision { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentBranch { get; set; }
        public virtual ICollection<FavouritedMenuByUser> FavouritedMenuByUsers { get; set; } = new List<FavouritedMenuByUser>();
    }
}
