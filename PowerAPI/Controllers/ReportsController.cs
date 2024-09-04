using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Helper;
using PowerAPI.Service.Reports;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Report API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        IReports _reports;

        /// <summary>
        /// Report controller
        /// </summary>
        public ReportsController(IReports reports)
        {
            _reports = reports;
        }

        /// <summary>
        /// gets Payslip record for the Employee Id and Payroll Period (Single Month)
        /// </summary>
        [Route("api/GetPayslip/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetPayslip(string EmployeeId, DateTime Period,
                                                    string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var payslip = await _reports.GetPayslip(EmployeeId, Period, tokenObj);

                        if (payslip != null)
                        {

                            statusMessage.Status = "Success";
                            statusMessage.Message = "Success";
                            statusMessage.data = payslip;
                            return Ok(statusMessage);
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Failed To Load Payslip. Try again or contact your system administrator";
                            return Ok(statusMessage);
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets Payslip record for the Employee Id and Payroll Period (For More than one Month)
        /// </summary>
        [Route("api/GetPayslipRange/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetPayslipRange(string EmployeeId, DateTime PeriodFrom,
                                                    DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var payslip = await _reports.GetPayslipRange(EmployeeId, PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = payslip;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets PayAnalysis for the Payroll Period (Single Month)
        /// </summary>
        [Route("api/GetPayAnalysis/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetPayAnalysis(DateTime Period,
                                                      string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var payslip = await _reports.GetPayAnalysis(Period, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = payslip;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets a list of Order By Id
        /// </summary>
        [Route("api/GetOrderReportById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderReportById(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orderReport = await _reports.GetOrderReportById(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = File(orderReport, System.Net.Mime.MediaTypeNames.Application.Pdf);
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets a list of RMA By Id
        /// </summary>
        [Route("api/GetRmaReportById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRmaReportById(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var rmaReport = await _reports.GetRmaReportById(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = File(rmaReport, System.Net.Mime.MediaTypeNames.Application.Pdf);
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message =  ex.Message;

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets a list of Order By Id
        /// </summary>
        [Route("api/GetSalesReceiptRollById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetSalesReceiptRollById(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _reports.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orderReport = await _reports.GetSalesReceiptRollById(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = File(orderReport, System.Net.Mime.MediaTypeNames.Application.Pdf);
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        return Ok(statusMessage);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";

                    return Ok(statusMessage);
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;

                return Ok(statusMessage);
            }
        }
    }

}
