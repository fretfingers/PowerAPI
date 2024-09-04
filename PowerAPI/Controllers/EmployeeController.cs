using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Helper;
using System.Text.RegularExpressions;
using PowerAPI.Data.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Employee API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        IEmployee _employee;


        /// <summary>
        /// Employee controller
        /// </summary>
        public EmployeeController(IEmployee employee)
        {
            _employee = employee;
        }

        /// <summary>
        /// gets a list of Employee records
        /// </summary>
        [Route("api/GetEmployees/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var employees = await _employee.GetAll(Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = employees;
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
        /// gets Employee record by Employee Id
        /// </summary>
        [Route("api/GetEmployeeById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeById(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var employee = await _employee.GetById(EmployeeId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = employee;
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
        /// updates a list of Employee records
        /// </summary>
        /// <param name="employeePolicy"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddEmployees/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddEmployees([FromBody]PayEmployees employeePolicy, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _employee.AddEmployees(employeePolicy, tokenObj);

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

        /// <summary>
        /// deletes Payroll Employee
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/DeleteEmployee/{token}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        statusMessage = await _employee.DeletePayEmployees(EmployeeId, tokenObj);

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
        /// gets a list of Employees records by Department Id
        /// </summary>
        [Route("api/GetEmployeeByDept/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByDept(string DepartmentId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var employees = await _employee.GetByDept(DepartmentId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = employees;
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
        /// gets lists of records awaiting approval by Login Employee
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetApprovalByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetApprovalByEmployee(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var approvals = await _employee.GetApprovalsByEmployee(EmployeeId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = approvals;
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
        /// gets a list of Employee Job Class records
        /// </summary>
        [Route("api/GetJobClass/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetJobClass(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var jobClass = await _employee.GetJobClass(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = jobClass;
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
        /// gets a list of Qualification Type
        /// </summary>
        [Route("api/GetQualificationType/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetQualificationType(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var qualificationType = await _employee.GetQualificationType(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = qualificationType;
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
        /// Change Employee Password
        /// </summary>
        /// <param name="changePasswordModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/ChangePasswordEmp/{token}")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordEmp(PasswordModel changePasswordModel, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (changePasswordModel == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Employee Change Data";
                    return Ok(statusMessage);
                }
                else if (changePasswordModel.Username == null || changePasswordModel.Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Username Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else if(changePasswordModel.OldPassword == null || changePasswordModel.OldPassword == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Old Password Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else if(changePasswordModel.NewPassword == null || changePasswordModel.NewPassword == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "New Password Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else
                {
                    Regex regexObj = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");
                    bool isMatch = changePasswordModel.NewPassword == null || changePasswordModel.NewPassword == "" ? false : regexObj.IsMatch(changePasswordModel.NewPassword);

                    if(changePasswordModel.NewPassword.Length < 8)
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Password Must Be Minimum Of Eight (8) Characters";
                        return Ok(statusMessage);
                    }
                    else //if(isMatch) this will be revisited
                    {
                        var tokenObj = await _employee.GetAccess(token);

                        if (tokenObj != null)
                        {
                            if (tokenObj.TotalDays >= 0)
                            {
                                statusMessage = await _employee.ChangePwd(changePasswordModel, tokenObj);
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
                    //else // this will be un-commented when the checks for alpha-numeric is closed/dealt
                    //{
                    //    statusMessage.Status = "Failed";
                    //    statusMessage.Message = "Password Must Be AlphaNumeric. Must Contain Numbers, Alphabets And Characters.";
                    //    return Ok(statusMessage);
                    //}
                    
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
        /// Reset Employee Password
        /// </summary>
        /// <param name="resetPasswordModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/ResetPasswordEmp/{token}")]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordEmp(PasswordModel resetPasswordModel, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (resetPasswordModel == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Employee Reset Data";
                    return Ok(statusMessage);
                }
                else if (resetPasswordModel.Username == null || resetPasswordModel.Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Username Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _employee.GetAccess(token);

                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            statusMessage = await _employee.ResetPwd(resetPasswordModel, tokenObj);
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
