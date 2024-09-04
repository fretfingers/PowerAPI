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
using System.IO;
using PowerAPI.Service.Reports;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using System.Configuration;
using DevExpress.XtraReports;

namespace PowerAPI.Service.Clients
{
    public class Reports : IReports
    {
        EnterpriseContext _DBContext;
        SqlDataSource sqlDataSource;


        public Reports(EnterpriseContext DBContext)
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

        public async Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPayAnalysis(DateTime Period, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollYearPayFile.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TranDate == Period.Date).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Payslip> GetPayslip(string Id, DateTime Period, ApiToken token)
        {
            Payslip payslip = new Payslip();
            List<Earnings> paySlipEarnings = new List<Earnings>();
            List<Deductions> paySlipDeductions = new List<Deductions>();

            try
            {
                var PayAnalaysis = await _DBContext.PayrollHrpayrollYearPayFile.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == Id &&
                                                        x.TranDate == Period.Date &&
                                                        x.OnPayroll == true &&
                                                        x.ActiveYn == true).ToListAsync();

                var payments = PayAnalaysis.Where(x => x.AttrId == "P").ToList();
                var deductions = PayAnalaysis.Where(x => x.AttrId == "D").ToList();

                double grossPay = (double)computeValues("+", payments);
                double totalDeductions = (double)computeValues("+", deductions);
                double NETEarnings = grossPay - totalDeductions;

                payslip.CompanyId = token.CompanyId;
                payslip.DivisionId = token.DivisionId;
                payslip.DepartmentId = token.DepartmentId;
                payslip.EmployeeId = Id;
                payslip.Period = Period;
                payslip.CurrencyID = PayAnalaysis.FirstOrDefault().CurrencyId.ToString();
                payslip.CurrencyExchangeRate = PayAnalaysis.FirstOrDefault().CurrencyExchangeRate != null ? (double)PayAnalaysis.FirstOrDefault().CurrencyExchangeRate : 1.0;

                foreach (var earnings in payments)
                {
                    Earnings earningsObj = new Earnings();

                    earningsObj.PayTypeId = earnings.PayTypeId;
                    earningsObj.PayTypeDescription = earnings.PayTypeDescription;
                    earningsObj.AttrId = earnings.AttrId;
                    earningsObj.Amount = earnings.Amount;
                    earningsObj.PayTypeBalance = earnings.PayTypeBalance;
                    earningsObj.Taxable = earnings.Taxable == null ? false : true;
                    earningsObj.ActiveYn = earnings.ActiveYn == null ? false : true;
                    earningsObj.OnPayroll = earnings.OnPayroll == null ? false : true;
                    earningsObj.ExactAmount = earnings.ExactAmount == null ? 0.0 : earnings.ExactAmount;
                    earningsObj.Rate = earnings.Rate == null ? 0.0 : earnings.Rate;
                    earningsObj.Units = earnings.Units == null ? 0.0 : earnings.Units;
                    earningsObj.Glaccount = earnings.Glaccount;
                    earningsObj.GlaccountEmployer = earnings.GlaccountEmployer;
                    earningsObj.GlaccountEmployerExp = earnings.GlaccountEmployerExp;
                    earningsObj.BranchCode = earnings.BranchCode;
                    earningsObj.CurrencyId = earnings.CurrencyId;
                    earningsObj.CurrencyExchangeRate = earnings.CurrencyExchangeRate == null ? 1.0 : earnings.CurrencyExchangeRate;

                    paySlipEarnings.Add(earningsObj);
                }


                foreach (var deduction in deductions)
                {
                    Deductions deductionsObj = new Deductions();

                    deductionsObj.PayTypeId = deduction.PayTypeId;
                    deductionsObj.PayTypeDescription = deduction.PayTypeDescription;
                    deductionsObj.AttrId = deduction.AttrId;
                    deductionsObj.Amount = deduction.Amount;
                    deductionsObj.PayTypeBalance = deduction.PayTypeBalance;
                    deductionsObj.Taxable = deduction.Taxable == null ? false : true;
                    deductionsObj.ActiveYn = deduction.ActiveYn == null ? false : true;
                    deductionsObj.OnPayroll = deduction.OnPayroll == null ? false : true;
                    deductionsObj.ExactAmount = deduction.ExactAmount == null ? 0.0 : deduction.ExactAmount;
                    deductionsObj.Rate = deduction.Rate == null ? 0.0 : deduction.Rate;
                    deductionsObj.Units = deduction.Units == null ? 0.0 : deduction.Units;
                    deductionsObj.Glaccount = deduction.Glaccount;
                    deductionsObj.GlaccountEmployer = deduction.GlaccountEmployer;
                    deductionsObj.GlaccountEmployerExp = deduction.GlaccountEmployerExp;
                    deductionsObj.BranchCode = deduction.BranchCode;
                    deductionsObj.CurrencyId = deduction.CurrencyId;
                    deductionsObj.CurrencyExchangeRate = deduction.CurrencyExchangeRate == null ? 1.0 : deduction.CurrencyExchangeRate;

                    paySlipDeductions.Add(deductionsObj);
                }


                payslip.GrossPay = (double)grossPay;
                payslip.TotalDeductions = (double)totalDeductions;
                payslip.NETEarnings = (double)NETEarnings;
                payslip.Earnings = paySlipEarnings == null ? null : paySlipEarnings;
                payslip.Deductions = paySlipDeductions == null ? null : paySlipDeductions;

            }
            catch (Exception ex)
            {
            }

            return payslip;
        }

        public async Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPayslipRange(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollYearPayFile.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == Id &&
                                                        x.TranDate >= PeriodFrom.Date &&
                                                        x.TranDate <= PeriodTo.Date).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPensionRangeRpt(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPensionRpt(string Id, DateTime Period, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetTaxRangeRpt(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetTaxRpt(string Id, DateTime Period, ApiToken token)
        {
            throw new NotImplementedException();
        }

        private float computeValues(string Operator, dynamic data)
        {
            float amount = 0;

            switch (Operator)
            {
                case "+":
                    foreach (var record in data)
                    {
                        amount += record.Amount;
                    }
                    break;
                case "-":
                    break;

            }

            return amount;

        }

        public async Task<byte[]> GetOrderReportById(string Id, ApiToken token)
        {
            try
            {
                //await _DBContext.OrderHeader.Where(x => x.CompanyId == token.CompanyId &&
                //                                        x.DivisionId == token.DivisionId &&
                //                                        x.DepartmentId == token.DepartmentId &&
                //                                        x.OrderNumber == Id).ToListAsync();
                byte[] reportOrder = await PrintOrderReport(token.CompanyId, token.DivisionId, token.DepartmentId, Id);

                return reportOrder;
            }
            //catch (IndexOutOfRangeException e)
            //{

            //}
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<byte[]> GetRmaReportById(string Id, ApiToken token)
        {
            // RmaOrder rmaOrder = new RmaOrder;
            try
            {
                //await _DBContext.InvoiceHeader.Where(x => x.CompanyId == token.CompanyId &&
                //                                        x.DivisionId == token.DivisionId &&
                //                                        x.DepartmentId == token.DepartmentId &&
                //                                        x.InvoiceNumber == Id).ToListAsync();
                byte[] rmaOrder = await PrintRmaReport(token.CompanyId, token.DivisionId, token.DepartmentId, Id);

                return rmaOrder;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Unknown error from service try again ", e);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> GetSalesReceiptRollById(string Id, ApiToken token)
        {
            try
            {

                byte[] salesReceiptRoll = await PrintSalesReceiptRoll(token.CompanyId, token.DivisionId, token.DepartmentId, Id);

                return salesReceiptRoll;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<byte[]> PrintOrderReport(string companyID, string divisionID, string departmentID, string orderNumber)
        {
            try
            {
                var report = new OrderReport();


                //var dataSources = DataSourceManager.GetDataSources(report, true);
                //foreach (var dataSource in dataSources)
                //{
                //    if (dataSource is SqlDataSource sds && !String.IsNullOrEmpty(sds.ConnectionName))
                //    {
                //        sds.ConnectionParameters = null;
                //    }
                //}

                report.Parameters["CompanyID"].Value = companyID;
                report.Parameters["DivisionID"].Value = divisionID;
                report.Parameters["DepartmentID"].Value = departmentID;
                report.Parameters["OrderNumber"].Value = orderNumber;


                using (var ms = new MemoryStream())
                {
                    await report.ExportToPdfAsync(ms);
                    //return File(ms.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
                    return ms.ToArray();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<byte[]> PrintRmaReport(string companyId, string divisionId, string departmentId, string invoiceNumber)
        {
            try
            {
                var report = new RmaReport();

                report.Parameters["CompanyID"].Value = companyId;
                report.Parameters["DivisionID"].Value = divisionId;
                report.Parameters["DepartmentID"].Value = departmentId;
                report.Parameters["InvoiceNumber"].Value = invoiceNumber;

                using (var ms = new MemoryStream())
                {
                    await report.ExportToPdfAsync(ms);
                    return ms.ToArray();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<byte[]> PrintSalesReceiptRoll(string companyID, string divisionID, string departmentID, string orderNumber)
        {
            try
            {
                var report = new SalesReceiptRoll();


                report.Parameters["CompanyID"].Value = companyID;
                report.Parameters["DivisionID"].Value = divisionID;
                report.Parameters["DepartmentID"].Value = departmentID;
                report.Parameters["OrderNumber"].Value = orderNumber;


                using (var ms = new MemoryStream())
                {
                    await report.ExportToPdfAsync(ms);
                    return ms.ToArray();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ...
        //private SqlDataSource CreateConnectionFromString()
        //{
        //    string connectionString = Configuration.GetConnectionString("Enterprise");
        //    CustomStringConnectionParameters connectionParameters =
        //        new CustomStringConnectionParameters(connectionString);
        //    return sqlDataSource = new SqlDataSource(connectionParameters);
        //}


    }
}
