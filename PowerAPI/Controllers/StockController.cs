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
    /// Stock API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class StockController : ControllerBase
    {
        IStock _stock;

        /// <summary>
        /// Stock Constructor
        /// </summary>
        /// <param name="stock"></param>
        public StockController(IStock stock)
        {
            _stock = stock;
        }

        /// <summary>
        /// gets a list of stocks
        /// </summary>
        [Route("api/GetStock/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStock([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStock(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by warehouse
        /// </summary>
        [Route("api/GetStockByWarehouse/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockByWarehouse([FromQuery] PaginationParams Param, string warehouseId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockByWarehouse(Param, warehouseId, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by warehouse and bin
        /// </summary>
        [Route("api/GetStockByWarehouseByBin/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockByWarehouseByBin([FromQuery] PaginationParams Param, string warehouseId, string warehouseBinId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockByWarehouseByBin(Param, warehouseId, warehouseBinId, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by item family
        /// </summary>
        [Route("api/GetStockByItemFamily/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockByItemFamily([FromQuery] PaginationParams Param, string itemFamilyId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockByItemFamily(Param, itemFamilyId, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by id
        /// </summary>
        [Route("api/GetStockById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockById([FromQuery] PaginationParams Param, string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockById(Param, Id, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by name
        /// </summary>
        [Route("api/GetStockByName/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockByName([FromQuery] PaginationParams Param, string name, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockByName(Param, name, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
        /// gets a list of stocks by name
        /// </summary>
        [Route("api/GetStockByWarehouseUser/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetStockByWarehouseUser([FromQuery] PaginationParams Param, string userId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _stock.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _stock.GetStockByWarehouseUser(Param, userId, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.InventoryByWarehouseList;
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
