using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;
using PowerAPI.Service.Helper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace PowerAPI.Service.Clients
{
    public class Employee : IEmployee
    {
        private readonly EnterpriseContext _DBContext;
        private readonly IApiAuthService _apiAuthService;


        MailSend _mailSend = new MailSend();
        private List<PayEmployees> payEmployees;

        public Employee(EnterpriseContext DBContext, IApiAuthService apiAuthService)
        {
            _DBContext = DBContext;
            _apiAuthService = apiAuthService;
        }
        public Task<StatusMessage> Add(PayrollEmployees employee, string Mode, ApiToken token)
        {
            throw new NotImplementedException();
        }


        public async Task<StatusMessage> ChangePwd(PasswordModel changePasswordModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            string newPwd = "";

            try
            {
                //check old password
                string oldPwd = EnterpriseExtras.doConvertPwd(changePasswordModel.OldPassword);

                var employee = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.EmployeeId == changePasswordModel.Username &&
                                                         x.EmployeePassword == oldPwd &&
                                                         x.ActiveYn == true &&
                                                         x.EmployeeTypeId != "User").FirstOrDefaultAsync();
                if (employee != null)
                {
                    //encrypt new password
                    newPwd = EnterpriseExtras.doConvertPwd(changePasswordModel.NewPassword);

                    employee.EmployeePassword = newPwd;
                    employee.EmployeePasswordOld = oldPwd;
                    employee.EmployeePasswordDate = DateTime.Now;

                    _DBContext.Entry(employee).State = EntityState.Modified;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Incorrect Username and/or Password";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public Task<StatusMessage> Delete(PayrollEmployees employee, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiToken> GetAccess(string token)
        {
            //int days = 0;
            //ApiToken apiToken = new ApiToken();
            //var regInfo = new TblVersion();
            //string cacheKey = "api_token";
            //string regCache = "reg_cache";

            try
            {

                /// To prevent repetition of logic, all Access Token validation depends on ApiAuthService
                return await _apiAuthService.GetAccess(token);

                //if(!_cache.TryGetValue(cacheKey, out apiToken) || apiToken is null)
                //{
                //    //get the comp on token
                //    apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                //    // Save the data in the cache
                //    _cache.Set(cacheKey, apiToken, TimeSpan.FromMinutes(10));
                //}

                //if (apiToken != null)
                //{
                //    if (!_cache.TryGetValue(regCache, out regInfo) || regInfo is null)
                //    {
                //        //get reg info
                //        regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                //                                            x.DivisionId == apiToken.DivisionId &&
                //                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();

                //        // Save the data in the cache
                //        _cache.Set(regCache, regInfo, TimeSpan.FromMinutes(10));
                //    }

                //    if (regInfo != null)
                //    {
                //        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                //        apiToken.RegCode = regInfo.RegCode;
                //        apiToken.RegCode = regInfo.RegName;
                //        apiToken.TotalDays = days;
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }

            //return apiToken;
        }

        public async Task<IEnumerable<PayEmployees>> GetAll(string Mode, ApiToken token)
        {
            List<PayEmployees> payEmployees = new List<PayEmployees>();
            List<PayrollEmployees> employees = new List<PayrollEmployees>();

            try
            {
                if (Mode == "Active")
                {
                    employees = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ActiveYn == true &&
                                                        x.EmployeeTypeId != "User").ToListAsync();

                }
                else if (Mode == "InActive")
                {
                    employees = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ActiveYn == false &&
                                                        x.EmployeeTypeId != "User").ToListAsync();
                }
                else
                {
                    employees = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeTypeId != "User").ToListAsync();
                }


                //generate full employee records
                payEmployees = await popEmployeeDataAsync(employees, token);




            }
            catch (Exception ex)
            {
                throw;
            }

            return payEmployees;
        }
        private async Task<List<PayEmployees>> popEmployeeDataAsync(List<PayrollEmployees> popEmployeeData, ApiToken token)
        {
            List<PayEmployees> employees = new List<PayEmployees>();
            try
            {
                if (popEmployeeData != null)
                {
                    foreach (PayrollEmployees payEmployees in popEmployeeData)
                    {
                        PayEmployees payEmployeesObj = new PayEmployees();

                        payEmployeesObj.CompanyId = payEmployees.CompanyId;
                        payEmployeesObj.DivisionId = payEmployees.DivisionId;
                        payEmployeesObj.DepartmentId = payEmployees.DepartmentId;
                        payEmployeesObj.EmployeeId = payEmployees.EmployeeId;
                        payEmployeesObj.EmployeeTypeId = payEmployees.EmployeeTypeId;
                        payEmployeesObj.EmployeeUserName = payEmployees.EmployeeUserName;
                        payEmployeesObj.EmployeePassword = payEmployees.EmployeePassword;
                        payEmployeesObj.EmployeePasswordOld = payEmployees.EmployeePasswordOld;
                        payEmployeesObj.EmployeePasswordDate = payEmployees.EmployeePasswordDate;
                        payEmployeesObj.EmployeePasswordExpires = payEmployees.EmployeePasswordExpires;
                        payEmployeesObj.EmployeePasswordExpiresDate = payEmployees.EmployeePasswordExpiresDate;
                        payEmployeesObj.EmployeeName = payEmployees.EmployeeName;
                        payEmployeesObj.ActiveYn = payEmployees.ActiveYn;
                        payEmployeesObj.EmployeeAddress1 = payEmployees.EmployeeAddress1;
                        payEmployeesObj.EmployeeAddress2 = payEmployees.EmployeeAddress2;
                        payEmployeesObj.EmployeeCity = payEmployees.EmployeeCity;
                        payEmployeesObj.EmployeeState = payEmployees.EmployeeState;
                        payEmployeesObj.EmployeeZip = payEmployees.EmployeeZip;
                        payEmployeesObj.EmployeeCountry = payEmployees.EmployeeCountry;
                        payEmployeesObj.EmployeePhone = payEmployees.EmployeePhone;
                        payEmployeesObj.EmployeeFax = payEmployees.EmployeeFax;
                        payEmployeesObj.EmployeeSsnumber = payEmployees.EmployeeSsnumber;
                        payEmployeesObj.EmployeeEmailAddress = payEmployees.EmployeeEmailAddress;
                        payEmployeesObj.EmployeeDepartmentId = payEmployees.EmployeeDepartmentId;
                        payEmployeesObj.PictureUrl = payEmployees.PictureUrl;
                        payEmployeesObj.HireDate = payEmployees.HireDate;
                        payEmployeesObj.Birthday = payEmployees.Birthday;
                        payEmployeesObj.Commissionable = payEmployees.Commissionable;
                        payEmployeesObj.CommissionPerc = payEmployees.CommissionPerc;
                        payEmployeesObj.CommissionCalcMethod = payEmployees.CommissionCalcMethod;
                        payEmployeesObj.EmployeeManager = payEmployees.EmployeeManager;
                        payEmployeesObj.EmployeeRegionId = payEmployees.EmployeeRegionId;
                        payEmployeesObj.EmployeeSourceId = payEmployees.EmployeeSourceId;
                        payEmployeesObj.EmployeeIndustryId = payEmployees.EmployeeIndustryId;
                        payEmployeesObj.Notes = payEmployees.Notes;
                        payEmployeesObj.NextOfKinName = payEmployees.NextOfKinName;
                        payEmployeesObj.NextOfKinNumber = payEmployees.NextOfKinNumber;
                        payEmployeesObj.MonthToDateGross = payEmployees.MonthToDateGross;
                        payEmployeesObj.MonthToDateFica = payEmployees.MonthToDateFica;
                        payEmployeesObj.MonthToDateFederal = payEmployees.MonthToDateFederal;
                        payEmployeesObj.MonthToDateState = payEmployees.MonthToDateState;
                        payEmployeesObj.MonthToDateLocal = payEmployees.MonthToDateLocal;
                        payEmployeesObj.MonthToDateOther = payEmployees.MonthToDateOther;
                        payEmployeesObj.QuarterToDateFederal = payEmployees.QuarterToDateFederal;
                        payEmployeesObj.QuarterToDateGross = payEmployees.QuarterToDateGross;
                        payEmployeesObj.QuarterToDateFica = payEmployees.QuarterToDateFica;
                        payEmployeesObj.QuarterToDateState = payEmployees.QuarterToDateState;
                        payEmployeesObj.QuarterToDateLocal = payEmployees.QuarterToDateLocal;
                        payEmployeesObj.QuarterToDateOther = payEmployees.QuarterToDateOther;
                        payEmployeesObj.YearToDateGross = payEmployees.YearToDateGross;
                        payEmployeesObj.YearToDateFica = payEmployees.YearToDateFica;
                        payEmployeesObj.YearToDateFederal = payEmployees.YearToDateFederal;
                        payEmployeesObj.YearToDateState = payEmployees.YearToDateState;
                        payEmployeesObj.YearToDateLocal = payEmployees.YearToDateLocal;
                        payEmployeesObj.YearToDateOther = payEmployees.YearToDateOther;
                        payEmployeesObj.EmployeeCounty = payEmployees.EmployeeCounty;
                        payEmployeesObj.Approved = payEmployees.Approved;
                        payEmployeesObj.ApprovedBy = payEmployees.ApprovedBy;
                        payEmployeesObj.ApprovedDate = payEmployees.ApprovedDate;
                        payEmployeesObj.EnteredBy = payEmployees.EnteredBy;
                        payEmployeesObj.LockedBy = payEmployees.LockedBy;
                        payEmployeesObj.LastEditDate = payEmployees.LastEditDate;
                        payEmployeesObj.CreationDate = payEmployees.CreationDate;
                        payEmployeesObj.DocumentId = payEmployees.DocumentId;
                        payEmployeesObj.EmployeeFirstname = payEmployees.EmployeeFirstname;
                        payEmployeesObj.EmployeeOthername = payEmployees.EmployeeOthername;
                        payEmployeesObj.TitleId = payEmployees.TitleId;
                        payEmployeesObj.EmployeeHomephone = payEmployees.EmployeeHomephone;
                        payEmployeesObj.EmployeeOfficephone = payEmployees.EmployeeOfficephone;
                        payEmployeesObj.EmployeeOfficeExt = payEmployees.EmployeeOfficeExt;
                        payEmployeesObj.EmployeeAddress3 = payEmployees.EmployeeAddress3;
                        payEmployeesObj.EmployeeAddress4 = payEmployees.EmployeeAddress4;
                        payEmployeesObj.EmployeeAddress5 = payEmployees.EmployeeAddress5;
                        payEmployeesObj.Pfaid = payEmployees.Pfaid;
                        payEmployeesObj.BankId = payEmployees.BankId;
                        payEmployeesObj.DesignationId = payEmployees.DesignationId;
                        payEmployeesObj.ContractTypeId = payEmployees.ContractTypeId;
                        payEmployeesObj.CurrencyId = payEmployees.CurrencyId;
                        payEmployeesObj.EmployeeMaidenName = payEmployees.EmployeeMaidenName;
                        payEmployeesObj.Disabled = payEmployees.Disabled;
                        payEmployeesObj.Pfano = payEmployees.Pfano;
                        payEmployeesObj.GenderId = payEmployees.GenderId;
                        payEmployeesObj.StatusId = payEmployees.StatusId;
                        payEmployeesObj.StateId = payEmployees.StateId;
                        payEmployeesObj.NationalityId = payEmployees.NationalityId;
                        payEmployeesObj.LanguageId = payEmployees.LanguageId;
                        payEmployeesObj.GradeId = payEmployees.GradeId;
                        payEmployeesObj.AccountNo = payEmployees.AccountNo;
                        payEmployeesObj.GroupId = payEmployees.GroupId;
                        payEmployeesObj.Taxable = payEmployees.Taxable;
                        payEmployeesObj.JobClassId = payEmployees.JobClassId;
                        payEmployeesObj.Location = payEmployees.Location;
                        payEmployeesObj.CostCenter = payEmployees.CostCenter;
                        payEmployeesObj.BaseValue = payEmployees.BaseValue;
                        payEmployeesObj.MonthlyTax = payEmployees.MonthlyTax;
                        payEmployeesObj.YearToDateTax = payEmployees.YearToDateTax;
                        payEmployeesObj.Department = payEmployees.Department;
                        payEmployeesObj.PromotionDate = payEmployees.PromotionDate;
                        payEmployeesObj.WorkedDays = payEmployees.WorkedDays;
                        payEmployeesObj.LastPayDate = payEmployees.LastPayDate;
                        payEmployeesObj.PensionDate = payEmployees.PensionDate;
                        payEmployeesObj.StatePaye = payEmployees.StatePaye;
                        payEmployeesObj.Gross = payEmployees.Gross;
                        payEmployeesObj.ConfirmationDate = payEmployees.ConfirmationDate;
                        payEmployeesObj.GroupDescription = payEmployees.GroupDescription;
                        payEmployeesObj.Picture = payEmployees.Picture;
                        payEmployeesObj.Cleared = payEmployees.Cleared;
                        payEmployeesObj.EmployeeActivityTypeId = payEmployees.EmployeeActivityTypeId;
                        payEmployeesObj.ProvideForTax = payEmployees.ProvideForTax;
                        payEmployeesObj.PayrollEmployeeUserGroupId = payEmployees.PayrollEmployeeUserGroupId;
                        payEmployeesObj.EmployeePostalAddress = payEmployees.EmployeePostalAddress;
                        payEmployeesObj.Lga = payEmployees.Lga;
                        payEmployeesObj.GeoArea = payEmployees.GeoArea;
                        payEmployeesObj.BnkSortCode = payEmployees.BnkSortCode;
                        payEmployeesObj.TaxNumber = payEmployees.TaxNumber;
                        payEmployeesObj.ExecutivePermision = payEmployees.ExecutivePermision;
                        payEmployeesObj.SplitPay = payEmployees.SplitPay;
                        payEmployeesObj.SplitPayPercent = payEmployees.SplitPayPercent;
                        payEmployeesObj.EmployeeSecondLevelApprovedBy = payEmployees.EmployeeSecondLevelApprovedBy;
                        payEmployeesObj.EmployeeSignature = payEmployees.EmployeeSignature;
                        payEmployeesObj.RoleId = payEmployees.RoleId;
                        payEmployeesObj.BaseValueOld = payEmployees.BaseValueOld;
                        payEmployeesObj.IsSupervisor = payEmployees.IsSupervisor;
                        payEmployeesObj.PinPassword = payEmployees.PinPassword;
                        payEmployeesObj.EmpManagerName = payEmployees.EmpManagerName;
                        payEmployeesObj.EmpSecondManagerName = payEmployees.EmpSecondManagerName;

                        payEmployeesObj.payrollEmployeesDetail = await _DBContext.PayrollEmployeesDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.warehouseByEmployees = await _DBContext.WarehouseByEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollPayElementsDetail = await _DBContext.PayrollHrpayrollPayElementsDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollQualification = await _DBContext.PayrollHrpayrollQualification.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollCareer = await _DBContext.PayrollHrpayrollCareer.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollNextOfKin = await _DBContext.PayrollHrpayrollNextOfKin.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollDependantRatio = await _DBContext.PayrollHrpayrollDependantRatio.Where(x => x.CompanyId == token.CompanyId &&
                                                          x.DivisionId == token.DivisionId &&
                                                          x.DepartmentId == token.DepartmentId &&
                                                          x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollTransactionElement = await _DBContext.PayrollHrpayrollTransactionElement.Where(x => x.CompanyId == token.CompanyId &&
                                                          x.DivisionId == token.DivisionId &&
                                                          x.DepartmentId == token.DepartmentId &&
                                                          x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollEmployeeReferee = await _DBContext.PayrollHrpayrollEmployeeReferee.Where(x => x.CompanyId == token.CompanyId &&
                                                          x.DivisionId == token.DivisionId &&
                                                          x.DepartmentId == token.DepartmentId &&
                                                          x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollEmployeePFA = await _DBContext.PayrollHrpayrollEmployeePfa.Where(x => x.CompanyId == token.CompanyId &&
                                                          x.DivisionId == token.DivisionId &&
                                                          x.DepartmentId == token.DepartmentId &&
                                                          x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollPromotion = await _DBContext.PayrollHrpayrollPromotion.Where(x => x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollDocumentation = await _DBContext.PayrollHrpayrollDocumentation.Where(x => x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.EmployeeId == payEmployees.EmployeeId).ToListAsync();
                        payEmployeesObj.payrollHrPayrollJobClassElement = await _DBContext.PayrollHrpayrollJobClassElement.Where(x => x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.EmployeeId == payEmployees.EmployeeId).ToListAsync();


                        employees.Add(payEmployeesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return employees;

        }

        public async Task<StatusMessage> AddEmployees(PayEmployees employeePolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<PayrollEmployeesDetail> payrollEmployeesDetails = new List<PayrollEmployeesDetail>();
            List<WarehouseByEmployees> warehouseByEmployees = new List<WarehouseByEmployees>();
            List<PayrollHrpayrollPayElementsDetail> payrollHrPayrollPayElementsDetail = new List<PayrollHrpayrollPayElementsDetail>();
            List<PayrollHrpayrollQualification> payrollHrPayrollQualification = new List<PayrollHrpayrollQualification>();
            List<PayrollHrpayrollCareer> payrollHrPayrollCareer = new List<PayrollHrpayrollCareer>();
            List<PayrollHrpayrollNextOfKin> payrollHrPayrollNextOfKin = new List<PayrollHrpayrollNextOfKin>();
            List<PayrollHrpayrollDependantRatio> payrollHrPayrollDependantRatio = new List<PayrollHrpayrollDependantRatio>();
            List<PayrollHrpayrollTransactionElement> payrollHrPayrollTransactionElement = new List<PayrollHrpayrollTransactionElement>();
            List<PayrollHrpayrollEmployeeReferee> payrollHrPayrollEmployeeReferee = new List<PayrollHrpayrollEmployeeReferee>();
            List<PayrollHrpayrollEmployeePfa> payrollHrPayrollEmployeePFA = new List<PayrollHrpayrollEmployeePfa>();
            List<PayrollHrpayrollPromotion> payrollHrPayrollPromotion = new List<PayrollHrpayrollPromotion>();
            List<PayrollHrpayrollDocumentation> payrollHrPayrollDocumentation = new List<PayrollHrpayrollDocumentation>();
            List<PayrollHrpayrollJobClassElement> payrollHrPayrollJobClassElement = new List<PayrollHrpayrollJobClassElement>();
            List<PayrollEmployees> PayrollEmployees = new List<PayrollEmployees>();
            try
            {
                //check if object empty
                if (employeePolicy != null)
                {
                    var payrollEmployee = await _DBContext.PayrollEmployees.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == employeePolicy.EmployeeId).FirstOrDefaultAsync();

                    if (payrollEmployee != null) // for update payroll policy
                    {
                        payrollEmployee.EmployeeTypeId = employeePolicy.EmployeeTypeId;
                        payrollEmployee.EmployeeUserName = employeePolicy.EmployeeUserName;
                        payrollEmployee.EmployeePassword = employeePolicy.EmployeePassword;
                        payrollEmployee.EmployeePasswordOld = employeePolicy.EmployeePasswordOld;
                        payrollEmployee.EmployeePasswordDate = employeePolicy.EmployeePasswordDate;
                        payrollEmployee.EmployeePasswordExpires = employeePolicy.EmployeePasswordExpires;
                        payrollEmployee.EmployeePasswordExpiresDate = employeePolicy.EmployeePasswordExpiresDate;
                        payrollEmployee.EmployeeName = employeePolicy.EmployeeName;
                        payrollEmployee.ActiveYn = employeePolicy.ActiveYn;
                        payrollEmployee.EmployeeAddress1 = employeePolicy.EmployeeAddress1;
                        payrollEmployee.EmployeeAddress2 = employeePolicy.EmployeeAddress2;
                        payrollEmployee.EmployeeCity = employeePolicy.EmployeeCity;
                        payrollEmployee.EmployeeState = employeePolicy.EmployeeState;
                        payrollEmployee.EmployeeZip = employeePolicy.EmployeeZip;
                        payrollEmployee.EmployeeCountry = employeePolicy.EmployeeCountry;
                        payrollEmployee.EmployeePhone = employeePolicy.EmployeePhone;
                        payrollEmployee.EmployeeFax = employeePolicy.EmployeeFax;
                        payrollEmployee.EmployeeSsnumber = employeePolicy.EmployeeSsnumber;
                        payrollEmployee.EmployeeEmailAddress = employeePolicy.EmployeeEmailAddress;
                        payrollEmployee.EmployeeDepartmentId = employeePolicy.EmployeeDepartmentId;
                        payrollEmployee.PictureUrl = employeePolicy.PictureUrl;
                        payrollEmployee.HireDate = employeePolicy.HireDate;
                        payrollEmployee.Birthday = employeePolicy.Birthday;
                        payrollEmployee.Commissionable = employeePolicy.Commissionable;
                        payrollEmployee.CommissionPerc = employeePolicy.CommissionPerc;
                        payrollEmployee.CommissionCalcMethod = employeePolicy.CommissionCalcMethod;
                        payrollEmployee.EmployeeManager = employeePolicy.EmployeeManager;
                        payrollEmployee.EmployeeRegionId = employeePolicy.EmployeeRegionId;
                        payrollEmployee.EmployeeSourceId = employeePolicy.EmployeeSourceId;
                        payrollEmployee.EmployeeIndustryId = employeePolicy.EmployeeIndustryId;
                        payrollEmployee.Notes = employeePolicy.Notes;
                        payrollEmployee.NextOfKinName = employeePolicy.NextOfKinName;
                        payrollEmployee.NextOfKinNumber = employeePolicy.NextOfKinNumber;
                        payrollEmployee.MonthToDateGross = employeePolicy.MonthToDateGross;
                        payrollEmployee.MonthToDateFica = employeePolicy.MonthToDateFica;
                        payrollEmployee.MonthToDateFederal = employeePolicy.MonthToDateFederal;
                        payrollEmployee.MonthToDateState = employeePolicy.MonthToDateState;
                        payrollEmployee.MonthToDateLocal = employeePolicy.MonthToDateLocal;
                        payrollEmployee.MonthToDateOther = employeePolicy.MonthToDateOther;
                        payrollEmployee.QuarterToDateGross = employeePolicy.QuarterToDateGross;
                        payrollEmployee.QuarterToDateFica = employeePolicy.QuarterToDateFica;
                        payrollEmployee.QuarterToDateFederal = employeePolicy.QuarterToDateFederal;
                        payrollEmployee.QuarterToDateState = employeePolicy.QuarterToDateState;
                        payrollEmployee.QuarterToDateLocal = employeePolicy.QuarterToDateLocal;
                        payrollEmployee.QuarterToDateOther = employeePolicy.QuarterToDateOther;
                        payrollEmployee.YearToDateGross = employeePolicy.YearToDateGross;
                        payrollEmployee.YearToDateFica = employeePolicy.YearToDateFica;
                        payrollEmployee.YearToDateFederal = employeePolicy.YearToDateFederal;
                        payrollEmployee.YearToDateState = employeePolicy.YearToDateState;
                        payrollEmployee.YearToDateLocal = employeePolicy.YearToDateLocal;
                        payrollEmployee.YearToDateOther = employeePolicy.YearToDateOther;
                        payrollEmployee.EmployeeCounty = employeePolicy.EmployeeCounty;
                        payrollEmployee.Approved = employeePolicy.Approved;
                        payrollEmployee.ApprovedBy = employeePolicy.ApprovedBy;
                        payrollEmployee.ApprovedDate = employeePolicy.ApprovedDate;
                        payrollEmployee.EnteredBy = employeePolicy.EnteredBy;
                        payrollEmployee.LockedBy = employeePolicy.LockedBy;
                        payrollEmployee.LockTs = employeePolicy.LockTs;
                        payrollEmployee.LastEditDate = employeePolicy.LastEditDate;
                        payrollEmployee.CreationDate = employeePolicy.CreationDate;
                        payrollEmployee.DocumentId = employeePolicy.DocumentId;
                        payrollEmployee.EmployeeFirstname = employeePolicy.EmployeeFirstname;
                        payrollEmployee.EmployeeOthername = employeePolicy.EmployeeOthername;
                        payrollEmployee.TitleId = employeePolicy.TitleId;
                        payrollEmployee.EmployeeHomephone = employeePolicy.EmployeeHomephone;
                        payrollEmployee.EmployeeOfficephone = employeePolicy.EmployeeOfficephone;
                        payrollEmployee.EmployeeOfficeExt = employeePolicy.EmployeeOfficeExt;
                        payrollEmployee.EmployeeAddress3 = employeePolicy.EmployeeAddress3;
                        payrollEmployee.EmployeeAddress4 = employeePolicy.EmployeeAddress4;
                        payrollEmployee.EmployeeAddress5 = employeePolicy.EmployeeAddress5;
                        payrollEmployee.Pfaid = employeePolicy.Pfaid;
                        payrollEmployee.BankId = employeePolicy.BankId;
                        payrollEmployee.DesignationId = employeePolicy.DesignationId;
                        payrollEmployee.EmployeeManagerId = employeePolicy.EmployeeManagerId;
                        payrollEmployee.ContractTypeId = employeePolicy.ContractTypeId;
                        payrollEmployee.EmployeeMaidenName = employeePolicy.EmployeeMaidenName;
                        payrollEmployee.Disabled = employeePolicy.Disabled;
                        payrollEmployee.Pfano = employeePolicy.Pfano;
                        payrollEmployee.GenderId = employeePolicy.GenderId;
                        payrollEmployee.StatusId = employeePolicy.StatusId;
                        payrollEmployee.StateId = employeePolicy.StateId;
                        payrollEmployee.NationalityId = employeePolicy.NationalityId;
                        payrollEmployee.LanguageId = employeePolicy.LanguageId;
                        payrollEmployee.GroupId = employeePolicy.GroupId;
                        payrollEmployee.AccountNo = employeePolicy.AccountNo;
                        payrollEmployee.GradeId = employeePolicy.GradeId;
                        payrollEmployee.Taxable = employeePolicy.Taxable;
                        payrollEmployee.JobClassId = employeePolicy.JobClassId;
                        payrollEmployee.Location = employeePolicy.Location;
                        payrollEmployee.CostCenter = employeePolicy.CostCenter;
                        payrollEmployee.BaseValue = employeePolicy.BaseValue;
                        payrollEmployee.MonthlyTax = employeePolicy.MonthlyTax;
                        payrollEmployee.YearToDateTax = employeePolicy.YearToDateTax;
                        payrollEmployee.Department = employeePolicy.Department;
                        payrollEmployee.PromotionDate = employeePolicy.PromotionDate;
                        payrollEmployee.WorkedDays = employeePolicy.WorkedDays;
                        payrollEmployee.LastPayDate = employeePolicy.LastPayDate;
                        payrollEmployee.PensionDate = employeePolicy.PensionDate;
                        payrollEmployee.StatePaye = employeePolicy.StatePaye;
                        payrollEmployee.Gross = employeePolicy.Gross;
                        payrollEmployee.ConfirmationDate = employeePolicy.ConfirmationDate;
                        payrollEmployee.GroupDescription = employeePolicy.GroupDescription;
                        payrollEmployee.Picture = employeePolicy.Picture;
                        payrollEmployee.Cleared = employeePolicy.Cleared;
                        payrollEmployee.EmployeeActivityTypeId = employeePolicy.EmployeeActivityTypeId;
                        payrollEmployee.ProvideForTax = employeePolicy.ProvideForTax;
                        payrollEmployee.PayrollEmployeeUserGroupId = employeePolicy.PayrollEmployeeUserGroupId;
                        payrollEmployee.EmployeePostalAddress = employeePolicy.EmployeePostalAddress;
                        payrollEmployee.BranchCode = employeePolicy.BranchCode;
                        payrollEmployee.Lga = employeePolicy.Lga;
                        payrollEmployee.GeoArea = employeePolicy.GeoArea;
                        payrollEmployee.BnkSortCode = employeePolicy.BnkSortCode;
                        payrollEmployee.TaxNumber = employeePolicy.TaxNumber;
                        payrollEmployee.ExecutivePermision = employeePolicy.ExecutivePermision;
                        payrollEmployee.SplitPay = employeePolicy.SplitPay;
                        payrollEmployee.SplitPayPercent = employeePolicy.SplitPayPercent;
                        payrollEmployee.EmployeeSecondLevelApprovedBy = employeePolicy.EmployeeSecondLevelApprovedBy;
                        payrollEmployee.EmployeeSignature = employeePolicy.EmployeeSignature;
                        payrollEmployee.BaseValueOld = employeePolicy.BaseValueOld;
                        payrollEmployee.RoleId = employeePolicy.RoleId;
                        payrollEmployee.IsSupervisor = employeePolicy.IsSupervisor;
                        payrollEmployee.PinPassword = employeePolicy.PinPassword;
                        payrollEmployee.EmpManagerName = employeePolicy.EmpManagerName;
                        payrollEmployee.EmpSecondManagerName = employeePolicy.EmpSecondManagerName;

                        _DBContext.Entry(payrollEmployee).State = EntityState.Modified;

                        if (employeePolicy.payrollEmployeesDetail != null && employeePolicy.payrollEmployeesDetail.Count > 0)
                        {
                            foreach (PayrollEmployeesDetail empDetail in employeePolicy.payrollEmployeesDetail)
                            {
                                var payrollEmployeesDetailObj = await _DBContext.PayrollEmployeesDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == empDetail.EmployeeId &&
                                                        x.PayYn == empDetail.PayYn).FirstOrDefaultAsync();

                                if (payrollEmployeesDetailObj != null)
                                {
                                    payrollEmployeesDetailObj.PayType = empDetail.PayType;
                                    payrollEmployeesDetailObj.PayFrequency = empDetail.PayFrequency;
                                    payrollEmployeesDetailObj.Salary = empDetail.Salary;
                                    payrollEmployeesDetailObj.HourlyRate = empDetail.HourlyRate;
                                    payrollEmployeesDetailObj.CommissionCalc = empDetail.CommissionCalc;
                                    payrollEmployeesDetailObj.ComissionPerc = empDetail.ComissionPerc;
                                    payrollEmployeesDetailObj.OvertimeRate = empDetail.OvertimeRate;
                                    payrollEmployeesDetailObj.Ficayn = empDetail.Ficayn;
                                    payrollEmployeesDetailObj.Fityn = empDetail.Ficayn;
                                    payrollEmployeesDetailObj.FicamedYn = empDetail.FicamedYn;
                                    payrollEmployeesDetailObj.Futayn = empDetail.Futayn;
                                    payrollEmployeesDetailObj.Sutayn = empDetail.Sutayn;
                                    payrollEmployeesDetailObj.Sdiyn = empDetail.Sdiyn;
                                    payrollEmployeesDetailObj.LocalYn = empDetail.Sdiyn;
                                    payrollEmployeesDetailObj.Luiyn = empDetail.Sdiyn;
                                    payrollEmployeesDetailObj.FederalAllowance = empDetail.FederalAllowance;
                                    payrollEmployeesDetailObj.StateAllowance = empDetail.StateAllowance;
                                    payrollEmployeesDetailObj.CountyAllowance = empDetail.CountyAllowance;
                                    payrollEmployeesDetailObj.CityAllowance = empDetail.CityAllowance;
                                    payrollEmployeesDetailObj.FederalWithholdingAmount = empDetail.FederalWithholdingAmount;
                                    payrollEmployeesDetailObj.StateWithholdingAmount = empDetail.StateWithholdingAmount;
                                    payrollEmployeesDetailObj.CountyWithhoddingAmount = empDetail.CountyWithhoddingAmount;
                                    payrollEmployeesDetailObj.CityWithhoddingAmount = empDetail.CityWithhoddingAmount;
                                    payrollEmployeesDetailObj.OtherAmount = empDetail.OtherAmount;
                                    payrollEmployeesDetailObj.FederalFilingStatus = empDetail.FederalFilingStatus;
                                    payrollEmployeesDetailObj.StateFilingStatus = empDetail.StateFilingStatus;
                                    payrollEmployeesDetailObj.CountyFilingStatus = empDetail.CountyFilingStatus;
                                    payrollEmployeesDetailObj.CityFilingStatus = empDetail.CityFilingStatus;
                                    payrollEmployeesDetailObj.OtherStatus = empDetail.OtherStatus;
                                    payrollEmployeesDetailObj.Dependents = empDetail.Dependents;
                                    payrollEmployeesDetailObj.MaleFemale = empDetail.MaleFemale;
                                    payrollEmployeesDetailObj.Amount = empDetail.Amount;
                                    payrollEmployeesDetailObj.NetAmount = empDetail.NetAmount;
                                    payrollEmployeesDetailObj.Additions = empDetail.Additions;
                                    payrollEmployeesDetailObj.Deductions = empDetail.Deductions;
                                    payrollEmployeesDetailObj.PreTaxedAmount = empDetail.PreTaxedAmount;
                                    payrollEmployeesDetailObj.BlankCheckHourlyRate = empDetail.BlankCheckHourlyRate;
                                    payrollEmployeesDetailObj.BlankCheckOvertimeRate = empDetail.BlankCheckOvertimeRate;
                                    payrollEmployeesDetailObj.YearToDateGross = empDetail.YearToDateGross;
                                    payrollEmployeesDetailObj.YearToDateAgi = empDetail.YearToDateAgi;
                                    payrollEmployeesDetailObj.YearToDateFica = empDetail.YearToDateFica;
                                    payrollEmployeesDetailObj.YearToDateFicamed = empDetail.YearToDateFicamed;
                                    payrollEmployeesDetailObj.YearToDateFit = empDetail.YearToDateFit;
                                    payrollEmployeesDetailObj.YearToDateFuta = empDetail.YearToDateFuta;
                                    payrollEmployeesDetailObj.YearToDateSuta = empDetail.YearToDateSuta;
                                    payrollEmployeesDetailObj.YearToDateSit = empDetail.YearToDateSit;
                                    payrollEmployeesDetailObj.YearToDateSdi = empDetail.YearToDateSdi;
                                    payrollEmployeesDetailObj.YearToDateLocal = empDetail.YearToDateLocal;
                                    payrollEmployeesDetailObj.YearToDateLui = empDetail.YearToDateLui;
                                    payrollEmployeesDetailObj.LastGross = empDetail.LastGross;
                                    payrollEmployeesDetailObj.LastAgi = empDetail.LastAgi;
                                    payrollEmployeesDetailObj.LastFica = empDetail.LastFica;
                                    payrollEmployeesDetailObj.LastFicamed = empDetail.LastFicamed;
                                    payrollEmployeesDetailObj.LastFit = empDetail.LastFit;
                                    payrollEmployeesDetailObj.LastFuta = empDetail.LastFuta;
                                    payrollEmployeesDetailObj.LastSuta = empDetail.LastSuta;
                                    payrollEmployeesDetailObj.LastSit = empDetail.LastSit;
                                    payrollEmployeesDetailObj.LastSdi = empDetail.LastSdi;
                                    payrollEmployeesDetailObj.LastLocal = empDetail.LastLocal;
                                    payrollEmployeesDetailObj.LastLui = empDetail.LastLui;
                                    payrollEmployeesDetailObj.LastCommissionAmount = empDetail.LastCommissionAmount;
                                    payrollEmployeesDetailObj.YearToDateRegularHours = empDetail.YearToDateRegularHours;
                                    payrollEmployeesDetailObj.YearToDateOvertimeHours = empDetail.YearToDateOvertimeHours;
                                    payrollEmployeesDetailObj.LastHours = empDetail.LastHours;
                                    payrollEmployeesDetailObj.LastPayDate = empDetail.LastPayDate;
                                    payrollEmployeesDetailObj.LockedBy = empDetail.LockedBy;
                                    payrollEmployeesDetailObj.LockTs = empDetail.LockTs;
                                    payrollEmployeesDetailObj.BranchCode = empDetail.BranchCode;

                                    _DBContext.Entry(payrollEmployeesDetailObj).State = EntityState.Modified;
                                }
                                else
                                {
                                    empDetail.CompanyId = token.CompanyId;
                                    empDetail.DivisionId = token.DivisionId;
                                    empDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(empDetail).State = EntityState.Added;
                                }
                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        PayrollEmployees payEmployees = new PayrollEmployees();

                        payEmployees.CompanyId = token.CompanyId;
                        payEmployees.DivisionId = token.DivisionId;
                        payEmployees.DepartmentId = token.DepartmentId;

                        payEmployees.EmployeeId = employeePolicy.EmployeeId;
                        payEmployees.EmployeeTypeId = employeePolicy.EmployeeTypeId;
                        payEmployees.EmployeeUserName = employeePolicy.EmployeeUserName;
                        payEmployees.EmployeePassword = employeePolicy.EmployeePassword;
                        payEmployees.EmployeePasswordOld = employeePolicy.EmployeePasswordOld;
                        payEmployees.EmployeePasswordDate = employeePolicy.EmployeePasswordDate;
                        payEmployees.EmployeePasswordExpires = employeePolicy.EmployeePasswordExpires;
                        payEmployees.EmployeePasswordExpiresDate = employeePolicy.EmployeePasswordExpiresDate;
                        payEmployees.EmployeeName = employeePolicy.EmployeeName;
                        payEmployees.ActiveYn = employeePolicy.ActiveYn;
                        payEmployees.EmployeeAddress1 = employeePolicy.EmployeeAddress1;
                        payEmployees.EmployeeAddress2 = employeePolicy.EmployeeAddress2;
                        payEmployees.EmployeeCity = employeePolicy.EmployeeCity;
                        payEmployees.EmployeeState = employeePolicy.EmployeeState;
                        payEmployees.EmployeeZip = employeePolicy.EmployeeZip;
                        payEmployees.EmployeeCountry = employeePolicy.EmployeeCountry;
                        payEmployees.EmployeePhone = employeePolicy.EmployeePhone;
                        payEmployees.EmployeeFax = employeePolicy.EmployeeFax;
                        payEmployees.EmployeeSsnumber = employeePolicy.EmployeeSsnumber;
                        payEmployees.EmployeeEmailAddress = employeePolicy.EmployeeEmailAddress;
                        payEmployees.EmployeeDepartmentId = employeePolicy.EmployeeDepartmentId;
                        payEmployees.PictureUrl = employeePolicy.PictureUrl;
                        payEmployees.HireDate = employeePolicy.HireDate;
                        payEmployees.Birthday = employeePolicy.Birthday;
                        payEmployees.Commissionable = employeePolicy.Commissionable;
                        payEmployees.CommissionPerc = employeePolicy.CommissionPerc;
                        payEmployees.CommissionCalcMethod = employeePolicy.CommissionCalcMethod;
                        payEmployees.EmployeeManager = employeePolicy.EmployeeManager;
                        payEmployees.EmployeeRegionId = employeePolicy.EmployeeRegionId;
                        payEmployees.EmployeeSourceId = employeePolicy.EmployeeSourceId;
                        payEmployees.EmployeeIndustryId = employeePolicy.EmployeeIndustryId;
                        payEmployees.Notes = employeePolicy.Notes;
                        payEmployees.NextOfKinName = employeePolicy.NextOfKinName;
                        payEmployees.NextOfKinNumber = employeePolicy.NextOfKinNumber;
                        payEmployees.MonthToDateGross = employeePolicy.MonthToDateGross;
                        payEmployees.MonthToDateFica = employeePolicy.MonthToDateFica;
                        payEmployees.MonthToDateFederal = employeePolicy.MonthToDateFederal;
                        payEmployees.MonthToDateState = employeePolicy.MonthToDateState;
                        payEmployees.MonthToDateLocal = employeePolicy.MonthToDateLocal;
                        payEmployees.MonthToDateOther = employeePolicy.MonthToDateOther;
                        payEmployees.QuarterToDateGross = employeePolicy.QuarterToDateGross;
                        payEmployees.QuarterToDateFica = employeePolicy.QuarterToDateFica;
                        payEmployees.QuarterToDateFederal = employeePolicy.QuarterToDateFederal;
                        payEmployees.QuarterToDateState = employeePolicy.QuarterToDateState;
                        payEmployees.QuarterToDateLocal = employeePolicy.QuarterToDateLocal;
                        payEmployees.QuarterToDateOther = employeePolicy.QuarterToDateOther;
                        payEmployees.YearToDateGross = employeePolicy.YearToDateGross;
                        payEmployees.YearToDateFica = employeePolicy.YearToDateFica;
                        payEmployees.YearToDateFederal = employeePolicy.YearToDateFederal;
                        payEmployees.YearToDateState = employeePolicy.YearToDateState;
                        payEmployees.YearToDateLocal = employeePolicy.YearToDateLocal;
                        payEmployees.YearToDateOther = employeePolicy.YearToDateOther;
                        payEmployees.EmployeeCounty = employeePolicy.EmployeeCounty;
                        payEmployees.Approved = employeePolicy.Approved;
                        payEmployees.ApprovedBy = employeePolicy.ApprovedBy;
                        payEmployees.ApprovedDate = employeePolicy.ApprovedDate;
                        payEmployees.EnteredBy = employeePolicy.EnteredBy;
                        payEmployees.LockedBy = employeePolicy.LockedBy;
                        payEmployees.LockTs = employeePolicy.LockTs;
                        payEmployees.LastEditDate = employeePolicy.LastEditDate;
                        payEmployees.CreationDate = employeePolicy.CreationDate;
                        payEmployees.DocumentId = employeePolicy.DocumentId;
                        payEmployees.EmployeeFirstname = employeePolicy.EmployeeFirstname;
                        payEmployees.EmployeeOthername = employeePolicy.EmployeeOthername;
                        payEmployees.TitleId = employeePolicy.TitleId;
                        payEmployees.EmployeeHomephone = employeePolicy.EmployeeHomephone;
                        payEmployees.EmployeeOfficephone = employeePolicy.EmployeeOfficephone;
                        payEmployees.EmployeeOfficeExt = employeePolicy.EmployeeOfficeExt;
                        payEmployees.EmployeeAddress3 = employeePolicy.EmployeeAddress3;
                        payEmployees.EmployeeAddress4 = employeePolicy.EmployeeAddress4;
                        payEmployees.EmployeeAddress5 = employeePolicy.EmployeeAddress5;
                        payEmployees.Pfaid = employeePolicy.Pfaid;
                        payEmployees.BankId = employeePolicy.BankId;
                        payEmployees.DesignationId = employeePolicy.DesignationId;
                        payEmployees.EmployeeManagerId = employeePolicy.EmployeeManagerId;
                        payEmployees.ContractTypeId = employeePolicy.ContractTypeId;
                        payEmployees.EmployeeMaidenName = employeePolicy.EmployeeMaidenName;
                        payEmployees.Disabled = employeePolicy.Disabled;
                        payEmployees.Pfano = employeePolicy.Pfano;
                        payEmployees.GenderId = employeePolicy.GenderId;
                        payEmployees.StatusId = employeePolicy.StatusId;
                        payEmployees.StateId = employeePolicy.StateId;
                        payEmployees.NationalityId = employeePolicy.NationalityId;
                        payEmployees.LanguageId = employeePolicy.LanguageId;
                        payEmployees.GroupId = employeePolicy.GroupId;
                        payEmployees.AccountNo = employeePolicy.AccountNo;
                        payEmployees.GradeId = employeePolicy.GradeId;
                        payEmployees.Taxable = employeePolicy.Taxable;
                        payEmployees.JobClassId = employeePolicy.JobClassId;
                        payEmployees.Location = employeePolicy.Location;
                        payEmployees.CostCenter = employeePolicy.CostCenter;
                        payEmployees.BaseValue = employeePolicy.BaseValue;
                        payEmployees.MonthlyTax = employeePolicy.MonthlyTax;
                        payEmployees.YearToDateTax = employeePolicy.YearToDateTax;
                        payEmployees.Department = employeePolicy.Department;
                        payEmployees.PromotionDate = employeePolicy.PromotionDate;
                        payEmployees.WorkedDays = employeePolicy.WorkedDays;
                        payEmployees.LastPayDate = employeePolicy.LastPayDate;
                        payEmployees.PensionDate = employeePolicy.PensionDate;
                        payEmployees.StatePaye = employeePolicy.StatePaye;
                        payEmployees.Gross = employeePolicy.Gross;
                        payEmployees.ConfirmationDate = employeePolicy.ConfirmationDate;
                        payEmployees.GroupDescription = employeePolicy.GroupDescription;
                        payEmployees.Picture = employeePolicy.Picture;
                        payEmployees.Cleared = employeePolicy.Cleared;
                        payEmployees.EmployeeActivityTypeId = employeePolicy.EmployeeActivityTypeId;
                        payEmployees.ProvideForTax = employeePolicy.ProvideForTax;
                        payEmployees.PayrollEmployeeUserGroupId = employeePolicy.PayrollEmployeeUserGroupId;
                        payEmployees.EmployeePostalAddress = employeePolicy.EmployeePostalAddress;
                        payEmployees.BranchCode = employeePolicy.BranchCode;
                        payEmployees.Lga = employeePolicy.Lga;
                        payEmployees.GeoArea = employeePolicy.GeoArea;
                        payEmployees.BnkSortCode = employeePolicy.BnkSortCode;
                        payEmployees.TaxNumber = employeePolicy.TaxNumber;
                        payEmployees.ExecutivePermision = employeePolicy.ExecutivePermision;
                        payEmployees.SplitPay = employeePolicy.SplitPay;
                        payEmployees.SplitPayPercent = employeePolicy.SplitPayPercent;
                        payEmployees.EmployeeSecondLevelApprovedBy = employeePolicy.EmployeeSecondLevelApprovedBy;
                        payEmployees.EmployeeSignature = employeePolicy.EmployeeSignature;
                        payEmployees.BaseValueOld = employeePolicy.BaseValueOld;
                        payEmployees.RoleId = employeePolicy.RoleId;
                        payEmployees.IsSupervisor = employeePolicy.IsSupervisor;
                        payEmployees.PinPassword = employeePolicy.PinPassword;
                        payEmployees.EmpManagerName = employeePolicy.EmpManagerName;
                        payEmployees.EmpSecondManagerName = employeePolicy.EmpSecondManagerName;

                        _DBContext.Entry(payEmployees).State = EntityState.Added;

                        if (employeePolicy.payrollEmployeesDetail != null && employeePolicy.payrollEmployeesDetail.Count > 0)
                        {
                            foreach (PayrollEmployeesDetail empDetail in employeePolicy.payrollEmployeesDetail)
                            {
                                var payrollEmployeesDetailobj = await _DBContext.PayrollEmployeesDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == empDetail.EmployeeId &&
                                                        x.PayYn == empDetail.PayYn).FirstOrDefaultAsync();

                                if (payrollEmployeesDetailobj != null)
                                {
                                    payrollEmployeesDetailobj.PayType = empDetail.PayType;
                                    payrollEmployeesDetailobj.PayFrequency = empDetail.PayFrequency;
                                    payrollEmployeesDetailobj.Salary = empDetail.Salary;
                                    payrollEmployeesDetailobj.HourlyRate = empDetail.HourlyRate;
                                    payrollEmployeesDetailobj.CommissionCalc = empDetail.CommissionCalc;
                                    payrollEmployeesDetailobj.ComissionPerc = empDetail.ComissionPerc;
                                    payrollEmployeesDetailobj.OvertimeRate = empDetail.OvertimeRate;
                                    payrollEmployeesDetailobj.Ficayn = empDetail.Ficayn;
                                    payrollEmployeesDetailobj.Fityn = empDetail.Fityn;
                                    payrollEmployeesDetailobj.FicamedYn = empDetail.FicamedYn;
                                    payrollEmployeesDetailobj.Futayn = empDetail.Futayn;
                                    payrollEmployeesDetailobj.Sutayn = empDetail.Sutayn;
                                    payrollEmployeesDetailobj.Sdiyn = empDetail.Sdiyn;
                                    payrollEmployeesDetailobj.LocalYn = empDetail.LocalYn;
                                    payrollEmployeesDetailobj.Luiyn = empDetail.Luiyn;
                                    payrollEmployeesDetailobj.FederalAllowance = empDetail.FederalAllowance;
                                    payrollEmployeesDetailobj.StateAllowance = empDetail.StateAllowance;
                                    payrollEmployeesDetailobj.CountyAllowance = empDetail.CountyAllowance;
                                    payrollEmployeesDetailobj.CityAllowance = empDetail.CityAllowance;
                                    payrollEmployeesDetailobj.FederalWithholdingAmount = empDetail.FederalWithholdingAmount;
                                    payrollEmployeesDetailobj.StateWithholdingAmount = empDetail.StateWithholdingAmount;
                                    payrollEmployeesDetailobj.CountyWithhoddingAmount = empDetail.CountyWithhoddingAmount;
                                    payrollEmployeesDetailobj.CityWithhoddingAmount = empDetail.CityWithhoddingAmount;
                                    payrollEmployeesDetailobj.OtherAmount = empDetail.OtherAmount;
                                    payrollEmployeesDetailobj.FederalFilingStatus = empDetail.FederalFilingStatus;
                                    payrollEmployeesDetailobj.StateFilingStatus = empDetail.StateFilingStatus;
                                    payrollEmployeesDetailobj.CountyFilingStatus = empDetail.CountyFilingStatus;
                                    payrollEmployeesDetailobj.CityFilingStatus = empDetail.CityFilingStatus;
                                    payrollEmployeesDetailobj.OtherStatus = empDetail.OtherStatus;
                                    payrollEmployeesDetailobj.Dependents = empDetail.Dependents;
                                    payrollEmployeesDetailobj.MaleFemale = empDetail.MaleFemale;
                                    payrollEmployeesDetailobj.Amount = empDetail.Amount;
                                    payrollEmployeesDetailobj.NetAmount = empDetail.NetAmount;
                                    payrollEmployeesDetailobj.Additions = empDetail.Additions;
                                    payrollEmployeesDetailobj.Deductions = empDetail.Deductions;
                                    payrollEmployeesDetailobj.PreTaxedAmount = empDetail.PreTaxedAmount;
                                    payrollEmployeesDetailobj.BlankCheckHourlyRate = empDetail.BlankCheckHourlyRate;
                                    payrollEmployeesDetailobj.BlankCheckOvertimeRate = empDetail.BlankCheckOvertimeRate;
                                    payrollEmployeesDetailobj.YearToDateGross = empDetail.YearToDateGross;
                                    payrollEmployeesDetailobj.YearToDateAgi = empDetail.YearToDateAgi;
                                    payrollEmployeesDetailobj.YearToDateFica = empDetail.YearToDateFica;
                                    payrollEmployeesDetailobj.YearToDateFicamed = empDetail.YearToDateFicamed;
                                    payrollEmployeesDetailobj.YearToDateFit = empDetail.YearToDateFit;
                                    payrollEmployeesDetailobj.YearToDateFuta = empDetail.YearToDateFuta;
                                    payrollEmployeesDetailobj.YearToDateSuta = empDetail.YearToDateSuta;
                                    payrollEmployeesDetailobj.YearToDateSit = empDetail.YearToDateSit;
                                    payrollEmployeesDetailobj.YearToDateSdi = empDetail.YearToDateSdi;
                                    payrollEmployeesDetailobj.YearToDateLocal = empDetail.YearToDateLocal;
                                    payrollEmployeesDetailobj.YearToDateLui = empDetail.YearToDateLui;
                                    payrollEmployeesDetailobj.LastGross = empDetail.LastGross;
                                    payrollEmployeesDetailobj.LastAgi = empDetail.LastAgi;
                                    payrollEmployeesDetailobj.LastFica = empDetail.LastFica;
                                    payrollEmployeesDetailobj.LastFicamed = empDetail.LastFicamed;
                                    payrollEmployeesDetailobj.LastFit = empDetail.LastFit;
                                    payrollEmployeesDetailobj.LastFuta = empDetail.LastFuta;
                                    payrollEmployeesDetailobj.LastSuta = empDetail.LastSuta;
                                    payrollEmployeesDetailobj.LastSit = empDetail.LastSit;
                                    payrollEmployeesDetailobj.LastSdi = empDetail.LastSdi;
                                    payrollEmployeesDetailobj.LastLocal = empDetail.LastLocal;
                                    payrollEmployeesDetailobj.LastLui = empDetail.LastLui;
                                    payrollEmployeesDetailobj.LastCommissionAmount = empDetail.LastCommissionAmount;
                                    payrollEmployeesDetailobj.YearToDateRegularHours = empDetail.YearToDateRegularHours;
                                    payrollEmployeesDetailobj.YearToDateOvertimeHours = empDetail.YearToDateOvertimeHours;
                                    payrollEmployeesDetailobj.LastHours = empDetail.LastHours;
                                    payrollEmployeesDetailobj.LastPayDate = empDetail.LastPayDate;
                                    payrollEmployeesDetailobj.LockedBy = empDetail.LockedBy;
                                    payrollEmployeesDetailobj.LockTs = empDetail.LockTs;
                                    payrollEmployeesDetailobj.BranchCode = empDetail.BranchCode;

                                    _DBContext.Entry(payrollEmployeesDetailobj).State = EntityState.Modified;
                                }
                                else
                                {
                                    empDetail.CompanyId = token.CompanyId;
                                    empDetail.DivisionId = token.DivisionId;
                                    empDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(empDetail).State = EntityState.Added;
                                }
                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Employee Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayEmployees(string EmployeeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var payrollEmployee = await _DBContext.PayrollEmployees.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == EmployeeId).FirstOrDefaultAsync();
                if (payrollEmployee != null)
                {
                    _DBContext.Entry(payrollEmployee).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }



        public async Task<Approvals> GetApprovalsByEmployee(string Id, ApiToken token)
        {
            Approvals approvals = new Approvals();

            Appraisals approvalAppraisal = new Appraisals();
            Loans approvalLoan = new Loans();
            Leaves approvalLeave = new Leaves();
            Data.POCO.Requisitions approvalRequisition = new Data.POCO.Requisitions();

            List<PayrollHrpayrollAppraisalHeader> appraisal = new List<PayrollHrpayrollAppraisalHeader>();
            List<PayrollHrpayrollLoanDetail> loan = new List<PayrollHrpayrollLoanDetail>();
            List<PayrollHrpayrollLeaveDetail> leave = new List<PayrollHrpayrollLeaveDetail>();
            List<RequisitionsHeader> requisition = new List<RequisitionsHeader>();

            int appraisalCount = 0;
            int leaveCount = 0;
            int loanCount = 0;
            int requisitionCount = 0;

            string imageBaseUrl = "https://client.powersoft-solutions.org/PEApi/resource/img";

            try
            {
                approvals.CompanyId = token.CompanyId;
                approvals.DivisionId = token.DivisionId;
                approvals.DepartmentId = token.DepartmentId;
                approvals.EmployeeId = Id;

                var payrollEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == Id &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();
                if (payrollEmployee != null)
                {
                    PayrollEmployees systemUser = new PayrollEmployees();

                    if (payrollEmployee.EmployeeEmailAddress != null)
                    {

                        systemUser = await _DBContext.PayrollEmployees
                                                   .Where(x => x.CompanyId == token.CompanyId &&
                                                               x.DivisionId == token.DivisionId &&
                                                               x.DepartmentId == token.DepartmentId &&
                                                               x.EmployeeEmailAddress == payrollEmployee.EmployeeEmailAddress &&
                                                               x.EmployeeTypeId == "User" &&
                                                               x.ActiveYn == true).FirstOrDefaultAsync();
                    }

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
                            userPermissions.SscanApproveAppraisal = userPermissions.SscanApproveAppraisal == null || userPermissions.SscanApproveAppraisal == false ? false : true;
                            userPermissions.SscanApproveLeave = userPermissions.SscanApproveLeave == null || userPermissions.SscanApproveLeave == false ? false : true;
                            userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;
                            userPermissions.SsisExecutive = userPermissions.SsisExecutive == null || userPermissions.SsisExecutive == false ? false : true;


                            if (userPermissions.SscanApproveAppraisal == true)
                            {
                                //get appraisals to be approved, confirmed, posted
                                var appraisals = await _DBContext.PayrollHrpayrollAppraisalHeader
                                                            .Where(x => x.CompanyId == token.CompanyId &&
                                                                        x.DivisionId == token.DivisionId &&
                                                                        x.DepartmentId == token.DepartmentId &&
                                                                        x.Cleared == true &&
                                                                        (
                                                                        (x.Approved == false &&
                                                                        x.ApprovedBy == Id)
                                                                        ||
                                                                        (
                                                                        x.Approved == true &&
                                                                        x.Confirmed == false &&
                                                                        x.ConfirmedBy == Id)

                                                                        ||
                                                                        (
                                                                        x.Approved == true &&
                                                                        x.Confirmed == true &&
                                                                        x.Posted == false &&
                                                                        x.PostedBy == Id)
                                                                        )).ToListAsync();
                                if (appraisals != null)
                                {
                                    appraisal = appraisals;
                                    appraisalCount = appraisals.Count;
                                }
                            }


                            if (userPermissions.SscanApproveLeave == true)
                            {
                                if (userPermissions.SsisExecutive == true)
                                {
                                    var leaves = await _DBContext.PayrollHrpayrollLeaveDetail
                                                        .Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.Cleared == true &&
                                                                    (
                                                                    (x.Approved == false &&
                                                                    x.ApprovedBy == Id)
                                                                    ||
                                                                    (
                                                                    x.Approved == true &&
                                                                    x.Posted == false &&
                                                                    x.PostedBy == Id)

                                                                    ||
                                                                    (
                                                                    x.Approved == true &&
                                                                    x.Audited == false &&
                                                                    x.AuditedBy == Id)

                                                                    ||
                                                                    (
                                                                    x.ExecutivePermision == false &&
                                                                    x.ExecutivePermisionBy == Id)
                                                                    )).ToListAsync();

                                    if (leaves != null)
                                    {
                                        leave = leaves;
                                        leaveCount = leaves.Count;
                                    }
                                }
                                else
                                {
                                    var leaves = await _DBContext.PayrollHrpayrollLeaveDetail
                                                        .Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.Cleared == true &&
                                                                    (
                                                                    (x.Approved == false &&
                                                                    x.ApprovedBy == Id)
                                                                    ||
                                                                    (
                                                                    x.Approved == true &&
                                                                    x.Posted == false &&
                                                                    x.PostedBy == Id)

                                                                    ||
                                                                    (
                                                                    x.Approved == true &&
                                                                    x.Audited == false &&
                                                                    x.AuditedBy == Id)
                                                                    )).ToListAsync();

                                    if (leaves != null)
                                    {
                                        leave = leaves;
                                        leaveCount = leaves.Count;
                                    }
                                }
                            }

                            if (userPermissions.SscanApproveLoan == true)
                            {

                                var loans = await _DBContext.PayrollHrpayrollLoanDetail
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.Cleared == true &&
                                                                   x.ActiveYn == true &&
                                                                   (
                                                                   (x.Approved == false &&
                                                                   x.ApprovedBy == Id)
                                                                   ||
                                                                   (
                                                                   x.Approved == true &&
                                                                   x.Posted == false &&
                                                                   x.PostedBy == Id)
                                                                   )).ToListAsync();

                                if (loans != null)
                                {
                                    loan = loans;
                                    loanCount = loans.Count;
                                }
                            }

                            if (userPermissions.SscanApproveLoan == true)
                            {

                                var requisitions = await _DBContext.RequisitionsHeader
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.Cleared == true &&
                                                                   (
                                                                   (x.Approved == false &&
                                                                   x.ApprovedBy == Id)
                                                                   ||
                                                                   (
                                                                   x.Approved == true &&
                                                                   x.Posted == false &&
                                                                   x.PostedBy == Id)
                                                                   )).ToListAsync();

                                if (requisitions != null)
                                {
                                    requisition = requisitions;
                                    requisitionCount = requisitions.Count;
                                }
                            }
                        }
                    }

                    //load result
                    approvalAppraisal.totalCount = appraisalCount;
                    approvalLeave.totalCount = leaveCount;
                    approvalLoan.totalCount = loanCount;
                    approvalRequisition.totalCount = requisitionCount;

                    approvalAppraisal.Description = "Appraisal";
                    approvalLeave.Description = "Leave";
                    approvalLoan.Description = "Loan";
                    approvalRequisition.Description = "Requisition";

                    approvalAppraisal.Icon = imageBaseUrl + "/appraisal.png";
                    approvalLeave.Icon = imageBaseUrl + "/leave.png";
                    approvalLoan.Icon = imageBaseUrl + "/loan.png";
                    approvalRequisition.Icon = imageBaseUrl + "/requisition.png";

                    approvalAppraisal.data = appraisal;
                    approvalLeave.data = leave;
                    approvalLoan.data = loan;
                    approvalRequisition.data = requisition;
                }

                approvals.Appraisals = approvalAppraisal != null ? approvalAppraisal : null;
                approvals.Leaves = approvalLeave != null ? approvalLeave : null;
                approvals.Loans = approvalLoan != null ? approvalLoan : null;
                approvals.Requisitions = approvalRequisition != null ? approvalRequisition : null;

            }
            catch (Exception ex)
            {
                
            }

            return approvals;
        }

        public async Task<IEnumerable<PayrollEmployees>> GetByDept(string Id, string Mode, ApiToken token)
        {
            try
            {
                if (Mode == "Active")
                {
                    return await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.Department == Id &&
                                                        x.ActiveYn == true &&
                                                        x.EmployeeTypeId != "User").ToListAsync();
                }
                else if (Mode == "InActive")
                {
                    return await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.Department == Id &&
                                                        x.ActiveYn == false &&
                                                        x.EmployeeTypeId != "User").ToListAsync();
                }
                else
                {
                    return await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.Department == Id &&
                                                        x.EmployeeTypeId != "User").ToListAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollEmployees>> GetById(string Id, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.EmployeeId == Id &&
                                                         x.EmployeeTypeId != "User").ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollJobClassHeader>> GetJobClass(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollJobClassHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollQualificationType>> GetQualificationType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollQualificationType.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Login(string Username, string Password, ApiToken token)
        {
            bool status = false;

            try
            {
                string pwd = EnterpriseExtras.doConvertPwd(Password);

                var employee = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.EmployeeId == Username &&
                                                         x.EmployeePassword == pwd &&
                                                         x.ActiveYn == true &&
                                                         x.EmployeeTypeId != "User").FirstOrDefaultAsync();
                if (employee != null)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public Task<StatusMessage> Process(string Id, string type, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<StatusMessage> ResetPwd(PasswordModel resetPasswordModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            string newPwd = "";

            string generatedPwd = "";

            try
            {
                //check old password
                var employee = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.EmployeeId == resetPasswordModel.Username &&
                                                         x.ActiveYn == true &&
                                                         x.EmployeeTypeId != "User").FirstOrDefaultAsync();
                if (employee == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Incorrect Username and/or Password";
                }
                else if(employee.EmployeeEmailAddress == null || employee.EmployeeEmailAddress == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "No Email Address. Contact System Administrator";
                }
                else
                {
                    //generate random Alpha-Numeric password of 8 characters length
                    generatedPwd = EnterpriseExtras.CreateRandomPassword(8);

                    if (generatedPwd != null && generatedPwd != "")
                    {
                        //encrypt new password
                        newPwd = EnterpriseExtras.doConvertPwd(generatedPwd);

                        employee.EmployeePasswordOld = employee.EmployeePassword;
                        employee.EmployeePassword = newPwd;
                        employee.EmployeePasswordDate = DateTime.Now;

                        _DBContext.Entry(employee).State = EntityState.Modified;
                        var rowCount = _DBContext.SaveChanges();

                        if(rowCount > 0)
                        {
                            var Body = "<html><body style='font-family:Calibri;'><p>" +
                                        "Dear " + employee.EmployeeName + ' ' + employee.EmployeeOthername +
                                        ",<br></p><p>Your password reset is successfull. You can now login with the new password below.</p><br>" +
                                        "<p><strong> New Password: </strong>" + generatedPwd +
                                        "</p>" +
                                        "<br><br><p> Sent from: <strong>Power Employee Manager</strong></p><p>Date: <strong> " + DateTime.Now.Day.ToString() + '/' + DateTime.Now.Month.ToString() + '/' + DateTime.Now.Year.ToString() +
                                        "</strong></p></body></html>" +
                                        "";

                            var counter = await _DBContext.MailSendCounter.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId
                                                         ).FirstOrDefaultAsync();

                            if (counter != null)
                            {
                                _mailSend.Counter = counter.Counter;
                                _mailSend.CompanyId = token.CompanyId;
                                _mailSend.DivisionId = token.DivisionId;
                                _mailSend.DepartmentId = token.DepartmentId;
                                _mailSend.Recipient = employee.EmployeeEmailAddress;
                                _mailSend.Body = Body;
                                _mailSend.Subject = "Password Reset";
                                _mailSend.SenderId = "Power Employee Manager";

                                await _DBContext.AddAsync(_mailSend);
                                var mailInsert = _DBContext.SaveChanges();

                                if (mailInsert > 0)
                                {
                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "A new password has been sent to your email.";

                                    //update counter
                                    counter.Counter = counter.Counter + 1;

                                    _DBContext.Entry(counter).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Failed To Send Mail. Try Again";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Mail Not Sent";
                            }
                            
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Failed To Send Mail. Try Again";
                        }                        
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Password Generation Failed";
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

        public Task<StatusMessage> Update(PayrollEmployees employee, ApiToken token)
        {
            throw new NotImplementedException();
        }
    }
}
