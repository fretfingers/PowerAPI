using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Helper;
using PowerAPI.Service.POCO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Sales API endpoints
    /// </summary>
    public class PosController : ControllerBase
    {
        IPos _pos;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Setup Constructor
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="appSettings"></param>
        public PosController(IPos pos, IOptions<AppSettings> appSettings)
        {
            _pos = pos;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// gets a list of Pos Shift Cart records
        /// </summary>
        [Route("api/GetPosShiftCartSync/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetPosShiftCartSync(DateTime PeriodFrom, DateTime PeriodTo, string token)
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
                var tokenObj = await _pos.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var posshift = await _pos.GetPosShiftCartSync(PeriodFrom, PeriodTo, tokenObj);
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = posshift;
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
        /// updates Pos Shift Cart
        /// <param name="token"></param>
        /// <param name="posshiftCart"></param>
        /// <returns></returns>
        [Route("api/AddPosShiftCart/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddPosShiftCart([FromBody] PosshiftCartTable posshiftCart, string token)
        { 
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _pos.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _pos.AddPosShiftCart(posshiftCart, tokenObj);

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
        /// updates Pos Shift Cart
        /// <param name="token"></param>
        /// <param name="posshiftCartBatch"></param>
        /// <returns></returns>
        [Route("api/AddPosShiftCartBatch/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddPosShiftCartBatch([FromBody] List<PosshiftCartTable> posshiftCartBatch, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _pos.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _pos.AddPosShiftCartBatch(posshiftCartBatch, tokenObj);

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
