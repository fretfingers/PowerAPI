using DevExpress.Data.Linq.Helpers;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.PivotGrid.OLAP;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class Projects : IProjects
    {
        EnterpriseContext _DBContext;

        public Projects(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }
        public async Task<ApiToken> GetAccess(string token)
        {
            int days = 0;
            ApiToken apiToken = new ApiToken();

            try
            {
                //get the comp on token
                apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                if (apiToken != null)
                {
                    //get reg info
                    var regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                                                            x.DivisionId == apiToken.DivisionId &&
                                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();
                    if (regInfo != null)
                    {
                        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                        apiToken.RegCode = regInfo.RegCode;
                        apiToken.RegCode = regInfo.RegName;
                        apiToken.TotalDays = days;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return apiToken;
        }

        public async Task<Paging> GetProjects(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.Projects> projects = new List<Data.ViewModels.Projects>();
            try
            {
                var totalCount = await _DBContext.Projects
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.Projects.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.ProjectId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                projects = await project(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    ProjectsList = projects
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetProjectsById(PaginationParams Param, string Id, ApiToken token)
        {
            List<Data.ViewModels.Projects> projects = new List<Data.ViewModels.Projects>();
            try
            {
                var totalCount = await _DBContext.Projects
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectId == Id
                                                        ).CountAsync();

                var result = await _DBContext.Projects.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectId == Id)//.ToListAsync();
                                                        .OrderBy(x => x.ProjectId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                projects = await project(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    ProjectsList = projects
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetProjectsByName(PaginationParams Param, string name, ApiToken token)
        {
            List<Data.ViewModels.Projects> projects = new List<Data.ViewModels.Projects>();
            try
            {
                var totalCount = await _DBContext.Projects
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectName == name
                                                        ).CountAsync();

                var result = await _DBContext.Projects.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectName == name)//.ToListAsync();
                                                        .OrderBy(x => x.ProjectId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                projects = await project(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    ProjectsList = projects
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       
        public async Task<Paging> GetProjectsByType(PaginationParams Param, string projectType, ApiToken token)
        {
            List<Data.ViewModels.Projects> projects = new List<Data.ViewModels.Projects>();
            try
            {
                var totalCount = await _DBContext.Projects
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectTypeId == projectType
                                                        ).CountAsync();

                var result = await _DBContext.Projects.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ProjectTypeId == projectType)//.ToListAsync();
                                                        .OrderBy(x => x.ProjectId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                projects = await project(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    ProjectsList = projects
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        #region View Model Data List

        private async Task<List<Data.ViewModels.Projects>> project(List<Data.Models.Projects> obj, ApiToken token)
        {
            List<Data.ViewModels.Projects> projects = new List<Data.ViewModels.Projects>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.Projects project in obj)
                    {
                        Data.ViewModels.Projects projectObj = new Data.ViewModels.Projects();

                        var workflows = new List<TransactionWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<TransactionWorkflow>
                                            {
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = project.ProjectId,
                                                    TransactionDate = project.ProjectStartDate ?? currentDate,
                                                    StepSequence = 1,
                                                    Status = "Draft",
                                                    IsCompleted = false,
                                                    DateCompleted = project.ProjectCompleteDate
                                                },
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = project.ProjectId,
                                                    TransactionDate = project.ProjectStartDate ?? currentDate,
                                                    StepSequence = 2,
                                                    Status = "Pending",
                                                    IsCompleted = false,
                                                    DateCompleted = project.ProjectCompleteDate
                                                },
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = project.ProjectId,
                                                    TransactionDate = project.ProjectStartDate ?? currentDate,
                                                    StepSequence = 3,
                                                    Status = "Approved",
                                                    IsCompleted = false,
                                                    DateCompleted = project.ProjectCompleteDate
                                                }
                                            };

                        var lastCompletedWorkflow = workflows
                                                        .Where(ow => ow.IsCompleted)
                                                        .OrderBy(workflow => workflow.StepSequence)
                                                        .LastOrDefault();


                        projectObj.CompanyId = project.CompanyId;
                        projectObj.DivisionId = project.DivisionId;
                        projectObj.DepartmentId = project.DepartmentId;
                        projectObj.ProjectId = project.ProjectId;
                        projectObj.ProjectName = project.ProjectName;
                        projectObj.ProjectDescription = project.ProjectDescription;
                        projectObj.CustomerId = project.CustomerId;
                        projectObj.ProjectTypeId = project.ProjectTypeId;
                        projectObj.ProjectStartDate = project.ProjectStartDate;
                        projectObj.ProjectCompleteDate = project.ProjectCompleteDate;
                        projectObj.CurrencyId = project.CurrencyId;
                        projectObj.CurrencyExchangeRate = project.CurrencyExchangeRate;
                        projectObj.ProjectEstRevenue = project.ProjectEstRevenue;
                        projectObj.ProjectActualRevenue = project.ProjectActualRevenue;
                        projectObj.ProjectEstCost = project.ProjectEstCost;
                        projectObj.ProjectActualCost = project.ProjectActualCost;
                        projectObj.EmployeeId = project.EmployeeId;
                        projectObj.ProjectNotes = project.ProjectNotes;
                        projectObj.GlsalesAccount = project.GlsalesAccount;
                        projectObj.ProjectOpen = project.ProjectOpen;
                        projectObj.Memorize = project.Memorize;
                        projectObj.LockedBy = project.LockedBy;
                        projectObj.LockTs = project.LockTs;
                        projectObj.OrderNumber = project.OrderNumber;
                        projectObj.ProjectOrderValue = project.ProjectOrderValue;
                        projectObj.ProjectAbsorbedCost = project.ProjectAbsorbedCost;
                        projectObj.Glcosaccount = project.Glcosaccount;
                        projectObj.Approved = project.Approved;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedDate = project.ApprovedDate;
                        projectObj.Cleared = project.Cleared;
                        projectObj.EnteredBy = project.EnteredBy;
                        projectObj.ClearedDate = project.ClearedDate;
                        projectObj.Void = project.Void;
                        projectObj.Commission = project.Commission;
                        projectObj.BranchCode = project.BranchCode;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;
                        projectObj.ApprovedBy = project.ApprovedBy;

                        projectObj.Status = lastCompletedWorkflow?.Status ?? "Draft";
                        projectObj.WorkFlowTrail = workflows;
                        projectObj.projectsDetails = await _DBContext.ProjectsDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.ProjectId == project.ProjectId).ToListAsync();
                        projects.Add(projectObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return projects;

        }

        #endregion View Model Data List
    }
}
