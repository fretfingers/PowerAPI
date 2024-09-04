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
    /// Fixed Assets API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class FixedAssetsController : ControllerBase
    {
        IFixedAssets _fixedAssets;

        /// <summary>
        /// Fixed Assets Constructor
        /// </summary>
        /// <param name="fixedAssets"></param>
        public FixedAssetsController(IFixedAssets fixedAssets)
        {
            _fixedAssets = fixedAssets;
        }

        /// <summary>
        /// gets a list of fixed Assets
        /// </summary>
        [Route("api/GetFixedAssets/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetFixedAssets([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _fixedAssets.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _fixedAssets.GetFixedAssets(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.FixedAssetsList;
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
        /// gets a list of fixed Assets by Type
        /// </summary>
        [Route("api/GetFixedAssetsByType/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetFixedAssetsByType([FromQuery] PaginationParams Param, string assetType, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _fixedAssets.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _fixedAssets.GetFixedAssetsByType(Param, assetType, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.FixedAssetsList;
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
        /// gets fixed Assets by Id
        /// </summary>
        [Route("api/GetFixedAssetsById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetFixedAssetsById([FromQuery] PaginationParams Param, string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _fixedAssets.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _fixedAssets.GetFixedAssetsById(Param, Id, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.FixedAssetsList;
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
        /// gets a list of fixed Assets by name
        /// </summary>
        [Route("api/GetFixedAssetsByName/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetFixedAssetsByName([FromQuery] PaginationParams Param, string name, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _fixedAssets.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _fixedAssets.GetFixedAssetsByName(Param, name, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.FixedAssetsList;
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
        /// gets a list of fixed Assets by status
        /// </summary>
        [Route("api/GetFixedAssetsByStatus/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetFixedAssetsByStatus([FromQuery] PaginationParams Param, string status, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _fixedAssets.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _fixedAssets.GetFixedAssetsByStatus(Param, status, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.FixedAssetsList;
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
