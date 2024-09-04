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
    /// Banks API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class BanksController : ControllerBase
    {
        IBanks _banks;

        /// <summary>
        /// Banks Constructor
        /// </summary>
        /// <param name="banks"></param>
        public BanksController(IBanks banks)
        {
            _banks = banks;
        }

        /// <summary>
        /// gets a list of cashbook banks
        /// </summary>
        [Route("api/GetCashbookBanks/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCashbookBanks([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _banks.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _banks.GetCashbookBanks(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.BankAccountsList;
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
