using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class CompanyBranch : Models.CompanyBranch
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchManager { get; set; }
        public string BranchAddress { get; set; }
        public string ContactPerson { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchAddress2 { get; set; }
        public string BranchAddress3 { get; set; }
        public string BranchCity { get; set; }
        public string BranchState { get; set; }
        public string BranchCountry { get; set; }
        public string BranchZip { get; set; }
        public string BranchPhone { get; set; }
        public string BranchEmail { get; set; }
        public string BranchFax { get; set; }
        public string BranchNotes { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
