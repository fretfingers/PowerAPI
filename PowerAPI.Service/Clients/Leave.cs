using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PowerAPI.Service.Helper;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PowerAPI.Service.Clients
{
    public class Leave : ILeave
    {
        EnterpriseContext _DBContext;

        public Leave(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }
        public async Task<StatusMessage> AddLeave(PayrollHrpayrollLeaveDetail leave, string Mode, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == leave.EmployeeId).FirstOrDefaultAsync();

                var leaveType = await _DBContext.PayrollHrpayrollLeaveType.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.LeaveTypeId == leave.LeaveType).FirstOrDefaultAsync();

                var approver = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == leave.ApprovedBy).FirstOrDefaultAsync();

                var secondApprover = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == leave.PostedBy).FirstOrDefaultAsync();

                if(emp != null)
                {
                    if(leaveType != null)
                    {
                        if(emp.GroupId != "" && emp.GroupId != null)
                        {
                            var leaveTypeGroup = await _DBContext.PayrollHrpayrollGroupLeave.Where(x => x.CompanyId == token.CompanyId &&
                                                                        x.DivisionId == token.DivisionId &&
                                                                        x.DepartmentId == token.DepartmentId &&
                                                                        x.GroupId == emp.GroupId &&
                                                                        x.LeaveTypeId == leave.LeaveType).FirstOrDefaultAsync();

                            leave.Cleared = false;
                            leave.Approved = false;
                            leave.Audited = false;
                            leave.Posted = false;

                            leave.ApprovedDate = null;
                            leave.AuditedDate = null;
                            leave.PostedDate = null;
                            leave.ExecutiveApproveDate = null;
                            leave.EnteredDate = DateTime.Now.Date;

                            leave.StartDate = leave.DateFrom;
                            leave.EndDate = leave.DateTo;
                            leave.NotesB = "Leave Application From ESS";

                            leave.CompanyId = token.CompanyId;
                            leave.DivisionId = token.DivisionId;
                            leave.DepartmentId = token.DepartmentId;
                            leave.OutLeaveDays = leaveTypeGroup != null && leaveTypeGroup.LeaveDays != null && leaveTypeGroup.LeaveDays != 0 ? leaveTypeGroup.LeaveDays : leaveType.MaximumDays;
                            leave.ActualAmount = 0;
                            leave.Amount = 0;
                            leave.PriorLeaveDays = 0;
                            leave.ApprovedByName = approver == null ? "" : approver.EmployeeName + " " + approver.EmployeeFirstname;
                            leave.PostedByName = secondApprover == null ? "" : secondApprover.EmployeeName + " " + secondApprover.EmployeeFirstname;

                            leave.LeaveDays = (int)(leave.DateTo - leave.DateFrom).TotalDays + 1;

                            //assign default
                            leave.PayTypeId = null;
                            leave.LockedBy = null;
                            leave.LockTs = null;
                            leave.Status = null;
                            leave.SsapprovedBy = null;
                            leave.ExecutivePermisionBy = null;
                            leave.BranchCode = token.BranchCode;
                            leave.ExecutivePermisionName = null;
                            leave.Transtype = null;
                            leave.AuditedBy = null;



                            _DBContext.Entry(leave).State = EntityState.Added;
                            _DBContext.SaveChanges();

                            if (Mode == "Submit")
                            {
                                //call procedure to request approval;
                                statusMessage = submit(leave, token);

                                if (statusMessage.Status == "Failed")
                                {
                                    //delete entry
                                    _DBContext.Entry(leave).State = EntityState.Deleted;
                                    _DBContext.SaveChanges();
                                }
                            }
                            else
                            {
                                //call procedure to request approval;
                                statusMessage = submit(leave, token);

                                if(statusMessage.Status == "Failed")
                                {
                                    //delete entry
                                    _DBContext.Entry(leave).State = EntityState.Deleted;
                                    _DBContext.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Belong To Any Leave Group. Contact Your System Administrator.";
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Leave Type";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Employee Does Not Exist";
                }       
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }



        public Task<StatusMessage> Delete(PayrollHrpayrollLeaveDetail leave, ApiToken token)
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
                    if(regInfo != null)
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

        public async Task<IEnumerable<PayrollHrpayrollLeaveDetail>> GetAll(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLeaveDetail.OrderByDescending(x => x.DateTo).Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
               
            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollLeaveDetail>> GetByEmployee(string Id, string Mode, ApiToken token)
        {
            try
            {
                if (Mode == "Current")
                {
                    return await _DBContext.PayrollHrpayrollLeaveDetail.OrderByDescending(x => x.DateTo).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id &&
                                                            x.DateTo >= DateTime.Now.Date).ToListAsync();
                }
                else if (Mode == "History")
                {
                    return await _DBContext.PayrollHrpayrollLeaveDetail.OrderByDescending(x => x.DateTo).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id &&
                                                            x.DateTo < DateTime.Now.Date).ToListAsync();
                }
                else
                {
                    return await _DBContext.PayrollHrpayrollLeaveDetail.OrderByDescending(x => x.DateTo).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PayrollHrpayrollLeaveDetail> GetById(string Id, string leaveType, DateTime startDate, DateTime endDate, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLeaveDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == Id &&
                                                        x.LeaveType == leaveType &&
                                                        x.DateFrom == startDate &&
                                                        x.DateTo == endDate).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollLeaveType>> GetLeaveTypeByEmployee(string Id, ApiToken token)
        {
            List<PayrollHrpayrollLeaveType> leaveTypes = new List<PayrollHrpayrollLeaveType>();
            try
            {
                var employee = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == Id).FirstOrDefaultAsync();

                var groupLeave = await _DBContext.PayrollHrpayrollGroupLeave.Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.GroupId == employee.GroupId).ToListAsync();

                foreach (var obj in groupLeave)
                {
                    var result = await _DBContext.PayrollHrpayrollLeaveType.Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.LeaveTypeId == obj.LeaveTypeId).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        if(result.LeaveGender == "" || result.LeaveGender == null || result.LeaveGender == employee.GenderId)
                        {
                            PayrollHrpayrollLeaveType leaveTypeObj = new PayrollHrpayrollLeaveType();

                            leaveTypeObj.CompanyId = obj.CompanyId;
                            leaveTypeObj.DivisionId = obj.DivisionId;
                            leaveTypeObj.DepartmentId = obj.DepartmentId;
                            leaveTypeObj.LeaveTypeId = obj.LeaveTypeId;
                            leaveTypeObj.Description = obj.Description == null || obj.Description == "" ? result.Description : obj.Description;
                            leaveTypeObj.Payable = result.Payable;
                            leaveTypeObj.OnPayroll = result.OnPayroll;
                            leaveTypeObj.LockedBy = obj.LockedBy;
                            leaveTypeObj.LockTs = obj.LockTs;
                            leaveTypeObj.CalcOnLeaveDays = result.CalcOnLeaveDays;
                            leaveTypeObj.MaximumDays = obj.LeaveDays == null || obj.LeaveDays == 0 ? (short)obj.LeaveDays : result.MaximumDays;
                            leaveTypeObj.MaxWorkingdays = result.MaxWorkingdays;
                            leaveTypeObj.UsagePeriod = result.UsagePeriod;
                            leaveTypeObj.BranchCode = result.BranchCode;
                            leaveTypeObj.LeaveGender = result.LeaveGender;

                            leaveTypes.Add(leaveTypeObj);
                        }
                    }
                }

                return leaveTypes;

                //inner join linq query to be reviewed later

                // var result  =  await (from x in _DBContext.PayrollEmployees
                //                         join y in _DBContext.PayrollHrpayrollGroupLeave 
                //                            on new { A = x.CompanyId, B = x.DivisionId, C = x.DepartmentId, D = x.GroupId } 
                //                            equals new { A = y.CompanyId, B = y.DivisionId, C = y.DepartmentId, D = y.GroupId }
                //                         join z in _DBContext.PayrollHrpayrollLeaveType
                //                            on new { A = x.CompanyId, B = x.DivisionId, C = x.DepartmentId, D = x.GenderId, E = y.LeaveTypeId }
                //                            equals new { A = z.CompanyId, B = z.DivisionId, C = z.DepartmentId, D = z.LeaveGender, E = z.LeaveTypeId }

                //                         select new
                //                         {
                //                                 CompanyId = z.CompanyId,
                //                                 DivisionId = z.DivisionId,
                //                                 DepartmentId = z.DepartmentId,
                //                                 LeaveTypeId = z.LeaveTypeId,
                //                                 Description = z.Description,
                //                                 Payable = z.Payable,
                //                                 OnPayroll = z.OnPayroll,
                //                                 LockedBy = z.LockedBy,
                //                                 LockTs = z.LockTs,
                //                                 CalcOnLeaveDays = z.CalcOnLeaveDays,
                //                                 MaximumDays = z.MaximumDays,
                //                                 MaxWorkingdays = z.MaxWorkingdays,
                //                                 UsagePeriod = z.UsagePeriod,
                //                                 BranchCode = z.BranchCode,
                //                                 LeaveGender = z.LeaveGender,
                //                                 EmployeeId = x.EmployeeId
                //                              }).Where(x => x.CompanyId == token.CompanyId &&
                //                                        x.DivisionId == token.DivisionId &&
                //                                        x.DepartmentId == token.DepartmentId &&
                //                                        x.EmployeeId == Id).ToListAsync();

                //foreach(var obj in result)
                //{
                //   PayrollHrpayrollLeaveType leaveTypeObj = new PayrollHrpayrollLeaveType();

                //   leaveTypeObj.CompanyId = obj.CompanyId;
                //   leaveTypeObj.DivisionId = obj.DivisionId;
                //   leaveTypeObj.DepartmentId = obj.DepartmentId;
                //   leaveTypeObj.LeaveTypeId = obj.LeaveTypeId;
                //   leaveTypeObj.Description = obj.Description;
                //   leaveTypeObj.Payable = obj.Payable;
                //   leaveTypeObj.OnPayroll = obj.OnPayroll;
                //   leaveTypeObj.LockedBy = obj.LockedBy;
                //   leaveTypeObj.LockTs = obj.LockTs;
                //   leaveTypeObj.CalcOnLeaveDays = obj.CalcOnLeaveDays;
                //   leaveTypeObj.MaximumDays = obj.MaximumDays;
                //   leaveTypeObj.MaxWorkingdays = obj.MaxWorkingdays;
                //   leaveTypeObj.UsagePeriod = obj.UsagePeriod;
                //   leaveTypeObj.BranchCode = obj.BranchCode;
                //   leaveTypeObj.LeaveGender = obj.LeaveGender;

                //   leaveTypes.Add(leaveTypeObj);
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<StatusMessage> Approve(LeaveAppModel leaveAppModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (leaveAppModel != null)
                {
                    var ssEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == leaveAppModel.ProcessBy &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();

                    var payrollEmployee = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == leaveAppModel.ProcessBy &&
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
                                    userPermissions.SscanApproveLeave = userPermissions.SscanApproveLeave == null || userPermissions.SscanApproveLeave == false ? false : true;
                                    userPermissions.SsisHr = userPermissions.SsisHr == null || userPermissions.SsisHr == false ? false : true;

                                    if (userPermissions.SscanApproveLeave == true)
                                    {
                                        var leave = await _DBContext.PayrollHrpayrollLeaveDetail
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == leaveAppModel.EmployeeId &&
                                                                   x.LeaveType == leaveAppModel.LeaveType).FirstOrDefaultAsync();
                                        if (leave.Approved != true)
                                        {
                                            //call procedure to approval;
                                            statusMessage = await approve(leaveAppModel, token);
                                        }
                                        else
                                        {
                                            statusMessage.Status = "Success";
                                            statusMessage.Message = "Success";
                                        }

                                        if (statusMessage.Status == "Success")
                                        {
                                            var leaveApproved = await _DBContext.PayrollHrpayrollLeaveDetail
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == leaveAppModel.EmployeeId &&
                                                                   x.LeaveType == leaveAppModel.LeaveType).FirstOrDefaultAsync();


                                            var leaveEmp = await _DBContext.PayrollEmployees
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == leaveAppModel.EmployeeId).FirstOrDefaultAsync();


                                            if (leaveEmp.EmployeeSecondLevelApprovedBy == leaveAppModel.ProcessBy)
                                            {
                                                leaveApproved.Audited = true;
                                                leaveApproved.AuditedBy = leaveAppModel.ProcessBy;
                                                leaveApproved.AuditedDate = Convert.ToDateTime(DateTime.Now.ToString());


                                                _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                                //_DBContext.SaveChanges();

                                            }

                                            if (leaveEmp.ExecutivePermision == true && userPermissions.SsisExecutive == true && userPermissions.EmployeeId == leaveAppModel.ProcessBy)
                                            {
                                                leaveApproved.ExecutivePermision = true;
                                                leaveApproved.ExecutivePermisionBy = leaveAppModel.ProcessBy;
                                                leaveApproved.ExecutiveApproveDate = Convert.ToDateTime(DateTime.Now.ToString());
                                                leaveApproved.ExecutivePermisionName = payrollEmployee.EmployeeFirstname + " " + payrollEmployee.EmployeeOthername + " " + payrollEmployee.EmployeeName;


                                                _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                                //_DBContext.SaveChanges();

                                            }

                                            if (userPermissions.SsisHr == true)
                                            {
                                                leaveApproved.Posted = true;
                                                leaveApproved.PostedBy = leaveAppModel.ProcessBy;
                                                leaveApproved.PostedDate = Convert.ToDateTime(DateTime.Now.ToString());
                                                leaveApproved.PostedByName = payrollEmployee.EmployeeFirstname + " " + payrollEmployee.EmployeeOthername + " " + payrollEmployee.EmployeeName;

                                                _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                                _DBContext.SaveChanges();

                                            }

                                            _DBContext.SaveChanges();

                                            //call procedure to post;
                                            //statusMessage = post(leaveAppModel, token);
                                        }
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "You dont have access to perform leave approval. Contact system Administrator.";

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
                        //get approval rights
                        var userPermissions = await _DBContext.AccessPermissions
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payrollEmployee.EmployeeId).FirstOrDefaultAsync();

                        if (userPermissions != null)
                        {
                            userPermissions.SscanApproveLeave = userPermissions.SscanApproveLeave == null || userPermissions.SscanApproveLeave == false ? false : true;
                            userPermissions.SsisHr = userPermissions.SsisHr == null || userPermissions.SsisHr == false ? false : true;
                            
                            if (userPermissions.SscanApproveLeave == true)
                            {
                                var leave = await _DBContext.PayrollHrpayrollLeaveDetail
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == leaveAppModel.EmployeeId &&
                                                           x.LeaveType == leaveAppModel.LeaveType).FirstOrDefaultAsync();
                                if (leave.Approved != true)
                                {
                                    //call procedure to approval;
                                    statusMessage = await approve(leaveAppModel, token);
                                }
                                else
                                {
                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }

                                if (statusMessage.Status == "Success")
                                {
                                    var leaveApproved = await _DBContext.PayrollHrpayrollLeaveDetail
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == leaveAppModel.EmployeeId &&
                                                           x.LeaveType == leaveAppModel.LeaveType).FirstOrDefaultAsync();


                                    var leaveEmp = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == leaveAppModel.EmployeeId).FirstOrDefaultAsync();


                                    if (leaveEmp.EmployeeSecondLevelApprovedBy == leaveAppModel.ProcessBy)
                                    {
                                        leaveApproved.Audited = true;
                                        leaveApproved.AuditedBy = leaveAppModel.ProcessBy;
                                        leaveApproved.AuditedDate = Convert.ToDateTime(DateTime.Now.ToString());
                                        

                                        _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                        //_DBContext.SaveChanges();

                                    }

                                    if (leaveEmp.ExecutivePermision == true && userPermissions.SsisExecutive == true && userPermissions.EmployeeId == leaveAppModel.ProcessBy)
                                    {
                                        leaveApproved.ExecutivePermision = true;
                                        leaveApproved.ExecutivePermisionBy = leaveAppModel.ProcessBy;
                                        leaveApproved.ExecutiveApproveDate = Convert.ToDateTime(DateTime.Now.ToString());
                                        leaveApproved.ExecutivePermisionName = payrollEmployee.EmployeeFirstname + " " + payrollEmployee.EmployeeOthername + " " + payrollEmployee.EmployeeName;


                                        _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                        //_DBContext.SaveChanges();

                                    }

                                    if (userPermissions.SsisHr == true)
                                    {
                                        leaveApproved.Posted = true;
                                        leaveApproved.PostedBy = leaveAppModel.ProcessBy;
                                        leaveApproved.PostedDate = Convert.ToDateTime(DateTime.Now.ToString());
                                        leaveApproved.PostedByName = payrollEmployee.EmployeeFirstname + " " + payrollEmployee.EmployeeOthername + " " + payrollEmployee.EmployeeName;

                                        _DBContext.Entry(leaveApproved).State = EntityState.Modified;
                                        _DBContext.SaveChanges();

                                    }

                                    _DBContext.SaveChanges();

                                    //call procedure to post;
                                    //statusMessage = post(leaveAppModel, token);
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "You dont have access to perform leave approval. Contact system Administrator.";

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
                        statusMessage.Message = "Invalid Approved By";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Leave does not exists.";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public Task<StatusMessage> Update(PayrollHrpayrollLeaveDetail leave, ApiToken token)
        {
            throw new NotImplementedException();
        }

        private StatusMessage submit(PayrollHrpayrollLeaveDetail leave, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sLeaveType = new SqlParameter("@LeaveType", leave.LeaveType);
                var sEmployeeID = new SqlParameter("@EmployeeID", leave.EmployeeId);
                var sDateFrom = new SqlParameter("@DateFrom", leave.DateFrom);
                var sDateTo = new SqlParameter("@DateTo", leave.DateTo);
                var sEnteredBy = new SqlParameter("@EnteredBy", leave.EmployeeId);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                _DBContext.Database
                          .ExecuteSqlRaw("enterprise.PayrollLeave_Cleared @CompanyID, @DivisionID, @DepartmentID, @LeaveType, @EmployeeID, @DateFrom, @DateTo, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sLeaveType, sEmployeeID, sDateFrom, sDateTo, sEnteredBy, PostingResult });

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
                statusMessage.Message = ex.ToString();
            }

            return statusMessage;
        }

        private async Task<StatusMessage> approve(LeaveAppModel leave, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sLeaveType = new SqlParameter("@LeaveType", leave.LeaveType);
                var sEmployeeID = new SqlParameter("@EmployeeID", leave.EmployeeId);
                var sDateFrom = new SqlParameter("@DateFrom", leave.DateFrom);
                var sDateTo = new SqlParameter("@DateTo", leave.DateTo);
                var sEnteredBy = new SqlParameter("@EnteredBy", leave.EmployeeId);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.PayrollLeave_Approve @CompanyID, @DivisionID, @DepartmentID, @LeaveType, @EmployeeID, @DateFrom, @DateTo, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sLeaveType, sEmployeeID, sDateFrom, sDateTo, sEnteredBy, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Leave Approved Successfully";
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
                statusMessage.Message = ex.ToString();
            }

            return statusMessage;
        }

    }
}
