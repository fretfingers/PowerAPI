using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Helper;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Requisition API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class RequisitionController : ControllerBase
    {
        IRequisition _requisition;

        /// <summary>
        /// Requisition controller
        /// </summary>
        public RequisitionController(IRequisition requisition)
        {
            _requisition = requisition;
        }

        /// <summary>
        /// gets a list of Requisition records
        /// </summary>
        [Route("api/GetRequisition/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRequisition(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var requisitions = await _requisition.GetAll(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = requisitions;
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
        /// gets list of requisition type
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetRequisitionType/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRequisitionType(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var requisitionsType = await _requisition.GetRequisitionType(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = requisitionsType;
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
        /// gets Requisition record by Id
        /// </summary>
        [Route("api/GetRequisitionById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRequisitionById(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var requisition = await _requisition.GetById(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = requisition;
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
        /// gets a list of Requisition By Employee Id
        /// </summary>
        [Route("api/GetRequisitionByEmployee/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRequisitionByEmployee(string EmployeeId, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var requisitions = await _requisition.GetByEmployee(EmployeeId, Mode, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = requisitions;

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
        /// creates a new Requisition record - Mode("Submit": Submits the Requisition request)
        /// </summary>
        [Route("api/AddRequisition/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddRequisition([FromBody]Data.ViewModels.Requisitions requisitions, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _requisition.Add(requisitions, Mode, tokenObj);

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
        /// gets a list of Warehouses
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetWarehouse/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouse(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var warehouses = await _requisition.GetWarehouse(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = warehouses;
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
        /// gets a list of Warehouse Bins By Warehouse
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetBinsByWarehouse/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetBinsByWarehouse(string WarehouseId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var warehouseBins = await _requisition.GetBinsByWarehouse(WarehouseId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = warehouseBins;
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
        /// gets a list of Inventory Items (Mode: Stock or Service. to see stock or service items). When not specified spools all items
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetInventoryItems/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems( string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var inventoryItems = await _requisition.GetInventoryItems( Mode, tokenObj);
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = inventoryItems;
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
        /// gets a list of Inventory Items (Mode: Stock or Service. to see stock or service items). When not specified spools all items
        /// </summary>
        /// <param name="itemsPerPage"></param>
        /// <param name="PageNumber"></param>
        /// <param name="Mode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/GetInventoryCatalog/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryCatalog(int itemsPerPage, int PageNumber, string Mode, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _requisition.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var inventoryItems = await _requisition.GetInventoryCatalog(itemsPerPage, PageNumber, Mode, tokenObj);

                       statusMessage.Metadata = inventoryItems.PaginationMetadata;
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = inventoryItems.InventoryList;
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
        /// Approve requisitions
        /// </summary>
        /// <param name="requisitionAppModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        //[Route("api/RequisitionApprove/{token}")]
        //[HttpPost]
        //public async Task<IActionResult> RequisitionApprove(RequisitionAppModel requisitionAppModel, string token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();

        //    try
        //    {
        //        var tokenObj = await _requisition.GetAccess(token);

        //        if (tokenObj != null)
        //        {
        //            if (tokenObj.TotalDays >= 0)
        //            {
        //                var approvedReq = await _requisition.Approve(requisitionAppModel, tokenObj);

        //                statusMessage.Status = "Success";
        //                statusMessage.Message = "Success";
        //                statusMessage.data = approvedReq;
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
        //    catch (Exception)
        //    {
        //        statusMessage.Status = "Failed";
        //        statusMessage.Message = "Unknown Error. Try Again";

        //        return Ok(statusMessage);
        //    }
        //}
    }
}
