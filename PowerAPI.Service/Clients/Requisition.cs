using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Service.Helper;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PowerAPI.Service.Clients
{
    public class Requisition : IRequisition
    {
        EnterpriseContext _DBContext;

        public Requisition(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }
        public async Task<StatusMessage> Add(Data.ViewModels.Requisitions requisition, string Mode, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            RequisitionsHeader requisitionsHeader = new RequisitionsHeader();
            List<RequisitionsDetail> requisitionsDetail = new List<RequisitionsDetail>();

            string requisitionID = "";
            string nextNumberName = "NextRequisitionsNumber";
            double total = 0;

            try
            {
                if (requisition != null)
                {
                    var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                x.DivisionId == token.DivisionId &&
                                                                x.DepartmentId == token.DepartmentId &&
                                                                x.EmployeeId == requisition.RequestedBy).FirstOrDefaultAsync();

                    var requisitionType = await _DBContext.RequisitionsType.Where(x => x.CompanyId == token.CompanyId &&
                                                                x.DivisionId == token.DivisionId &&
                                                                x.DepartmentId == token.DepartmentId &&
                                                                x.RequisitionTypeId == requisition.RequisitionTypeId).FirstOrDefaultAsync();


                    if (emp != null && requisitionType != null)
                    {
                        requisition.CompanyId = token.CompanyId;
                        requisition.DivisionId = token.DivisionId;
                        requisition.DepartmentId = token.DepartmentId;

                        //get next requisition number
                        requisitionID = getNextEntityID(nextNumberName, token);

                        requisition.RequisitionId = requisitionID;

                        requisition.Cleared = false;
                        requisition.Approved = false;
                        requisition.Posted = false;

                        requisition.RequisitionDate = DateTime.Now;
                        requisition.CurrencyId = "NGN";
                        requisition.CurrencyExchangeRate = 1;
                        requisition.SystemDate = DateTime.Now;
                        requisition.EnteredDate = DateTime.Now;
                        requisition.EnteredBy = requisition.RequestedBy;
                        requisition.ClearedBy = requisition.RequestedBy;
                        requisition.RequestedByName = emp.EmployeeName;

                        requisitionsHeader = await convertToRequisitionHeader(requisition, token);

                        requisitionsDetail = requisition.requisitionDetail != null ? requisition.requisitionDetail : null;              

                        if (requisitionsDetail != null)
                        {
                            foreach (RequisitionsDetail req in requisitionsDetail)
                            {
                                req.CompanyId = token.CompanyId;
                                req.DivisionId = token.DivisionId;
                                req.DepartmentId = token.DepartmentId;
                                req.RequisitionId = requisitionID;

                                req.Price = req.Price == null || req.Price <= 0 ? 1 : req.Price;
                                req.Quantity = req.Quantity == null || req.Quantity <= 0 ? 1 : req.Quantity;

                                req.SubTotal = (double)(req.Price * req.Quantity);
                                req.Total = (double)(req.Price * req.Quantity);

                                req.RequisitionDetailId = 0;

                                total += (double)req.Total;

                                _DBContext.Entry(req).State = EntityState.Added;
                            }
                        }

                        requisitionsHeader.Total = total;
                        requisitionsHeader.SubTotal = total;

                        _DBContext.Entry(requisitionsHeader).State = EntityState.Added;

                        _DBContext.SaveChanges();

                        if (Mode == "Submit")
                        {
                            //call procedure to request approval;
                            statusMessage = submit(requisition, token);

                            if (statusMessage.Status == "Failed")
                            {
                                //delete entry
                                _DBContext.Entry(requisitionsHeader).State = EntityState.Deleted;
                                _DBContext.Entry(requisitionsDetail).State = EntityState.Deleted;
                                _DBContext.SaveChanges();
                            }
                        }
                        else
                        {
                            //call procedure to request approval;
                            statusMessage = submit(requisition, token);

                            if (statusMessage.Status == "Failed")
                            {
                                //delete entry
                                _DBContext.Entry(requisitionsHeader).State = EntityState.Deleted;
                                _DBContext.Entry(requisitionsDetail).State = EntityState.Deleted;
                                _DBContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Employee Does Not Exist";
                    }
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> Delete(Data.ViewModels.Requisitions requisition, ApiToken token)
        {
            throw new NotImplementedException();
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
               
            }

            return apiToken;
        }

        public async Task<IEnumerable<Data.ViewModels.Requisitions>> GetAll(ApiToken token)
        {
            List<Data.ViewModels.Requisitions> requisitions = new List<Data.ViewModels.Requisitions>();
            try
            {
                var requisition = await _DBContext.RequisitionsHeader.OrderByDescending(x => x.RequiredDate).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId).ToListAsync();

                requisitions = await req(requisition, token);

            }
            catch (Exception)
            {

            }

            return requisitions;
        }

        public async Task<IEnumerable<Data.ViewModels.Requisitions>> GetByEmployee(string Id, string Mode, ApiToken token)
        {
            List<Data.ViewModels.Requisitions> requisitions = new List<Data.ViewModels.Requisitions>();

            try
            {
                if (Mode == "Current")
                {
                    var requisition = await _DBContext.RequisitionsHeader.OrderByDescending(x => x.RequiredDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.RequestedBy == Id &&
                                                            x.Approved == false).ToListAsync();

                    requisitions = await req(requisition, token);
                }
                else if (Mode == "History")
                {
                    var requisition = await _DBContext.RequisitionsHeader.OrderByDescending(x => x.RequiredDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.RequestedBy == Id &&
                                                            x.Approved == true).ToListAsync();

                    requisitions = await req(requisition, token);
                }
                else
                {
                    var requisition = await _DBContext.RequisitionsHeader.OrderByDescending(x => x.RequiredDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.RequestedBy == Id &&
                                                            x.Approved == false).ToListAsync();
                    requisitions = await req(requisition, token);
                }
            }
            catch (Exception)
            {

            }
            return requisitions;
        }

        public async Task<Data.ViewModels.Requisitions> GetById(string Id, ApiToken token)
        {
            List<Data.ViewModels.Requisitions> requisitions = new List<Data.ViewModels.Requisitions>();
            Data.ViewModels.Requisitions requisition = new Data.ViewModels.Requisitions();

            try
            {
                var reqs = await _DBContext.RequisitionsHeader.OrderByDescending(x => x.RequiredDate).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.RequisitionId == Id).ToListAsync();

                requisitions = await req(reqs, token);

                requisition = requisitions.FirstOrDefault();
            }
            catch (Exception)
            {

            }

            return requisition;
        }

        public async Task<IEnumerable<RequisitionsType>> GetRequisitionType(ApiToken token)
        {
            try
            {
                return await _DBContext.RequisitionsType.OrderBy(x => x.RequisitionTypeId).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> Update(Data.ViewModels.Requisitions requisition, ApiToken token)
        {
            throw new NotImplementedException();
        }

        private async Task<List<Data.ViewModels.Requisitions>> req(List<RequisitionsHeader> requisition, ApiToken token)
        {
            List<Data.ViewModels.Requisitions> requisitions = new List<Data.ViewModels.Requisitions>();
            try
            {
                if (requisition != null)
                {
                    foreach (RequisitionsHeader req in requisition)
                    {
                        Data.ViewModels.Requisitions requisitionObj = new Data.ViewModels.Requisitions();

                        requisitionObj.CompanyId = req.CompanyId;
                        requisitionObj.DivisionId = req.DivisionId;
                        requisitionObj.DepartmentId = req.DepartmentId;
                        requisitionObj.RequisitionId = req.RequisitionId;
                        requisitionObj.RequisitionTypeId = req.RequisitionTypeId;
                        requisitionObj.RequisitionDate = req.RequisitionDate;
                        requisitionObj.CurrencyId = req.CurrencyId;
                        requisitionObj.CurrencyExchangeRate = req.CurrencyExchangeRate;
                        requisitionObj.Description = req.Description;
                        requisitionObj.EntityId = req.EntityId;
                        requisitionObj.SubTotal = req.SubTotal;
                        requisitionObj.Total = req.Total;
                        requisitionObj.SystemDate = req.SystemDate;
                        requisitionObj.EnteredBy = req.EnteredBy;
                        requisitionObj.EnteredDate = req.EnteredDate;
                        requisitionObj.Cleared = req.Cleared;
                        requisitionObj.ClearedBy = req.ClearedBy;
                        requisitionObj.ClearedDate = req.ClearedDate;
                        requisitionObj.Approved = req.Approved;
                        requisitionObj.ApprovedBy = req.ApprovedBy;
                        requisitionObj.ApprovedDate = req.ApprovedDate;
                        requisitionObj.LockedBy = req.LockedBy;
                        requisitionObj.LockTs = req.LockTs;
                        requisitionObj.RequiredDate = req.RequiredDate;
                        requisitionObj.RequestedBy = req.RequestedBy;
                        requisitionObj.Posted = req.Posted;
                        requisitionObj.PostedBy = req.PostedBy;
                        requisitionObj.PostedDate = req.PostedDate;
                        requisitionObj.ConvertedTo = req.ConvertedTo;
                        requisitionObj.ApprovedByName = req.ApprovedByName;
                        requisitionObj.RequestedByName = req.RequestedByName;
                        requisitionObj.ReferenceNumber = req.ReferenceNumber;

                        requisitionObj.requisitionDetail = await _DBContext.RequisitionsDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.RequisitionId == req.RequisitionId).ToListAsync();

                        requisitions.Add(requisitionObj);
                    }
                }
            }
            catch (Exception)
            {

            }

            return requisitions;


        }

        private async Task<RequisitionsHeader> convertToRequisitionHeader(Data.ViewModels.Requisitions requisition, ApiToken token)
        {
            RequisitionsHeader requisitions = new RequisitionsHeader();
            try
            {
                if (requisition != null)
                {
                    
                        requisitions.CompanyId = requisition.CompanyId;
                        requisitions.DivisionId = requisition.DivisionId;
                        requisitions.DepartmentId = requisition.DepartmentId;
                        requisitions.RequisitionId = requisition.RequisitionId;
                        requisitions.RequisitionTypeId = requisition.RequisitionTypeId;
                        requisitions.RequisitionDate = requisition.RequisitionDate;
                        requisitions.CurrencyId = requisition.CurrencyId;
                        requisitions.CurrencyExchangeRate = requisition.CurrencyExchangeRate;
                        requisitions.Description = requisition.Description;
                        requisitions.EntityId = requisition.EntityId;
                        requisitions.SubTotal = requisition.SubTotal;
                        requisitions.Total = requisition.Total;
                        requisitions.SystemDate = requisition.SystemDate;
                        requisitions.EnteredBy = requisition.EnteredBy;
                        requisitions.EnteredDate = requisition.EnteredDate;
                        requisitions.Cleared = (bool)requisition.Cleared;
                        requisitions.ClearedBy = requisition.ClearedBy;
                        requisitions.ClearedDate = requisition.ClearedDate;
                        requisitions.Approved = (bool)requisition.Approved;
                        requisitions.ApprovedBy = requisition.ApprovedBy;
                        requisitions.ApprovedDate = requisition.ApprovedDate;
                        requisitions.LockedBy = requisition.LockedBy;
                        requisitions.LockTs = requisition.LockTs;
                        requisitions.RequiredDate = requisition.RequiredDate;
                        requisitions.RequestedBy = requisition.RequestedBy;
                        requisitions.Posted = (bool)requisition.Posted;
                        requisitions.PostedBy = requisition.PostedBy;
                        requisitions.PostedDate = requisition.PostedDate;
                        requisitions.ConvertedTo = requisition.ConvertedTo;
                        requisitions.ApprovedByName = requisition.ApprovedByName;
                        requisitions.RequestedByName = requisition.RequestedByName;
                        requisitions.ReferenceNumber = requisition.ReferenceNumber;

                }
            }
            catch (Exception)
            {

            }

            return requisitions;
        }

        //get next number
        public string getNextEntityID(string nextNumberName, ApiToken token)
        {
            string sEntityID = "";

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sNextNumberName = new SqlParameter("@Entity", nextNumberName);
                var EntityID = new SqlParameter("@EntityID", SqlDbType.NVarChar, 255);
                EntityID.Direction = ParameterDirection.Output;

                _DBContext.Database
                          .ExecuteSqlRaw("enterprise.GetNextEntityID @CompanyID, @DivisionID, @DepartmentID, @Entity, @EntityID Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sNextNumberName, EntityID });

                sEntityID = EntityID.Value.ToString();
            }
            catch (Exception ex)
            {

            }

            return sEntityID;
        }

        public async Task<IEnumerable<Warehouses>> GetWarehouse(ApiToken token)
        {
            try
            {
                return await _DBContext.Warehouses.OrderBy(x => x.WarehouseId).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.Active == true).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<WarehouseBins>> GetWarehouseBins(ApiToken token)
        {
            try
            {
                return await _DBContext.WarehouseBins.OrderBy(x => x.WarehouseBinId).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.Active == true).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<WarehouseBins>> GetBinsByWarehouse(string Id, ApiToken token)
        {
            try
            {
                return await _DBContext.WarehouseBins.OrderBy(x => x.WarehouseBinId).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.WarehouseId == Id &&
                                                           x.Active == true).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<Data.Models.InventoryItems>> GetInventoryItems(string Mode, ApiToken token)
        {
            try
            {
                List<Data.Models.InventoryItems> inventoryItems = new List<Data.Models.InventoryItems>();


                if (Mode == "Stock")
                {
                    return await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                              x.DivisionId == token.DivisionId &&
                                                              x.DepartmentId == token.DepartmentId &&
                                                              x.IsActive == true &&
                                                              x.ItemTypeId == "stock")
                                                              .ToListAsync();
                }
                else if (Mode == "Service")
                {
                    return await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.IsActive == true &&
                                                               x.ItemTypeId == "service")
                                                               .ToListAsync();
                }
                else
                {
                    return await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.IsActive == true)
                                                               .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Paging> GetInventoryCatalog(int itemsPerPage, int PageNumber, string Mode, ApiToken token)
        {
            PaginationParams @params = new PaginationParams();
           // PaginationParams itemsPerPage = new PaginationParams();
           // PaginationParams PageNumber = new PaginationParams();
            try
            {
                var totalCount = await _DBContext.RmatransactionsHeader.CountAsync();

                List<Data.Models.InventoryItems> inventoryItems = new List<Data.Models.InventoryItems>();


                if (Mode == "Stock")
                {
                     inventoryItems = await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.IsActive == true &&
                                                               x.ItemTypeId == "stock")
                                                               .Skip((@params.Page - 1) * @params.ItemsPerPage)
                                                               .Take(@params.ItemsPerPage)
                                                               .ToListAsync();
                }
                else if(Mode == "Service")
                {
                    inventoryItems = await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.IsActive == true &&
                                                               x.ItemTypeId == "service")
                                                               .Skip((@params.Page - 1) * @params.ItemsPerPage)
                                                               .Take(@params.ItemsPerPage)
                                                               .ToListAsync();
                }
                else
                {
                    inventoryItems = await _DBContext.InventoryItems.OrderBy(x => x.ItemId).Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.IsActive == true)
                                                               .Skip((@params.Page - 1) * @params.ItemsPerPage)
                                                               .Take(@params.ItemsPerPage)
                                                               .ToListAsync();
                }

                 var paginationMetadata = new PaginationMetadata(@params.Page, totalCount, @params.ItemsPerPage);
                 

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryList = inventoryItems,
                };
                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }

            //return null;
        }

        public async Task<StatusMessage> Approve(RequisitionAppModel requisitionAppModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (requisitionAppModel != null)
                {
                    var ssEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();

                    var payrollEmployee = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                           x.EmployeeTypeId == "User" &&
                                                           x.ActiveYn == true).FirstOrDefaultAsync();

                    if (ssEmployee != null)
                    {
                        PayrollEmployees systemUser = new PayrollEmployees();

                        if (ssEmployee.EmployeeEmailAddress != null)
                        {

                            systemUser = await _DBContext.PayrollEmployees
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeEmailAddress == ssEmployee.EmployeeEmailAddress &&
                                                                   x.EmployeeTypeId == "User" &&
                                                                   x.ActiveYn == true).FirstOrDefaultAsync();
                            if (systemUser != null)
                            {
                                //get approval rights
                                var userPermissions = await _DBContext.AccessPermissions
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == systemUser.EmployeeId).FirstOrDefaultAsync();

                                if (userPermissions != null)
                                {
                                    userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;

                                    if (userPermissions.SscanApproveLoan == true)
                                    {
                                        requisitionAppModel.ProcessBy = systemUser.EmployeeId;

                                        //call procedure to approval;
                                        statusMessage = await approve(requisitionAppModel, token);
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "You dont have access to perform requisition approval. Contact system Administrator.";

                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "You dont have access to perform this function. Contact system Administrator.";
                                }

                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Could not find ERP User profile mapped to your Employee No. Contact system Administrator.";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "An email address needs to be setup for your User profile on ERP. Contact system Administrator.";
                        }
                    }
                    else if(payrollEmployee != null)
                    {

                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Approved By";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Requisition does not exist.";
                }
           
            }
            catch (SqlException ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }

            return statusMessage;
        }

        private StatusMessage submit(Data.ViewModels.Requisitions requisition, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sRequisitionID = new SqlParameter("@RequisitionID", requisition.RequisitionId);
                var sEmployeeID = new SqlParameter("@EmployeeID", requisition.EnteredBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                _DBContext.Database
                          .ExecuteSqlRaw("enterprise.Requisitions_Book @CompanyID, @DivisionID, @DepartmentID, @RequisitionID, @EmployeeID, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sRequisitionID, sEmployeeID, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = result;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        private async Task<StatusMessage> approve(RequisitionAppModel requisition, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sRequisitionID = new SqlParameter("@RequisitionID", requisition.RequisitionID);
                var sEmployeeID = new SqlParameter("@EmployeeID", requisition.ProcessBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.Requisitions_Approve @CompanyID, @DivisionID, @DepartmentID, @RequisitionID, @EmployeeID, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sRequisitionID, sEmployeeID, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Requisition Approved Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = result;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> ConvertToPurchase(RequisitionAppModel requisitionAppModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (requisitionAppModel != null)
                {
                    var ssEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();

                    var payrollEmployee = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                           x.EmployeeTypeId == "User" &&
                                                           x.ActiveYn == true).FirstOrDefaultAsync();

                    if (ssEmployee != null)
                    {
                        PayrollEmployees systemUser = new PayrollEmployees();

                        if (ssEmployee.EmployeeEmailAddress != null)
                        {

                            systemUser = await _DBContext.PayrollEmployees
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeEmailAddress == ssEmployee.EmployeeEmailAddress &&
                                                                   x.EmployeeTypeId == "User" &&
                                                                   x.ActiveYn == true).FirstOrDefaultAsync();
                            if (systemUser != null)
                            {
                                //get approval rights
                                var userPermissions = await _DBContext.AccessPermissions
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == systemUser.EmployeeId).FirstOrDefaultAsync();

                                if (userPermissions != null)
                                {
                                    userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;

                                    if (userPermissions.SscanApproveLoan == true)
                                    {
                                        requisitionAppModel.ProcessBy = systemUser.EmployeeId;

                                        //call procedure to approval;
                                        statusMessage = await convertToPurchase(requisitionAppModel, token);
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "You dont have access to perform requisition approval. Contact system Administrator.";

                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "You dont have access to perform this function. Contact system Administrator.";
                                }

                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Could not find ERP User profile mapped to your Employee No. Contact system Administrator.";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "An email address needs to be setup for your User profile on ERP. Contact system Administrator.";
                        }
                    }
                    else if (payrollEmployee != null)
                    {

                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Approved By";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Requisition does not exist.";
                }

            }
            catch (SqlException ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }

            return statusMessage;
        }

        public async Task<StatusMessage> ConvertToIssue(RequisitionAppModel requisitionAppModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (requisitionAppModel != null)
                {
                    var ssEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();

                    var payrollEmployee = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == requisitionAppModel.ProcessBy &&
                                                           x.EmployeeTypeId == "User" &&
                                                           x.ActiveYn == true).FirstOrDefaultAsync();

                    if (ssEmployee != null)
                    {
                        PayrollEmployees systemUser = new PayrollEmployees();

                        if (ssEmployee.EmployeeEmailAddress != null)
                        {

                            systemUser = await _DBContext.PayrollEmployees
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeEmailAddress == ssEmployee.EmployeeEmailAddress &&
                                                                   x.EmployeeTypeId == "User" &&
                                                                   x.ActiveYn == true).FirstOrDefaultAsync();
                            if (systemUser != null)
                            {
                                //get approval rights
                                var userPermissions = await _DBContext.AccessPermissions
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == systemUser.EmployeeId).FirstOrDefaultAsync();

                                if (userPermissions != null)
                                {
                                    userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;

                                    if (userPermissions.SscanApproveLoan == true)
                                    {
                                        requisitionAppModel.ProcessBy = systemUser.EmployeeId;

                                        //call procedure to approval;
                                        statusMessage = await convertToIssue(requisitionAppModel, token);
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "You dont have access to perform requisition approval. Contact system Administrator.";

                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "You dont have access to perform this function. Contact system Administrator.";
                                }

                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Could not find ERP User profile mapped to your Employee No. Contact system Administrator.";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "An email address needs to be setup for your User profile on ERP. Contact system Administrator.";
                        }
                    }
                    else if (payrollEmployee != null)
                    {

                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Approved By";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Requisition does not exist.";
                }

            }
            catch (SqlException ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.ToString();
            }

            return statusMessage;
        }

        private async Task<StatusMessage> convertToPurchase(RequisitionAppModel requisition, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sRequisitionID = new SqlParameter("@RequisitionID", requisition.RequisitionID);
                var sEmployeeID = new SqlParameter("@EmployeeID", requisition.ProcessBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.Requisitions_ConvertTo_Purchase @CompanyID, @DivisionID, @DepartmentID, @RequisitionID, @EmployeeID, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sRequisitionID, sEmployeeID, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Purchase Created From Requisition Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = result;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        private async Task<StatusMessage> convertToIssue(RequisitionAppModel requisition, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sRequisitionID = new SqlParameter("@RequisitionID", requisition.RequisitionID);
                var sEmployeeID = new SqlParameter("@EmployeeID", requisition.ProcessBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.Requisitions_ConvertTo_Issue @CompanyID, @DivisionID, @DepartmentID, @RequisitionID, @EmployeeID, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sRequisitionID, sEmployeeID, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Stock Issue Created From Requisition Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = result;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }
    }


}
