using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.Models
{
    public partial class CompanyKeys
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set;}
        public string DepartmentId { get; set;}
        public string Username { get; set;}
        public string Password { get; set; }
        public string Token { get; set;}
        public string ApiKey { get; set;}
        public string ProfileType { get; set;}
        public bool Active { get; set; }
        public string BaseUrl { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string SenderId { get; set; }    
    }
}
