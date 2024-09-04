using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
//using PowerAPI.Helper;
using Microsoft.AspNetCore.Cors;
//using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
//using PowerAPI.Helper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Attendance API endpoints
    /// </summary>
    //[EnableCors("AllowOrigin")]
    [Authorize]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        IAttendance _attendance;

        /// <summary>
        /// Attendant controller
        /// </summary>
        public AttendanceController(IAttendance attendance)
        {
            _attendance = attendance;
        }

        /// <summary>
        /// gets a list of Attendance records
        /// </summary>
        [Route("api/GetAttendance/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetAttendance(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetAttendance(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
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
        /// gets a list of Attendance records by Employee Id
        /// </summary>
        [Route("api/GetAttendanceByEmployee/{token}")]
        ////[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetAttendanceByEmployee(string EmployeeId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetByEmployee(EmployeeId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;

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
        /// gets Attendance record by the Id
        /// </summary>
        [Route("api/GetAttendanceById/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetAttendanceById(string EmployeeId, DateTime AttendanceDate,
                                                    string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetById(EmployeeId, AttendanceDate, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
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
        /// creates a new Attendance record or update current date Attendance record
        /// </summary>
        [Route("api/AddAttendance/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] PayrollHrpayrollAttDetail attendance, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            //AuthorizeAttribute authorizeAttribute = new AuthorizeAttribute();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                //var user = authorizeAttribute.user;

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _attendance.Add(attendance, Mode, tokenObj);

                        return Ok(result);
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


        ///// <summary>
        ///// Gets lateness report details of an employee
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[Route("api/GetLatenessReportDetails/{token}")]
        ////[EnableCors("AllowOrigin")]
        //[HttpGet]
        //public async Task<IActionResult> GetLatenessReportDetails(string token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();

        //    try
        //    {
        //        var tokenObj = await _attendance.GetAccess(token);

        //        if (tokenObj != null)
        //        {
        //            if (tokenObj.TotalDays >= 0)
        //            {
        //                var attendance = await _attendance.GetLatenessReportDetails(tokenObj);

        //                statusMessage.Status = "Success";
        //                statusMessage.Message = "Success";
        //                statusMessage.data = attendance;
        //                return Ok(statusMessage);
        //            }
        //            else
        //            {
        //                statusMessage.Status = "Failed";
        //                statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

        //                return Ok(statusMessage);
        //            }
        //        }
        //        else
        //        {
        //            statusMessage.Status = "Failed";
        //            statusMessage.Message = "Invalid Token";

        //            return Ok(statusMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        statusMessage.Status = "Failed";
        //        statusMessage.Message = "Unknown Error. Try Again";

        //        return Ok(statusMessage);
        //    }
        //}

        /// <summary>
        /// Gets the deduction report of employees absenteeism and lateness in minutes
        /// </summary>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetLatenessReportDetails/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetLatenessReportDetails(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _attendance.GetAccess(token);
                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetLatenessReportDetails(PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License expired. Contact System Administrator";

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
        /// Gets lateness summary of an employee
        /// </summary>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetLatenessReportSummary/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetLatenessReportSummary(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _attendance.GetAccess(token);
                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetLatenessReportSummary(PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
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
        /// Gets Absentees report detail
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetAbsenteeismReportDetail/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetAbsenteeismReportDetail(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetAbsenteeismReportDetail(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
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
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }
        /// <summary>
        /// Gets the Absenteeism report summary
        /// </summary>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetAbsenteeismReportSummary/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetAbsenteeismReportSummary(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _attendance.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetAbsenteeismReportSummary(PeriodFrom, PeriodTo,tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
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
        /// Gets the deduction report of employees absenteeism and lateness in minutes
        /// </summary>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetDeductionReport/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetDeductionReport(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _attendance.GetAccess(token);
                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetDeductionReport(PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License expired. Contact System Administrator";

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
        /// Gets the daily attendance summary report of employees
        /// </summary>
        /// <param name="token"></param>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <returns></returns>
        [Route("api/GetDailyAttendanceSummary/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetDailyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (PeriodFrom == null)
                {
                    PeriodFrom = new DateTime(1900, 01, 01);
                }

                if (PeriodTo == null)
                {
                    PeriodTo = DateTime.Now;
                }
                var tokenObj = await _attendance.GetAccess(token);
                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetDailyAttendanceSummary(PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License expired. Contact System Administrator";

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
        /// Gets the daily attendance summary report of employees
        /// </summary>
        /// <param name="token"></param>
        /// <param name="PeriodFrom"></param>
        /// <param name="PeriodTo"></param>
        /// <returns></returns>
        [Route("api/GetMonthlyAttendanceSummary/{token}")]
        //[EnableCors("AllowOrigin")]
        [HttpGet]
        public async Task<IActionResult> GetMonthlyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (PeriodFrom == null)
                {
                    PeriodFrom = new DateTime(1900, 01, 01);
                }

                if (PeriodTo == null)
                {
                    PeriodTo = DateTime.Now;
                }
                var tokenObj = await _attendance.GetAccess(token);
                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var attendance = await _attendance.GetMonthlyAttendanceSummary(PeriodFrom, PeriodTo, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = attendance;
                        return Ok(statusMessage);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License expired. Contact System Administrator";

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
    }
}





