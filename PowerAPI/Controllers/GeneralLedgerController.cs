using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Helper;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Clients;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// General Ledger API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class GeneralLedgerController : ControllerBase
    {
        IGeneralLedger _generalLedger;

        /// <summary>
        /// general ledger Constructor
        /// </summary>
        /// <param name="generalLedger"></param>
        public GeneralLedgerController(IGeneralLedger generalLedger)
        {
            _generalLedger = generalLedger;
        }

        /// <summary>
        /// gets a list of chart of accounts
        /// </summary>
        [Route("api/GetCOA/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCOA([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _generalLedger.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _generalLedger.GetCOA(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.LedgerChartOfAccountList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }

        /// <summary>
        /// gets chart of accounts by Id
        /// </summary>
        [Route("api/GetCOAById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCOAById([FromQuery] PaginationParams Param, string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _generalLedger.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _generalLedger.GetCOAById(Param, Id, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.LedgerChartOfAccountList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }

        /// <summary>
        /// gets a list of chart of accounts foreign
        /// </summary>
        [Route("api/GetCOAForeign/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCOAForeign([FromQuery] PaginationParams Param, string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _generalLedger.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _generalLedger.GetCOAForeign(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.LedgerChartOfAccountList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }

        /// <summary>
        /// gets a list of chart of accounts by account type e.g Revenue, income, Cost of Sales, Expense, Cash, Bank, Fixed Assets etc
        /// </summary>
        [Route("api/GetCOAByAccountType/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCOAByAccountType([FromQuery] PaginationParams Param, string accountType, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _generalLedger.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _generalLedger.GetCOAByAccountType(Param, accountType, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.LedgerChartOfAccountList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }


        /// <summary>
        /// gets a list of chart of accounts by account name
        /// </summary>
        [Route("api/GetCOAByName/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCOAByName([FromQuery] PaginationParams Param, string name, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _generalLedger.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _generalLedger.GetCOAByName(Param, name, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.LedgerChartOfAccountList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }
    }
}
