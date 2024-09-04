using DevExpress.DataAccess.Native.Web;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Helper;
using PowerAPI.Service.POCO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class Pos : IPos
    {
        EnterpriseContext _DBContext;
       
        public Pos(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }

        public async Task<ApiToken> GetAccess(string token)
        {
            int days = 0;
            ApiToken apiToken = new ApiToken();

            try
            {
                //get the comp on token
                apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                if (apiToken != null)
                {
                    //get reg info
                    var regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                                                            x.DivisionId == apiToken.DivisionId &&
                                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();
                    if (regInfo != null)
                    {
                        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                        apiToken.RegCode = regInfo.RegCode;
                        apiToken.RegCode = regInfo.RegName;
                        apiToken.TotalDays = days;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return apiToken;
        }

        public async Task<IEnumerable<PosshiftCartTable>> GetPosShiftCartSync(DateTime PeriodFrom, DateTime PeriodTo,  ApiToken token)
        {
            List<PosshiftCartTable> posshift = new List<PosshiftCartTable>();
            try
            {
                posshift = await _DBContext.PosshiftCartTable.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TransDateTime.Value.Date >= PeriodFrom.Date &&
                                                        x.TransDateTime.Value.Date <= PeriodTo.Date &&
                                                       (x.Synchronized == false || 
                                                        x.Synchronized == null))
                                                        .ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return posshift;
        }
        //List<PosshiftCartTable> posShiftCart = new List<PosshiftCartTable>();

        public async Task<StatusMessage> AddPosShiftCart(PosshiftCartTable posshiftCart, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (posshiftCart != null)
                {

                    var posShift = await _DBContext.PosshiftCartTable.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ShiftId == posshiftCart.ShiftId &&
                                                        x.OrderNumber == posshiftCart.OrderNumber &&
                                                        x.OrderLineNumber == posshiftCart.OrderLineNumber &&
                                                        x.EmployeeId == posshiftCart.EmployeeId &&
                                                        x.TerminalId == posshiftCart.TerminalId).FirstOrDefaultAsync();


                    if (posShift != null) // for update payroll policy
                    {

                        posShift.OrderNumber = posshiftCart.OrderNumber;
                        posShift.OrderLineNumber = posshiftCart.OrderLineNumber;
                        posShift.EmployeeId = posshiftCart.EmployeeId;
                        posShift.TerminalId = posshiftCart.TerminalId;
                        posShift.CustomerId = posshiftCart.CustomerId;
                        posShift.TransDateTime = posshiftCart.TransDateTime;
                        posShift.ItemId = posshiftCart.ItemId;
                        posShift.ItemName = posshiftCart.ItemName;
                        posShift.ItemDescription = posshiftCart.ItemDescription;
                        posShift.Quantity = posshiftCart.Quantity;
                        posShift.Price = posshiftCart.Price;
                        posShift.CurrencyId = posshiftCart.CurrencyId;
                        posShift.WarehouseId = posshiftCart.WarehouseId;
                        posShift.ItemWeight = posshiftCart.ItemWeight;
                        posShift.Taxable = posshiftCart.Taxable;
                        posShift.Remove = posshiftCart.Remove;
                        posShift.TaxGroupId = posshiftCart.TaxGroupId;
                        posShift.TaxAmount = posshiftCart.TaxAmount;
                        posShift.PictureUrl = posshiftCart.PictureUrl;
                        posShift.ItemTaxPercent = posshiftCart.ItemTaxPercent;
                        posShift.ItemTotalAmount = posshiftCart.ItemTotalAmount;
                        posShift.ItemCost = posshiftCart.ItemCost;
                        posShift.SalesTaxGroupId = posshiftCart.SalesTaxGroupId;
                        posShift.SalesTaxPercent = posshiftCart.SalesTaxPercent;
                        posShift.LockedBy = posshiftCart.LockedBy;
                        posShift.LockTs = posshiftCart.LockTs;
                        posShift.DiscountPer = posshiftCart.DiscountPer;
                        posShift.DiscountAmt = posshiftCart.DiscountAmt;
                        posShift.Status = posshiftCart.Status;
                        posShift.Posted = posshiftCart.Posted;
                        posShift.NoOfPerson = posshiftCart.NoOfPerson;
                        posShift.Served = posshiftCart.Served;
                        posShift.OrderStartDateTime = posshiftCart.OrderStartDateTime;
                        posShift.OrderEndDateTime = posshiftCart.OrderEndDateTime;
                        posShift.Reconcilled = posshiftCart.Reconcilled;
                        posShift.Void = posshiftCart.Void;
                        posShift.LastEditDate = posshiftCart.LastEditDate;
                        posShift.CreationDate = posshiftCart.CreationDate;
                        posShift.VoidBy = posshiftCart.VoidBy;
                        posShift.VoidReason = posshiftCart.VoidReason;
                        posShift.ItemCategoryId = posshiftCart.ItemCategoryId;
                        posShift.Closed = posshiftCart.Closed;
                        posShift.WarehouseBinId = posshiftCart.WarehouseBinId;
                        posShift.Tpintegrated = posshiftCart.Tpintegrated;
                        posShift.ServeStatus = posshiftCart.ServeStatus;
                        posShift.TableId = posshiftCart.TableId;
                        posShift.WaitressId = posshiftCart.WaitressId;
                        posShift.BranchCode = posshiftCart.BranchCode;
                        posShift.Merged = posshiftCart.Merged;
                        posShift.MarkForMerge = posshiftCart.MarkForMerge;
                        posShift.OrderNumber = posshiftCart.OrderNumber;
                        posShift.MergedBy = posshiftCart.MergedBy;
                        posShift.MergedDate = posshiftCart.MergedDate;
                        posShift.OrderNumberOld = posshiftCart.OrderNumberOld;
                        posShift.IsMerged = posshiftCart.IsMerged;
                        posShift.Synchronized = posshiftCart.Synchronized;
                        posShift.SynchronizedBy = posshiftCart.SynchronizedBy;
                        posShift.SychronizedDate = posshiftCart.SychronizedDate;
                        posShift.OnHold = posshiftCart.OnHold;
                        posShift.TrackingNumber = posshiftCart.TrackingNumber;
                        posShift.TrackingAmount = posshiftCart.TrackingAmount;
                        posShift.Printed = posshiftCart.Printed;
                        posShift.CustomerRoomNo = posshiftCart.CustomerRoomNo;
                        posShift.LastModifiedBit = posshiftCart.LastModifiedBit;
                        posShift.OldQuantity = posshiftCart.OldQuantity;
                        posShift.CustomerPhone = posshiftCart.CustomerPhone;
                        posShift.CustomerName = posshiftCart.CustomerName;
                        posShift.CustomerEmail = posshiftCart.CustomerEmail;
                        posShift.MultipleDiscountGroupId = posshiftCart.MultipleDiscountGroupId;
                        posShift.ReceiveOrderCashier = posshiftCart.ReceiveOrderCashier;
                        posShift.OrderNumber = posshiftCart.OrderNumber;



                        _DBContext.Entry(posShift).State = EntityState.Modified;
                                        _DBContext.SaveChanges();

                                        statusMessage.Status = "Success";
                                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll loan policy
                    {

                        posshiftCart.CompanyId = token.CompanyId;
                        posshiftCart.DivisionId = token.DivisionId;
                        posshiftCart.DepartmentId = token.DepartmentId;
                        posshiftCart.OrderLineNumber = 0;

                        _DBContext.Entry(posshiftCart).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid POS Shift Cart Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddPosShiftCartBatch(List<PosshiftCartTable> posshiftCartBatch, ApiToken token)
        {
          //List<PosshiftCartTable> posShiftCart = new List<PosshiftCartTable>();

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (posshiftCartBatch != null)
                {
                    foreach (PosshiftCartTable posshiftCart in posshiftCartBatch)
                    {
                        PosshiftCartTable posShift = await _DBContext.PosshiftCartTable.Where(x =>
                                                    x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.ShiftId == posshiftCart.ShiftId &&
                                                    x.OrderNumber == posshiftCart.OrderNumber &&
                                                    x.OrderLineNumber == posshiftCart.OrderLineNumber &&
                                                    x.EmployeeId == posshiftCart.EmployeeId &&
                                                    x.TerminalId == posshiftCart.TerminalId).FirstOrDefaultAsync();
                         if (posShift != null) // for update payroll policy
                         {
                            posShift.OrderNumber = posshiftCart.OrderNumber;
                            posShift.OrderLineNumber = posshiftCart.OrderLineNumber;
                            posShift.EmployeeId = posshiftCart.EmployeeId;
                            posShift.TerminalId = posshiftCart.TerminalId;
                            posShift.CustomerId = posshiftCart.CustomerId;
                            posShift.TransDateTime = posshiftCart.TransDateTime;
                            posShift.ItemId = posshiftCart.ItemId;
                            posShift.ItemName = posshiftCart.ItemName;
                            posShift.ItemDescription = posshiftCart.ItemDescription;
                            posShift.Quantity = posshiftCart.Quantity;
                            posShift.Price = posshiftCart.Price;
                            posShift.CurrencyId = posshiftCart.CurrencyId;
                            posShift.WarehouseId = posshiftCart.WarehouseId;
                            posShift.ItemWeight = posshiftCart.ItemWeight;
                            posShift.Taxable = posshiftCart.Taxable;
                            posShift.Remove = posshiftCart.Remove;
                            posShift.TaxGroupId = posshiftCart.TaxGroupId;
                            posShift.TaxAmount = posshiftCart.TaxAmount;
                            posShift.PictureUrl = posshiftCart.PictureUrl;
                            posShift.ItemTaxPercent = posshiftCart.ItemTaxPercent;
                            posShift.ItemTotalAmount = posshiftCart.ItemTotalAmount;
                            posShift.ItemCost = posshiftCart.ItemCost;
                            posShift.SalesTaxGroupId = posshiftCart.SalesTaxGroupId;
                            posShift.SalesTaxPercent = posshiftCart.SalesTaxPercent;
                            posShift.LockedBy = posshiftCart.LockedBy;
                            posShift.LockTs = posshiftCart.LockTs;
                            posShift.DiscountPer = posshiftCart.DiscountPer;
                            posShift.DiscountAmt = posshiftCart.DiscountAmt;
                            posShift.Status = posshiftCart.Status;
                            posShift.Posted = posshiftCart.Posted;
                            posShift.NoOfPerson = posshiftCart.NoOfPerson;
                            posShift.Served = posshiftCart.Served;
                            posShift.OrderStartDateTime = posshiftCart.OrderStartDateTime;
                            posShift.OrderEndDateTime = posshiftCart.OrderEndDateTime;
                            posShift.Reconcilled = posshiftCart.Reconcilled;
                            posShift.Void = posshiftCart.Void;
                            posShift.LastEditDate = posshiftCart.LastEditDate;
                            posShift.CreationDate = posshiftCart.CreationDate;
                            posShift.VoidBy = posshiftCart.VoidBy;
                            posShift.VoidReason = posshiftCart.VoidReason;
                            posShift.ItemCategoryId = posshiftCart.ItemCategoryId;
                            posShift.Closed = posshiftCart.Closed;
                            posShift.WarehouseBinId = posshiftCart.WarehouseBinId;
                            posShift.Tpintegrated = posshiftCart.Tpintegrated;
                            posShift.ServeStatus = posshiftCart.ServeStatus;
                            posShift.TableId = posshiftCart.TableId;
                            posShift.WaitressId = posshiftCart.WaitressId;
                            posShift.BranchCode = posshiftCart.BranchCode;
                            posShift.Merged = posshiftCart.Merged;
                            posShift.MarkForMerge = posshiftCart.MarkForMerge;
                            posShift.OrderNumber = posshiftCart.OrderNumber;
                            posShift.MergedBy = posshiftCart.MergedBy;
                            posShift.MergedDate = posshiftCart.MergedDate;
                            posShift.OrderNumberOld = posshiftCart.OrderNumberOld;
                            posShift.IsMerged = posshiftCart.IsMerged;
                            posShift.Synchronized = posshiftCart.Synchronized;
                            posShift.SynchronizedBy = posshiftCart.SynchronizedBy;
                            posShift.SychronizedDate = posshiftCart.SychronizedDate;
                            posShift.OnHold = posshiftCart.OnHold;
                            posShift.TrackingNumber = posshiftCart.TrackingNumber;
                            posShift.TrackingAmount = posshiftCart.TrackingAmount;
                            posShift.Printed = posshiftCart.Printed;
                            posShift.CustomerRoomNo = posshiftCart.CustomerRoomNo;
                            posShift.LastModifiedBit = posshiftCart.LastModifiedBit;
                            posShift.OldQuantity = posshiftCart.OldQuantity;
                            posShift.CustomerPhone = posshiftCart.CustomerPhone;
                            posShift.CustomerName = posshiftCart.CustomerName;
                            posShift.CustomerEmail = posshiftCart.CustomerEmail;
                            posShift.MultipleDiscountGroupId = posshiftCart.MultipleDiscountGroupId;
                            posShift.ReceiveOrderCashier = posshiftCart.ReceiveOrderCashier;
                            posShift.OrderNumber = posshiftCart.OrderNumber;



                            _DBContext.Entry(posShift).State = EntityState.Modified;
                            _DBContext.SaveChanges();

                            statusMessage.Status = "Success";
                            statusMessage.Message = "Success";
                         }
                        else // insert or create payroll loan policy
                        {
                            //List<PosshiftCartTable> posshiftCart = new List<PosshiftCartTable>();
                            foreach (PosshiftCartTable posshift in posshiftCartBatch)
                            {
                                posshift.CompanyId = token.CompanyId;
                                posshift.DivisionId = token.DivisionId;
                                posshift.DepartmentId = token.DepartmentId;
                                posshift.OrderLineNumber = 0;

                                _DBContext.Entry(posshift).State = EntityState.Added;
                                _DBContext.SaveChanges();

                                statusMessage.Status = "Success";
                                statusMessage.Message = "Success";

                            }



                        }
                    }
                    
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid POS Shift Cart Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }
    }    
}
