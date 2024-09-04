using Microsoft.AspNetCore.Mvc;
//using PowerAPI.Models;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Helper;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Leave API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        ILeave _leave;

        /// <summary>
        /// Leave controller
        /// </summary>
        public LeaveController(ILeave leave)
        {
            _leave = leave;
        }

        /// <summary>
        /// gets a list of LeaveTypes By Employee Id
        /// </summary>
        [Route("api/GetLeaveTypeByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLeaveTypeByEmployee(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _leave.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var leave = await _leave.GetLeaveTypeByEmployee(EmployeeId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = leave;
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
        /// gets a list of Leave records
        /// </summary>
        [Route("api/GetLeave/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLeave(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _leave.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var leaves = await _leave.GetAll(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = leaves;
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
        /// gets Leave record by Id
        /// </summary>
        [Route("api/GetLeaveById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLeaveById(string EmployeeId, string LeaveType,
                                                    DateTime StartDate, DateTime EndDate,
                                                    string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _leave.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var leave = await _leave.GetById(EmployeeId, LeaveType, StartDate, EndDate, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = leave;
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
        /// gets a list of Leave By Employee Id
        /// </summary>
        [Route("api/GetLeaveByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLeaveByEmployee(string EmployeeId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _leave.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var leaves = await _leave.GetByEmployee(EmployeeId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = leaves;

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
        /// creates a new Leave record - Mode("Submit": Submits the leave request)
        /// </summary>
        [Route("api/AddLeave/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddLeave([FromBody] PayrollHrpayrollLeaveDetail Leave, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _leave.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _leave.AddLeave(Leave, Mode, tokenObj);

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

        //// PUT api/values/5
        //[Route("api/UpdateLeave/{token}")]
        //[HttpPut]
        //public void UpdateLeave([FromBody]Leave leave, string token)
        //{
        //}

        //// DELETE api/values/5
        //[Route("api/DeleteLeave/{token}")]
        //[HttpDelete]
        //public void DeleteLeave(string Employee, string LeaveType, string token)
        //{
        //}
    }
}
