using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using PowerAPI.Helper;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Data.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Appraisal API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class AppraisalController : ControllerBase
    {
        IAppraisal _appraisal;

        /// <summary>
        /// Leave controller
        /// </summary>
        /// <param name="appraisal"></param>
        public AppraisalController(IAppraisal appraisal)
        {
            _appraisal = appraisal;
        }

        /// <summary>
        /// gets a list of Appraisal records
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetAppraisal/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetAppraisal(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var appraisal = await _appraisal.GetAll(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = appraisal;
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
        /// gets Appraisal record by Id
        /// </summary>
        /// <param name="EmployeeId">Appraisal Id</param>
        /// <param name="PeriodId">Appraisal PeriodId</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetAppraisalById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetAppraisalById(string EmployeeId, string PeriodId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var appraisal = await _appraisal.GetById(EmployeeId, PeriodId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = appraisal;
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
        /// gets a list of Appraisal By Employee Id
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="Mode"> This should be one of the following :- Current, History</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetAppraisalByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetAppraisalByEmployee(string EmployeeId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var appraisal = await _appraisal.GetByEmployee(EmployeeId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = appraisal;

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
        /// loads Employee Appraisal KPI's
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/LoadAppraisalScoreCard/{token}")]
        [HttpGet]
        public async Task<IActionResult> LoadAppraisalScoreCard(string EmployeeId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var appraisal = await _appraisal.LoadAppraisalScoreCard(EmployeeId, tokenObj);

                        if (appraisal == null)
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "No Active Appraisal. Contact Your System Administrator";
                            statusMessage.data = appraisal;
                        }
                        else
                        {
                            statusMessage.Status = "Success";
                            statusMessage.Message = "Success";
                            statusMessage.data = appraisal;
                        }

                        

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
        /// creates a new Appraisal record
        /// </summary>
        /// <param name="appraisals"> Single Appraisal object to be added or modified</param>
        /// <param name="Mode"> This can either be blank or set as "Submit" ("Submit": Submits the Appraisal)</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisal/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisal([FromBody]Data.ViewModels.Appraisal appraisals, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.Add(appraisals, Mode, tokenObj);

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
       /// creates or updates Appraisal KPI's - Qualitative Objectives
       /// </summary>
       /// <param name="appraisalDetail">Appraisal KPI detail list to be added or modified</param>
       /// <param name="token"></param>
       /// <returns></returns>
        [Route("api/AddAppraisalDetail/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalDetail([FromBody]List<PayrollHrpayrollAppraisalDetail> appraisalDetail, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddDetail(appraisalDetail, tokenObj);

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
        /// creates or updates Appraisal KPI's - Quantitative Objectives
        /// </summary>
        /// <param name="appraisalEmployeeDetail">Appraisal KPI detail list to be added or modified</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisalEmployeeDetail/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalEmployeeDetail([FromBody]List<PayrollHrpayrollEmployeeAppraisalDetail> appraisalEmployeeDetail, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddEmployeeDetail(appraisalEmployeeDetail, tokenObj);

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
        /// creates or updates Appraisal Questionnaire
        /// </summary>
        /// <param name="appraisalQuestionnaire">Appraisal Questionnaire list to be added or modified</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisalQuestionnaire/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalQuestionnaire([FromBody]List<PayrollHrpayrollAppraisalQuestionnaire> appraisalQuestionnaire, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddQuestionnaire(appraisalQuestionnaire, tokenObj);

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
        /// creates or updates Appraisal Training
        /// </summary>
        /// <param name="appraisalTraining">Appraisal Training list to be added or modified</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisalTraining/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalTraining([FromBody]List<PayrollHrpayrollTrainingDetail> appraisalTraining, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddTraining(appraisalTraining, tokenObj);

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
        /// creates or updates Appraisal Manager Comments
        /// </summary>
        /// <param name="appraisalComments">Appraisal Comment list to be added or modified</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisalComments/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalComments([FromBody]List<PayrollHrpayrollAppraisalComments> appraisalComments, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddComments(appraisalComments, tokenObj);

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
        /// creates or updates Appraisal HR Comments
        /// </summary>
        /// <param name="appraisalOthers">Appraisal Other Comments list to be added or modified</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/AddAppraisalOthers/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddAppraisalOthers([FromBody]List<PayrollHrpayrollAppraisalOthers> appraisalOthers, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _appraisal.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _appraisal.AddOthers(appraisalOthers, tokenObj);

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


    }
}
