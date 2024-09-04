using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Helper;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
//using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Loan API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class LoanController : ControllerBase
    {
        ILoan _loan;

        /// <summary>
        /// Loan controller
        /// </summary>
        public LoanController(ILoan loan)
        {
            _loan = loan;
        }

        /// <summary>
        /// gets a list of LoanTypes by Employee Id
        /// </summary>
        [Route("api/GetLoanTypeByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLoanTypeByEmployee(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var loan = await _loan.GetLoanTypeByEmployee(EmployeeId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = loan;
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
        /// gets a list of Loans
        /// </summary>
        [Route("api/GetLoan/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLoan(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var loans = await _loan.GetAll(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = loans;
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
        /// gets Loan By Id
        /// </summary>
        [Route("api/GetLoanById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLoanById(string EmployeeId, string LoanType,
                                                      string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var loan = await _loan.GetById(EmployeeId, LoanType, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = loan;
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
        /// gets a list of loans By Employee Id
        /// </summary>
        [Route("api/GetLoanByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetLoanByEmployee(string EmployeeId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var loans = await _loan.GetByEmployee(EmployeeId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = loans;
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
        /// creates a new Loan record - Mode("Submit": Submits the loan request)
        /// </summary>
        [Route("api/AddLoan/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddLoan([FromBody]PayrollHrpayrollLoanDetail Loan, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _loan.Add(Loan, Mode, tokenObj);

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
        /// Recalc Loan - returns a recomputed Loan object
        /// </summary>
        /// <param name="Loan"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/RecalcLoan/{token}")]
        [HttpPost]
        public async Task<IActionResult> RecalcLoan([FromBody]PayrollHrpayrollLoanDetail Loan, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _loan.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _loan.Recalc(Loan, tokenObj);

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
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
