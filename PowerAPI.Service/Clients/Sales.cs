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
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DevExpress.CodeParser;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using DevExpress.Office.Drawing;
using Humanizer;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace PowerAPI.Service.Clients
{
    public class Sales : ISales
    {
        private readonly EnterpriseContext _DBContext;
        private readonly IApiAuthService _apiAuthService;
        string _NextNumberName = "NextOrderNumber";
        public Sales(EnterpriseContext DBContext, IApiAuthService apiAuthService)
        {
            _DBContext = DBContext;
            _apiAuthService = apiAuthService;
        }



        public async Task<ApiToken> GetAccess(string token)
        {
            //int days = 0;
            //ApiToken apiToken = new ApiToken();

            try
            {

                return await _apiAuthService.GetAccess(token);

                // using

                ////get the comp on token
                //apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                //if (apiToken != null)
                //{
                //    //get reg info
                //    var regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                //                                            x.DivisionId == apiToken.DivisionId &&
                //                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();
                //    if (regInfo != null)
                //    {
                //        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                //        apiToken.RegCode = regInfo.RegCode;
                //        apiToken.RegCode = regInfo.RegName;
                //        apiToken.TotalDays = days;
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }

            //return apiToken;
        }

        public async Task<Paging> GetAllOrders(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.OrderRetrieveDto> orderHeaders = new List<Data.ViewModels.OrderRetrieveDto>();

            try
            {
                var totalCount = await _DBContext.OrderHeader.CountAsync();

                var orderDet = await _DBContext.OrderHeader.OrderByDescending(x => x.OrderDate).Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TransactionTypeId == "Order")
                                                        .OrderBy(x => x.OrderNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage)
                                                        .ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                orderHeaders = await order(orderDet, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    OrderList = orderHeaders
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OrderRetrieveDto>> GetOrderById(string Id, ApiToken token)
        {
            List<OrderRetrieveDto> orders = new List<OrderRetrieveDto>();
            try
            {
                var Order = await _DBContext.OrderHeader.OrderByDescending(x => x.OrderDate).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.OrderNumber == Id &&
                                                           x.TransactionTypeId == "Order").ToListAsync();
                orders = await order(Order, token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return orders;
        }

        public async Task<IEnumerable<OrderRetrieveDto>> GetOrderByCustomerId(string Id, ApiToken token)
        {
            List<OrderRetrieveDto> orderdetail = new List<OrderRetrieveDto>();
            try
            {
                var Order = await _DBContext.OrderHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.CustomerId == Id &&
                                                           x.TransactionTypeId == "Order")
                                                           .OrderByDescending(x => x.OrderNumber).ToListAsync();
                orderdetail = await order(Order, token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return orderdetail;
        }

        private async Task<List<Data.ViewModels.OrderRetrieveDto>> order(List<OrderHeader> orderHeaders, ApiToken token)
        {
            List<Data.ViewModels.OrderRetrieveDto> orders = new List<Data.ViewModels.OrderRetrieveDto>();
            try
            {
                if (orderHeaders != null)
                {
                    foreach (OrderHeader orderHeader in orderHeaders)
                    {
                        var workflows = new List<OrderWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        if (orderHeader.TransactionTypeId == "Order")
                        {

                            workflows = new List<OrderWorkflow>
                                            {
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 1,
                                                    Status = "Booked",
                                                    IsCompleted = orderHeader.Posted ?? false,
                                                    DateCompleted = orderHeader.PostedDate
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 2,
                                                    Status = "On Hold",
                                                    IsCompleted = orderHeader.OrderTypeId == "hold",
                                                    DateCompleted = orderHeader.OrderTypeId == "hold"? orderHeader.PostedDate: null
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 3,
                                                    Status = "Back Ordered",
                                                    IsCompleted = orderHeader.Backordered ?? false,
                                                    DateCompleted = orderHeader.Backordered == true ? orderHeader.PostedDate : null
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 4,
                                                    Status = "Picked",
                                                    IsCompleted = orderHeader.Picked ?? false,
                                                    DateCompleted = orderHeader.Picked == true ? orderHeader.PickedDate : null
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 5,
                                                    Status = "Invoiced",
                                                    IsCompleted = orderHeader.Invoiced ?? false,
                                                    DateCompleted = orderHeader.Invoiced == true ? orderHeader.InvoiceDate : null
                                                }
                                            };
                        }
                        else if (orderHeader.TransactionTypeId == "Quote")
                        {
                            workflows = new List<OrderWorkflow>
                                            {
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 1,
                                                    Status = "Proposed",
                                                    IsCompleted = true,
                                                    DateCompleted = orderHeader.OrderDate
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 2,
                                                    Status = "Accepted",
                                                    IsCompleted = true,
                                                    DateCompleted = orderHeader.OrderDate
                                                },
                                                new OrderWorkflow
                                                {
                                                    OrderNumber = orderHeader.OrderNumber,
                                                    OrderDate = orderHeader.OrderDate ?? currentDate,
                                                    StepSequence = 3,
                                                    Status = "Converted",
                                                    IsCompleted = false,
                                                    DateCompleted = null
                                                }
                                            };
                        }

                        var lastCompletedWorkflow = workflows
                                                        .Where(ow => ow.IsCompleted)
                                                        .OrderBy(workflow => workflow.StepSequence)
                                                        .LastOrDefault();




                        Data.ViewModels.OrderRetrieveDto orderObj = new Data.ViewModels.OrderRetrieveDto();

                        orderObj.CompanyId = orderHeader.CompanyId;
                        orderObj.DivisionId = orderHeader.DivisionId;
                        orderObj.DepartmentId = orderHeader.DepartmentId;
                        orderObj.OrderNumber = orderHeader.OrderNumber;
                        orderObj.TransactionTypeId = orderHeader.TransactionTypeId;
                        orderObj.OrderTypeId = orderHeader.OrderTypeId;
                        orderObj.OrderDate = orderHeader.OrderDate;
                        orderObj.OrderDueDate = orderHeader.OrderDueDate;
                        orderObj.OrderShipDate = orderHeader.OrderShipDate;
                        orderObj.OrderCancelDate = orderHeader.OrderCancelDate;
                        orderObj.SystemDate = orderHeader.SystemDate;
                        orderObj.Memorize = orderHeader.Memorize;
                        orderObj.PurchaseOrderNumber = orderHeader.PurchaseOrderNumber;
                        orderObj.TaxExemptId = orderHeader.TaxExemptId;
                        orderObj.TaxGroupId = orderHeader.TaxGroupId;
                        orderObj.CustomerId = orderHeader.CustomerId;
                        orderObj.TermsId = orderHeader.TermsId;
                        orderObj.CurrencyId = orderHeader.CurrencyId;
                        orderObj.CurrencyExchangeRate = orderHeader.CurrencyExchangeRate;
                        orderObj.Subtotal = orderHeader.Subtotal;
                        orderObj.DiscountPers = orderHeader.DiscountPers;
                        orderObj.DiscountAmount = orderHeader.DiscountAmount;
                        orderObj.TaxPercent = orderHeader.TaxPercent;
                        orderObj.TaxAmount = orderHeader.TaxAmount;
                        orderObj.TaxableSubTotal = orderHeader.TaxableSubTotal;
                        orderObj.Freight = orderHeader.Freight;
                        orderObj.TaxFreight = orderHeader.TaxFreight;
                        orderObj.Handling = orderHeader.Handling;
                        orderObj.Advertising = orderHeader.Advertising;
                        orderObj.Total = orderHeader.Total;
                        orderObj.EmployeeId = orderHeader.EmployeeId;
                        orderObj.Commission = orderHeader.Commission;
                        orderObj.CommissionableSales = orderHeader.CommissionableSales;
                        orderObj.ComissionalbleCost = orderHeader.ComissionalbleCost;
                        orderObj.CustomerDropShipment = orderHeader.CustomerDropShipment;
                        orderObj.ShipMethodId = orderHeader.ShipMethodId;
                        orderObj.WarehouseId = orderHeader.WarehouseId;
                        orderObj.ShipForId = orderHeader.ShipForId;
                        orderObj.ShipToId = orderHeader.ShipToId;
                        orderObj.ShippingName = orderHeader.ShippingName;
                        orderObj.ShippingAddress1 = orderHeader.ShippingAddress1;
                        orderObj.ShippingAddress2 = orderHeader.ShippingAddress2;
                        orderObj.ShippingAddress3 = orderHeader.ShippingAddress3;
                        orderObj.ShippingCity = orderHeader.ShippingCity;
                        orderObj.ShippingState = orderHeader.ShippingState;
                        orderObj.ShippingZip = orderHeader.ShippingZip;
                        orderObj.ShippingCountry = orderHeader.ShippingCountry;
                        //orderObj.ScheduledStartDate = orderHeader.ScheduledStartDate;
                        //orderObj.ScheduledEndDate = orderHeader.ScheduledEndDate;
                        //orderObj.ServiceStartDate = orderHeader.ServiceStartDate;
                        //orderObj.ServiceEndDate = orderHeader.ServiceEndDate;
                        orderObj.PerformedBy = orderHeader.PerformedBy;
                        orderObj.GlsalesAccount = orderHeader.GlsalesAccount;
                        orderObj.Glcogaccount = orderHeader.Glcogaccount;
                        orderObj.PaymentMethodId = orderHeader.PaymentMethodId;
                        orderObj.AmountPaid = orderHeader.AmountPaid;
                        orderObj.BalanceDue = orderHeader.BalanceDue;
                        orderObj.UndistributedAmount = orderHeader.UndistributedAmount;
                        orderObj.CheckNumber = orderHeader.CheckNumber;
                        orderObj.CheckDate = orderHeader.CheckDate;
                        orderObj.CreditCardTypeId = orderHeader.CreditCardTypeId;
                        orderObj.CreditCardName = orderHeader.CreditCardName;
                        orderObj.CreditCardNumber = orderHeader.CreditCardNumber;
                        orderObj.CreditCardCsvnumber = orderHeader.CreditCardCsvnumber;
                        orderObj.CreditCardExpDate = orderHeader.CreditCardExpDate;
                        orderObj.CreditCardBillToZip = orderHeader.CreditCardBillToZip;
                        orderObj.CreditCardValidationCode = orderHeader.CreditCardValidationCode;
                        orderObj.Backordered = orderHeader.Backordered;
                        orderObj.Picked = orderHeader.Picked;
                        orderObj.PickedDate = orderHeader.PickedDate;
                        orderObj.Printed = orderHeader.Printed;
                        orderObj.PrintedDate = orderHeader.PrintedDate;
                        orderObj.Shipped = orderHeader.Shipped;
                        orderObj.ShipDate = orderHeader.ShipDate;
                        orderObj.TrackingNumber = orderHeader.TrackingNumber;
                        orderObj.Billed = orderHeader.Billed;
                        orderObj.Invoiced = orderHeader.Invoiced;
                        orderObj.InvoiceDate = orderHeader.InvoiceDate;
                        orderObj.InvoiceNumber = orderHeader.InvoiceNumber;
                        orderObj.Posted = orderHeader.Posted;
                        orderObj.PostedDate = orderHeader.PostedDate;
                        orderObj.AllowanceDiscountPerc = orderHeader.AllowanceDiscountPerc;
                        orderObj.CashTendered = orderHeader.CashTendered;
                        orderObj.MasterBillOfLading = orderHeader.MasterBillOfLading;
                        orderObj.MasterBillOfLadingDate = orderHeader.MasterBillOfLadingDate;
                        orderObj.TrailerNumber = orderHeader.TrailerNumber;
                        orderObj.TrailerPrefix = orderHeader.TrailerPrefix;
                        orderObj.HeaderMemo1 = orderHeader.HeaderMemo1;
                        orderObj.HeaderMemo2 = orderHeader.HeaderMemo2;
                        orderObj.HeaderMemo3 = orderHeader.HeaderMemo3;
                        orderObj.HeaderMemo4 = orderHeader.HeaderMemo4;
                        orderObj.HeaderMemo5 = orderHeader.HeaderMemo5;
                        orderObj.HeaderMemo6 = orderHeader.HeaderMemo6;
                        orderObj.HeaderMemo7 = orderHeader.HeaderMemo7;
                        orderObj.HeaderMemo8 = orderHeader.HeaderMemo8;
                        orderObj.HeaderMemo9 = orderHeader.HeaderMemo9;
                        orderObj.HeaderMemo10 = orderHeader.HeaderMemo10;
                        //orderObj.HeaderMemo11 = orderHeader.HeaderMemo11;
                        //orderObj.HeaderMemo12 = orderHeader.HeaderMemo12;
                        orderObj.Approved = orderHeader.Approved;
                        orderObj.ApprovedBy = orderHeader.ApprovedBy;
                        orderObj.ApprovedDate = orderHeader.ApprovedDate;
                        orderObj.Signature = orderHeader.Signature;
                        orderObj.SignaturePassword = orderHeader.SignaturePassword;
                        orderObj.SupervisorPassword = orderHeader.SupervisorPassword;
                        orderObj.SupervisorSignature = orderHeader.SupervisorSignature;
                        orderObj.ManagerSignature = orderHeader.ManagerSignature;
                        orderObj.ManagerPassword = orderHeader.ManagerPassword;
                        orderObj.LockedBy = orderHeader.LockedBy;
                        orderObj.LockTs = orderHeader.LockTs;
                        orderObj.BankId = orderHeader.BankId;
                        orderObj.OriginalOrderNumber = orderHeader.OriginalOrderNumber;
                        orderObj.OriginalOrderDate = orderHeader.OriginalOrderDate;
                        orderObj.DeliveryNumber = orderHeader.DeliveryNumber;
                        //orderObj.Ullage1 = orderHeader.Ullage1;
                        //orderObj.Ullage2 = orderHeader.Ullage2;
                        //orderObj.Ullage3 = orderHeader.Ullage3;
                        //orderObj.Ullage4 = orderHeader.Ullage4;
                        //orderObj.Ullage5 = orderHeader.Ullage5;
                        //orderObj.Ullage6 = orderHeader.Ullage6;
                        //orderObj.Ullage7 = orderHeader.Ullage7;
                        //orderObj.Ullage8 = orderHeader.Ullage8;
                        //orderObj.Ullage9 = orderHeader.Ullage9;
                        //orderObj.Ullage10 = orderHeader.Ullage10;
                        //orderObj.Ullage11 = orderHeader.Ullage11;
                        //orderObj.Ullage12 = orderHeader.Ullage12;
                        orderObj.BranchCode = orderHeader.BranchCode;
                        //orderObj.Merged = orderHeader.Merged;
                        //orderObj.Created = orderHeader.Created;
                        //orderObj.FinanceApproved = orderHeader.FinanceApproved;
                        //orderObj.FinanceApprovedDate = orderHeader.FinanceApprovedDate;
                        //orderObj.FinanceComment = orderHeader.FinanceComment;
                        //orderObj.FinanceReturnedDate = orderHeader.FinanceReturnedDate;
                        //orderObj.Bdmapproved = orderHeader.Bdmapproved;
                        //orderObj.BdmapprovedDate = orderHeader.BdmapprovedDate;
                        //orderObj.Bdmcomment = orderHeader.Bdmcomment;
                        //orderObj.Fmapproved = orderHeader.Fmapproved;
                        //orderObj.FmapprovedDate = orderHeader.FmapprovedDate;
                        //orderObj.Fmcomment = orderHeader.Fmcomment;
                        //orderObj.Mdapproved = orderHeader.Mdapproved;
                        //orderObj.MdapprovedDate = orderHeader.MdapprovedDate;
                        //orderObj.Mdcomment = orderHeader.Mdcomment;
                        //orderObj.Regularized = orderHeader.Regularized;
                        //orderObj.Fmvoid = orderHeader.Fmvoid;
                        //orderObj.FmvoidedDate = orderHeader.FmvoidedDate;
                        //orderObj.ReceiptId = orderHeader.ReceiptId;
                        //orderObj.CommercialComment = orderHeader.CommercialComment;
                        //orderObj.FinanceApprovedBy = orderHeader.FinanceApprovedBy;
                        //orderObj.FmapprovedBy = orderHeader.FmapprovedBy;
                        //orderObj.CooapprovedBy = orderHeader.CooapprovedBy;

                        orderObj.Status = lastCompletedWorkflow?.Status ?? "Draft";
                        orderObj.WorkFlowTrail = workflows;

                        orderObj.orderDetail = await _DBContext.OrderDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.OrderNumber == orderHeader.OrderNumber).ToListAsync();

                        orders.Add(orderObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return orders;

        }

        public async Task<StatusMessage> AddOrder(Order salesPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<OrderDetail> orderDetail = new List<OrderDetail>();
            Data.Models.CurrencyTypes currency = new Data.Models.CurrencyTypes();

            var tot = 0.0;
            var GrandTotal = 0.0;
            try
            {
                var company = await _DBContext.Companies.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();


                if (company != null)
                {
                    currency = await _DBContext.CurrencyTypes.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.CurrencyId == company.CurrencyId).FirstOrDefaultAsync();
                }
                //check if object empty
                if (salesPolicy != null)
                {
                    var orderHead = await _DBContext.OrderHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == salesPolicy.OrderNumber).FirstOrDefaultAsync();

                    var customer = await _DBContext.CustomerInformation.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                    tot = await CalcSubTot(salesPolicy, token);
                    GrandTotal += tot;

                    if (orderHead != null) // for update sales policy
                    {
                        orderHead.CompanyId = salesPolicy.CompanyId;
                        orderHead.DivisionId = salesPolicy.DivisionId;
                        orderHead.DepartmentId = salesPolicy.DepartmentId;
                        orderHead.OrderNumber = salesPolicy.OrderNumber;
                        orderHead.TransactionTypeId = "Order";
                        orderHead.OrderTypeId = "Order";
                        orderHead.OrderDate = DateTime.Now;
                        orderHead.OrderDueDate = salesPolicy.OrderDueDate;
                        orderHead.OrderShipDate = salesPolicy.OrderShipDate;
                        orderHead.OrderCancelDate = salesPolicy.OrderCancelDate;
                        orderHead.SystemDate = salesPolicy.SystemDate;
                        orderHead.Memorize = salesPolicy.Memorize;
                        orderHead.PurchaseOrderNumber = salesPolicy.PurchaseOrderNumber;
                        orderHead.TaxExemptId = salesPolicy.TaxExemptId;
                        orderHead.TaxGroupId = salesPolicy.TaxGroupId;
                        orderHead.TermsId = salesPolicy.TermsId;
                        orderHead.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        orderHead.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        orderHead.Subtotal = (decimal)tot;
                        orderHead.DiscountPers = salesPolicy.DiscountPers;
                        orderHead.DiscountAmount = salesPolicy.DiscountAmount;
                        orderHead.TaxPercent = salesPolicy.TaxPercent;
                        orderHead.TaxAmount = salesPolicy.TaxAmount;
                        orderHead.TaxableSubTotal = salesPolicy.TaxableSubTotal;
                        orderHead.Freight = salesPolicy.Freight;
                        orderHead.TaxFreight = salesPolicy.TaxFreight;
                        orderHead.Handling = salesPolicy.Handling;
                        orderHead.Advertising = salesPolicy.Advertising;
                        orderHead.Total = (decimal)GrandTotal;
                        orderHead.EmployeeId = salesPolicy.EmployeeId;
                        orderHead.Commission = salesPolicy.Commission;
                        orderHead.CommissionableSales = salesPolicy.CommissionableSales;
                        orderHead.ComissionalbleCost = salesPolicy.ComissionalbleCost;
                        orderHead.CustomerDropShipment = salesPolicy.CustomerDropShipment;
                        orderHead.ShipMethodId = salesPolicy.ShipMethodId;
                        orderHead.WarehouseId = salesPolicy.WarehouseId;
                        orderHead.ShipForId = salesPolicy.ShipForId;
                        orderHead.ShipToId = salesPolicy.ShipToId;
                        orderHead.ShippingName = customer.CustomerName;
                        orderHead.ShippingAddress1 = customer.CustomerAddress1;
                        orderHead.ShippingAddress2 = customer.CustomerAddress2;
                        orderHead.ShippingAddress3 = customer.CustomerAddress3;
                        orderHead.ShippingCity = customer.CustomerCity;
                        orderHead.ShippingState = customer.CustomerState;
                        orderHead.ShippingZip = salesPolicy.ShippingZip;
                        orderHead.ShippingCountry = customer.CustomerCountry;
                        //orderHead.ScheduledStartDate = salesPolicy.ScheduledStartDate;
                        //orderHead.ScheduledEndDate = salesPolicy.ScheduledEndDate;
                        orderHead.ServiceStartDate = salesPolicy.ServiceStartDate;
                        orderHead.ServiceEndDate = salesPolicy.ServiceEndDate;
                        orderHead.PerformedBy = salesPolicy.PerformedBy;
                        orderHead.GlsalesAccount = salesPolicy.GlsalesAccount;
                        orderHead.Glcogaccount = salesPolicy.Glcogaccount;
                        orderHead.PaymentMethodId = salesPolicy.PaymentMethodId;
                        orderHead.AmountPaid = salesPolicy.AmountPaid;
                        orderHead.BalanceDue = salesPolicy.BalanceDue;
                        orderHead.UndistributedAmount = salesPolicy.UndistributedAmount;
                        orderHead.CheckNumber = salesPolicy.CheckNumber;
                        orderHead.CheckDate = salesPolicy.CheckDate;
                        orderHead.CreditCardTypeId = salesPolicy.CreditCardTypeId;
                        orderHead.CreditCardName = salesPolicy.CreditCardName;
                        orderHead.CreditCardNumber = salesPolicy.CreditCardNumber;
                        orderHead.CreditCardCsvnumber = salesPolicy.CreditCardCsvnumber;
                        orderHead.CreditCardExpDate = salesPolicy.CreditCardExpDate;
                        orderHead.CreditCardBillToZip = salesPolicy.CreditCardBillToZip;
                        orderHead.CreditCardValidationCode = salesPolicy.CreditCardValidationCode;
                        orderHead.Backordered = salesPolicy.Backordered;
                        orderHead.Picked = salesPolicy.Picked;
                        orderHead.PickedDate = salesPolicy.PickedDate;
                        orderHead.Printed = salesPolicy.Printed;
                        orderHead.PrintedDate = salesPolicy.PrintedDate;
                        orderHead.Shipped = salesPolicy.Shipped;
                        orderHead.ShipDate = salesPolicy.ShipDate;
                        orderHead.TrackingNumber = salesPolicy.TrackingNumber;
                        orderHead.Billed = salesPolicy.Billed;
                        orderHead.Invoiced = salesPolicy.Invoiced;
                        orderHead.InvoiceDate = salesPolicy.InvoiceDate;
                        orderHead.InvoiceNumber = salesPolicy.InvoiceNumber;
                        orderHead.Posted = salesPolicy.Posted;
                        orderHead.PostedDate = salesPolicy.PostedDate;
                        orderHead.AllowanceDiscountPerc = salesPolicy.AllowanceDiscountPerc;
                        orderHead.CashTendered = salesPolicy.CashTendered;
                        orderHead.MasterBillOfLading = salesPolicy.MasterBillOfLading;
                        orderHead.MasterBillOfLadingDate = salesPolicy.MasterBillOfLadingDate;
                        orderHead.TrailerNumber = salesPolicy.TrailerNumber;
                        orderHead.TrailerPrefix = salesPolicy.TrailerPrefix;
                        orderHead.HeaderMemo1 = salesPolicy.HeaderMemo1;
                        orderHead.HeaderMemo2 = salesPolicy.HeaderMemo2;
                        orderHead.HeaderMemo3 = salesPolicy.HeaderMemo3;
                        orderHead.HeaderMemo4 = salesPolicy.HeaderMemo4;
                        orderHead.HeaderMemo5 = salesPolicy.HeaderMemo5;
                        orderHead.HeaderMemo6 = salesPolicy.HeaderMemo6;
                        orderHead.HeaderMemo7 = salesPolicy.HeaderMemo7;
                        orderHead.HeaderMemo8 = salesPolicy.HeaderMemo8;
                        orderHead.HeaderMemo9 = salesPolicy.HeaderMemo9;
                        orderHead.HeaderMemo10 = salesPolicy.HeaderMemo10;
                        orderHead.HeaderMemo11 = salesPolicy.HeaderMemo11;
                        orderHead.HeaderMemo12 = "Mobile Sales";
                        orderHead.Approved = salesPolicy.Approved;
                        orderHead.ApprovedBy = salesPolicy.ApprovedBy;
                        orderHead.ApprovedDate = salesPolicy.ApprovedDate;
                        orderHead.Signature = salesPolicy.Signature;
                        orderHead.SignaturePassword = salesPolicy.SignaturePassword;
                        orderHead.SupervisorPassword = salesPolicy.SupervisorPassword;
                        orderHead.SupervisorSignature = salesPolicy.SupervisorSignature;
                        orderHead.ManagerSignature = salesPolicy.ManagerSignature;
                        orderHead.ManagerPassword = salesPolicy.ManagerPassword;
                        orderHead.LockedBy = salesPolicy.LockedBy;
                        orderHead.LockTs = salesPolicy.LockTs;
                        orderHead.BankId = "CASH";
                        orderHead.OriginalOrderNumber = salesPolicy.OriginalOrderNumber;
                        orderHead.OriginalOrderDate = salesPolicy.OriginalOrderDate;
                        orderHead.DeliveryNumber = salesPolicy.DeliveryNumber;
                        //orderHead.Ullage1 = salesPolicy.Ullage1;
                        //orderHead.Ullage2 = salesPolicy.Ullage2;
                        //orderHead.Ullage3 = salesPolicy.Ullage3;
                        //orderHead.Ullage4 = salesPolicy.Ullage4;
                        //orderHead.Ullage5 = salesPolicy.Ullage5;
                        //orderHead.Ullage6 = salesPolicy.Ullage6;
                        //orderHead.Ullage7 = salesPolicy.Ullage7;
                        //orderHead.Ullage8 = salesPolicy.Ullage8;
                        //orderHead.Ullage9 = salesPolicy.Ullage9;
                        //orderHead.Ullage10 = salesPolicy.Ullage10;
                        //orderHead.Ullage11 = salesPolicy.Ullage11;
                        //orderHead.Ullage12 = salesPolicy.Ullage12;
                        orderHead.BranchCode = salesPolicy.BranchCode;
                        //orderHead.Merged = false; // salesPolicy.Merged;
                        //orderHead.Created = null; salesPolicy.Created;
                        //orderHead.FinanceApproved = salesPolicy.FinanceApproved;
                        //orderHead.FinanceApprovedDate = salesPolicy.FinanceApprovedDate;
                        //orderHead.FinanceComment = salesPolicy.FinanceComment;
                        //orderHead.FinanceReturnedDate = salesPolicy.FinanceReturnedDate;
                        //orderHead.Bdmapproved = salesPolicy.Bdmapproved;
                        //orderHead.BdmapprovedDate = salesPolicy.BdmapprovedDate;
                        //orderHead.Bdmcomment = salesPolicy.Bdmcomment;
                        //orderHead.Fmapproved = salesPolicy.Fmapproved;
                        //orderHead.FmapprovedDate = salesPolicy.FmapprovedDate;
                        //orderHead.Fmcomment = salesPolicy.Fmcomment;
                        //orderHead.Mdapproved = salesPolicy.Mdapproved;
                        //orderHead.MdapprovedDate = salesPolicy.MdapprovedDate;
                        //orderHead.Mdcomment = salesPolicy.Mdcomment;
                        //orderHead.Regularized = salesPolicy.Regularized;
                        //orderHead.Fmvoid = salesPolicy.Fmvoid;
                        //orderHead.FmvoidedDate = salesPolicy.FmvoidedDate;
                        //orderHead.ReceiptId = salesPolicy.ReceiptId;
                        //orderHead.CommercialComment = salesPolicy.CommercialComment;
                        //orderHead.FinanceApprovedBy = salesPolicy.FinanceApprovedBy;
                        //orderHead.FmapprovedBy = salesPolicy.FmapprovedBy;
                        //orderHead.CooapprovedBy = salesPolicy.CooapprovedBy;

                        _DBContext.Entry(orderHead).State = EntityState.Modified;

                        if (salesPolicy.orderDetail != null && salesPolicy.orderDetail.Count > 0)
                        {
                            var Cust = await _DBContext.CustomerInformation.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                            var Comp = await _DBContext.Companies.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId).FirstOrDefaultAsync();
                            //x.WarehouseId == salesPolicy.WarehouseId


                            var inventoryItems = await GetItemsByCustomerId(salesPolicy.CustomerId, token);

                            foreach (OrderDetail ordDetail in salesPolicy.orderDetail)
                            {
                                var orderDetailObj = await _DBContext.OrderDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == ordDetail.OrderNumber &&
                                                        x.ItemId == ordDetail.ItemId).FirstOrDefaultAsync();

                                var item = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == ordDetail.ItemId).FirstOrDefaultAsync();
                                // x.ItemDefaultWarehouse == ordDetail.WarehouseId


                                if (item != null)
                                {
                                    var orderWarehouseBin = await _DBContext.WarehouseBins.Where(x => x.CompanyId == token.CompanyId &&
                                                                                              x.DepartmentId == token.DepartmentId &&
                                                                                              x.DivisionId == token.DivisionId &&
                                                                                              x.WarehouseId == ordDetail.WarehouseId &&
                                                                                              x.WarehouseBinId == ordDetail.WarehouseBinId).FirstOrDefaultAsync();

                                    string defaultWarehouseBin = "";

                                    string defaultWarehouse = "";

                                    if (!String.IsNullOrEmpty(Cust.WarehouseId))
                                    {
                                        defaultWarehouse = Cust.WarehouseId;

                                    }
                                    else if (!String.IsNullOrEmpty(item.ItemDefaultWarehouse))
                                    {
                                        defaultWarehouse = item.ItemDefaultWarehouse;
                                    }
                                    else if (!String.IsNullOrEmpty(Comp.WarehouseId))
                                    {
                                        defaultWarehouse = Comp.WarehouseId;

                                    }

                                    var orderWarehouse = await _DBContext.Warehouses.Where(x => x.CompanyId == token.CompanyId &&
                                                x.DepartmentId == token.DepartmentId &&
                                                x.DivisionId == token.DivisionId &&
                                                x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                    if (orderWarehouse != null)
                                    {
                                        var itemWarehouse = await _DBContext.InventoryItems.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.ItemId == ordDetail.ItemId &&
                                                     x.ItemDefaultWarehouse == defaultWarehouse).FirstOrDefaultAsync();

                                        if (itemWarehouse == null)
                                        {
                                            var Comps = await _DBContext.Companies.Where(x =>
                                                    x.CompanyId == token.CompanyId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                            if (Comps != null)
                                            {
                                                defaultWarehouseBin = Comps.WarehouseBinId;
                                            }
                                        }
                                        else if (itemWarehouse != null)
                                        {
                                            defaultWarehouseBin = itemWarehouse.ItemDefaultWarehouseBin;
                                        }
                                    }

                                    if (orderDetailObj != null)
                                    {
                                        orderDetailObj.ItemId = ordDetail.ItemId;
                                        orderDetailObj.ItemUpccode = ordDetail.ItemUpccode;
                                        orderDetailObj.WarehouseBinId = defaultWarehouseBin;
                                        orderDetailObj.WarehouseId = defaultWarehouse;
                                        //orderDetailObj.SerialNumber = ordDetail.SerialNumber;
                                        orderDetailObj.Description = item.ItemName;
                                        orderDetailObj.OrderQty = ordDetail.OrderQty;
                                        orderDetailObj.BackOrdered = false;
                                        orderDetailObj.BackOrderQyyty = ordDetail.BackOrderQyyty;
                                        orderDetailObj.ItemUom = ordDetail.ItemUom;
                                        orderDetailObj.ItemWeight = ordDetail.ItemWeight;
                                        orderDetailObj.DiscountPerc = ordDetail.DiscountPerc;
                                        orderDetailObj.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable); ;
                                        orderDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                        orderDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                        orderDetailObj.ItemCost = ordDetail.ItemCost;
                                        orderDetailObj.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == ordDetail.ItemId).FirstOrDefault().Price; ;
                                        orderDetailObj.TaxGroupId = ordDetail.TaxGroupId;
                                        orderDetailObj.TaxAmount = ordDetail.TaxAmount;
                                        orderDetailObj.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        orderDetailObj.SubTotal = (decimal)tot;
                                        orderDetailObj.Total = (decimal)GrandTotal;
                                        orderDetailObj.TotalWeight = ordDetail.TotalWeight;
                                        orderDetailObj.GlsalesAccount = ordDetail.GlsalesAccount;
                                        orderDetailObj.Glcogaccount = ordDetail.Glcogaccount;
                                        orderDetailObj.ProjectId = ordDetail.ProjectId;
                                        orderDetailObj.TrackingNumber = ordDetail.TrackingNumber;
                                        orderDetailObj.WarehouseBinZone = ordDetail.WarehouseBinZone;
                                        orderDetailObj.PalletLevel = ordDetail.PalletLevel;
                                        orderDetailObj.CartonLevel = ordDetail.CartonLevel;
                                        orderDetailObj.PackLevelA = ordDetail.PackLevelA;
                                        orderDetailObj.PackLevelB = ordDetail.PackLevelB;
                                        orderDetailObj.PackLevelC = ordDetail.PackLevelC;
                                        orderDetailObj.ScheduledStartDate = ordDetail.ScheduledStartDate;
                                        orderDetailObj.ScheduledEndDate = ordDetail.ScheduledEndDate;
                                        orderDetailObj.ServiceStartDate = ordDetail.ServiceStartDate;
                                        orderDetailObj.ServiceEndDate = ordDetail.ServiceEndDate;
                                        orderDetailObj.PerformedBy = ordDetail.PerformedBy;
                                        orderDetailObj.DetailMemo1 = ordDetail.DetailMemo1;
                                        orderDetailObj.DetailMemo2 = ordDetail.DetailMemo2;
                                        orderDetailObj.DetailMemo3 = ordDetail.DetailMemo3;
                                        orderDetailObj.DetailMemo4 = ordDetail.DetailMemo4;
                                        orderDetailObj.DetailMemo5 = ordDetail.ItemId;
                                        orderDetailObj.DetailMemo5 = ordDetail.DetailMemo5;
                                        orderDetailObj.LockedBy = ordDetail.LockedBy;
                                        orderDetailObj.LockTs = ordDetail.LockTs;
                                        orderDetailObj.Invoiced = false;
                                        orderDetailObj.InvoicedDate = ordDetail.InvoicedDate;
                                        orderDetailObj.InvoicedQty = ordDetail.OrderQty;
                                        orderDetailObj.DeliveryNumber = ordDetail.DeliveryNumber;
                                        orderDetailObj.GlanalysisType1 = ordDetail.GlanalysisType1;
                                        orderDetailObj.GlanalysisType2 = ordDetail.GlanalysisType2;
                                        orderDetailObj.AssetId = ordDetail.AssetId;
                                        orderDetailObj.MultipleDiscountAmount = ordDetail.MultipleDiscountAmount;
                                        orderDetailObj.MultipleDiscountGroupId = ordDetail.MultipleDiscountGroupId;
                                        orderDetailObj.MultipleDiscountPercent = ordDetail.MultipleDiscountPercent;
                                        orderDetailObj.DiscountAmount = ordDetail.DiscountAmount;
                                        orderDetailObj.MarkUponCost = ordDetail.MarkUponCost;
                                        orderDetailObj.MarkUpRate = ordDetail.MarkUpRate;
                                        orderDetailObj.ItemUnitCost = ordDetail.ItemUnitCost;
                                        orderDetailObj.BranchCode = ordDetail.BranchCode;
                                        orderDetailObj.ProductTypeId = ordDetail.ProductTypeId;
                                        orderDetailObj.AdvertTypeId = ordDetail.ItemId;
                                        orderDetailObj.BackOrderBooked = false;
                                        orderDetailObj.BackOrderBookedBy = ordDetail.BackOrderBookedBy;
                                        orderDetailObj.BackOrderBookedDate = ordDetail.BackOrderBookedDate;

                                        _DBContext.Entry(orderDetailObj).State = EntityState.Modified;
                                        _DBContext.SaveChanges();
                                    }
                                    else
                                    {
                                        ordDetail.CompanyId = token.CompanyId;
                                        ordDetail.DivisionId = token.DivisionId;
                                        ordDetail.DepartmentId = token.DepartmentId;
                                        ordDetail.WarehouseId = defaultWarehouse;
                                        ordDetail.WarehouseBinId = defaultWarehouseBin;
                                        ordDetail.Description = item.ItemName;
                                        ordDetail.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);
                                        ordDetail.TaxGroupId = item.TaxGroupId;
                                        ordDetail.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        ordDetail.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == ordDetail.ItemId).FirstOrDefault().Price;
                                        ordDetail.SubTotal = (decimal)tot;
                                        ordDetail.Total = (decimal)GrandTotal;
                                        ordDetail.Invoiced = false;
                                        ordDetail.BackOrderBooked = false;
                                        ordDetail.BackOrdered = false;

                                        _DBContext.Entry(ordDetail).State = EntityState.Added;
                                        _DBContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Invalid Item ID";
                                }
                            }
                        }
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create sales policy
                    {
                        OrderHeader orderHeader = new OrderHeader();
                        _NextNumberName = "NextOrderNumber";
                        string orderNumber = await getNextEntityID(_NextNumberName, token);
                        string BankId = "CASH";
                        string HeaderMemo12 = "Sales Mobile Order";

                        orderHeader.CompanyId = token.CompanyId;
                        orderHeader.DivisionId = token.DivisionId;
                        orderHeader.DepartmentId = token.DepartmentId;
                        orderHeader.OrderNumber = orderNumber;
                        orderHeader.TransactionTypeId = "Order";
                        orderHeader.OrderTypeId = "Order";
                        orderHeader.OrderDate = DateTime.Now;
                        orderHeader.OrderDueDate = salesPolicy.OrderDueDate;
                        orderHeader.OrderShipDate = salesPolicy.OrderShipDate;
                        orderHeader.OrderCancelDate = salesPolicy.OrderCancelDate;
                        orderHeader.SystemDate = DateTime.Now;
                        orderHeader.Memorize = false;
                        orderHeader.PurchaseOrderNumber = salesPolicy.PurchaseOrderNumber;
                        orderHeader.TaxExemptId = salesPolicy.TaxExemptId;
                        orderHeader.TaxGroupId = salesPolicy.TaxGroupId;
                        orderHeader.CustomerId = salesPolicy.CustomerId;
                        orderHeader.TermsId = salesPolicy.TermsId;
                        orderHeader.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        orderHeader.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        orderHeader.Subtotal = (decimal)tot;
                        orderHeader.DiscountPers = salesPolicy.DiscountPers;
                        orderHeader.DiscountAmount = salesPolicy.DiscountAmount;
                        orderHeader.TaxPercent = salesPolicy.TaxPercent;
                        orderHeader.TaxAmount = salesPolicy.TaxAmount;
                        orderHeader.TaxableSubTotal = salesPolicy.TaxableSubTotal;
                        orderHeader.Freight = salesPolicy.Freight;
                        orderHeader.TaxFreight = false;
                        orderHeader.Handling = salesPolicy.Handling;
                        orderHeader.Advertising = salesPolicy.Advertising;
                        orderHeader.Total = (decimal)GrandTotal;
                        orderHeader.EmployeeId = salesPolicy.EmployeeId;
                        orderHeader.Commission = salesPolicy.Commission;
                        orderHeader.CommissionableSales = salesPolicy.CommissionableSales;
                        orderHeader.ComissionalbleCost = salesPolicy.ComissionalbleCost;
                        orderHeader.CustomerDropShipment = false;
                        orderHeader.ShipMethodId = salesPolicy.ShipMethodId;
                        orderHeader.WarehouseId = salesPolicy.WarehouseId;
                        orderHeader.ShipForId = salesPolicy.ShipForId;
                        orderHeader.ShipToId = salesPolicy.ShipToId;
                        orderHeader.ShippingName = customer.CustomerName;
                        orderHeader.ShippingAddress1 = customer.CustomerAddress1;
                        orderHeader.ShippingAddress2 = customer.CustomerAddress2;
                        orderHeader.ShippingAddress3 = customer.CustomerAddress3;
                        orderHeader.ShippingCity = customer.CustomerCity;
                        orderHeader.ShippingState = customer.CustomerState;
                        orderHeader.ShippingZip = salesPolicy.ShippingZip;
                        orderHeader.ShippingCountry = customer.CustomerCountry;
                        //orderHeader.ScheduledStartDate = salesPolicy.ScheduledStartDate;
                        //orderHeader.ScheduledEndDate = salesPolicy.ScheduledEndDate;
                        orderHeader.ServiceStartDate = salesPolicy.ServiceStartDate;
                        orderHeader.ServiceEndDate = salesPolicy.ServiceEndDate;
                        orderHeader.PerformedBy = salesPolicy.PerformedBy;
                        orderHeader.GlsalesAccount = salesPolicy.GlsalesAccount;
                        orderHeader.Glcogaccount = salesPolicy.Glcogaccount;
                        orderHeader.PaymentMethodId = salesPolicy.PaymentMethodId;
                        orderHeader.AmountPaid = salesPolicy.AmountPaid;
                        orderHeader.BalanceDue = salesPolicy.BalanceDue;
                        orderHeader.UndistributedAmount = salesPolicy.UndistributedAmount;
                        orderHeader.CheckNumber = salesPolicy.CheckNumber;
                        orderHeader.CheckDate = salesPolicy.CheckDate;
                        orderHeader.CreditCardTypeId = salesPolicy.CreditCardTypeId;
                        orderHeader.CreditCardName = salesPolicy.CreditCardName;
                        orderHeader.CreditCardNumber = salesPolicy.CreditCardNumber;
                        orderHeader.CreditCardCsvnumber = salesPolicy.CreditCardCsvnumber;
                        orderHeader.CreditCardExpDate = salesPolicy.CreditCardExpDate;
                        orderHeader.CreditCardBillToZip = salesPolicy.CreditCardBillToZip;
                        orderHeader.CreditCardValidationCode = salesPolicy.CreditCardValidationCode;
                        orderHeader.Backordered = false;
                        orderHeader.Picked = false;
                        orderHeader.PickedDate = salesPolicy.PickedDate;
                        orderHeader.Printed = false;
                        orderHeader.PrintedDate = salesPolicy.PrintedDate;
                        orderHeader.Shipped = false;
                        orderHeader.ShipDate = salesPolicy.ShipDate;
                        orderHeader.TrackingNumber = salesPolicy.TrackingNumber;
                        orderHeader.Billed = false;
                        orderHeader.Invoiced = false;
                        orderHeader.InvoiceDate = salesPolicy.InvoiceDate;
                        orderHeader.InvoiceNumber = salesPolicy.InvoiceNumber;
                        orderHeader.Posted = false;
                        orderHeader.PostedDate = salesPolicy.PostedDate;
                        orderHeader.AllowanceDiscountPerc = salesPolicy.AllowanceDiscountPerc;
                        orderHeader.CashTendered = salesPolicy.CashTendered;
                        orderHeader.MasterBillOfLading = salesPolicy.MasterBillOfLading;
                        orderHeader.MasterBillOfLadingDate = salesPolicy.MasterBillOfLadingDate;
                        orderHeader.TrailerNumber = salesPolicy.TrailerNumber;
                        orderHeader.TrailerPrefix = salesPolicy.TrailerPrefix;
                        orderHeader.HeaderMemo1 = salesPolicy.HeaderMemo1;
                        orderHeader.HeaderMemo2 = salesPolicy.HeaderMemo2;
                        orderHeader.HeaderMemo3 = "Mobile Sales";
                        orderHeader.HeaderMemo4 = salesPolicy.HeaderMemo4;
                        orderHeader.HeaderMemo5 = salesPolicy.HeaderMemo5;
                        orderHeader.HeaderMemo6 = salesPolicy.HeaderMemo6;
                        orderHeader.HeaderMemo7 = salesPolicy.HeaderMemo7;
                        orderHeader.HeaderMemo8 = salesPolicy.HeaderMemo8;
                        orderHeader.HeaderMemo9 = salesPolicy.HeaderMemo9;
                        orderHeader.HeaderMemo10 = salesPolicy.HeaderMemo10;
                        orderHeader.HeaderMemo11 = salesPolicy.HeaderMemo11;
                        orderHeader.HeaderMemo12 = HeaderMemo12;
                        orderHeader.Approved = false;
                        orderHeader.ApprovedBy = salesPolicy.ApprovedBy;
                        orderHeader.ApprovedDate = salesPolicy.ApprovedDate;
                        orderHeader.Signature = salesPolicy.Signature;
                        orderHeader.SignaturePassword = salesPolicy.SignaturePassword;
                        orderHeader.SupervisorPassword = salesPolicy.SupervisorPassword;
                        orderHeader.SupervisorSignature = salesPolicy.SupervisorSignature;
                        orderHeader.ManagerSignature = salesPolicy.ManagerSignature;
                        orderHeader.ManagerPassword = salesPolicy.ManagerPassword;
                        orderHeader.LockedBy = salesPolicy.LockedBy;
                        orderHeader.LockTs = salesPolicy.LockTs;
                        orderHeader.BankId = BankId;
                        orderHeader.OriginalOrderNumber = salesPolicy.OriginalOrderNumber;
                        orderHeader.OriginalOrderDate = salesPolicy.OriginalOrderDate;
                        orderHeader.DeliveryNumber = salesPolicy.DeliveryNumber;
                        //orderHeader.Ullage1 = salesPolicy.Ullage1;
                        //orderHeader.Ullage2 = salesPolicy.Ullage2;
                        //orderHeader.Ullage3 = salesPolicy.Ullage3;
                        //orderHeader.Ullage4 = salesPolicy.Ullage4;
                        //orderHeader.Ullage5 = salesPolicy.Ullage5;
                        //orderHeader.Ullage6 = salesPolicy.Ullage6;
                        //orderHeader.Ullage7 = salesPolicy.Ullage7;
                        //orderHeader.Ullage8 = salesPolicy.Ullage8;
                        //orderHeader.Ullage9 = salesPolicy.Ullage9;
                        //orderHeader.Ullage10 = salesPolicy.Ullage10;
                        //orderHeader.Ullage11 = salesPolicy.Ullage11;
                        //orderHeader.Ullage12 = salesPolicy.Ullage12;
                        orderHeader.BranchCode = salesPolicy.BranchCode;
                        orderHeader.Merged = false; // salesPolicy.Merged;
                        orderHeader.Created = false;// salesPolicy.Created;
                        orderHeader.FinanceApproved = false; //salesPolicy.FinanceApproved;
                        //orderHeader.FinanceApprovedDate = salesPolicy.FinanceApprovedDate;
                        //orderHeader.FinanceComment = salesPolicy.FinanceComment;
                        //orderHeader.FinanceReturnedDate = salesPolicy.FinanceReturnedDate;
                        //orderHeader.Bdmapproved = false; // salesPolicy.Bdmapproved;
                        //orderHeader.BdmapprovedDate = salesPolicy.BdmapprovedDate;
                        //orderHeader.Bdmcomment = salesPolicy.Bdmcomment;
                        //orderHeader.Fmapproved = false; // salesPolicy.Fmapproved;
                        //orderHeader.FmapprovedDate = salesPolicy.FmapprovedDate;
                        //orderHeader.Fmcomment = salesPolicy.Fmcomment;
                        //orderHeader.Mdapproved = false; // salesPolicy.Mdapproved;
                        //orderHeader.MdapprovedDate = salesPolicy.MdapprovedDate;
                        //orderHeader.Mdcomment = salesPolicy.Mdcomment;
                        //orderHeader.Regularized = false;
                        //orderHeader.Fmvoid = false;
                        //orderHeader.FmvoidedDate = salesPolicy.FmvoidedDate;
                        //orderHeader.ReceiptId = salesPolicy.ReceiptId;
                        //orderHeader.CommercialComment = salesPolicy.CommercialComment;
                        //orderHeader.FinanceApprovedBy = salesPolicy.FinanceApprovedBy;
                        //orderHeader.FmapprovedBy = salesPolicy.FmapprovedBy;
                        //orderHeader.CooapprovedBy = salesPolicy.CooapprovedBy;

                        _DBContext.Entry(orderHeader).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (salesPolicy.orderDetail != null && salesPolicy.orderDetail.Count > 0)
                        {
                            var Cust = await _DBContext.CustomerInformation.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                            var Comp = await _DBContext.Companies.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId).FirstOrDefaultAsync();
                            //x.WarehouseId == salesPolicy.WarehouseId


                            var inventoryItems = await GetItemsByCustomerId(salesPolicy.CustomerId, token);

                            foreach (OrderDetail ordDetail in salesPolicy.orderDetail)
                            {
                                var orderDetailObj = await _DBContext.OrderDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == ordDetail.OrderNumber &&
                                                        x.ItemId == ordDetail.ItemId).FirstOrDefaultAsync();

                                var item = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == ordDetail.ItemId).FirstOrDefaultAsync();
                                // x.ItemDefaultWarehouse == ordDetail.WarehouseId


                                if (item != null)
                                {
                                    var orderWarehouseBin = await _DBContext.WarehouseBins.Where(x => x.CompanyId == token.CompanyId &&
                                                                                              x.DepartmentId == token.DepartmentId &&
                                                                                              x.DivisionId == token.DivisionId &&
                                                                                              x.WarehouseId == ordDetail.WarehouseId &&
                                                                                              x.WarehouseBinId == ordDetail.WarehouseBinId).FirstOrDefaultAsync();

                                    string defaultWarehouseBin = "";

                                    string defaultWarehouse = "";

                                    if (!String.IsNullOrEmpty(Cust.WarehouseId))
                                    {
                                        defaultWarehouse = Cust.WarehouseId;

                                    }
                                    else if (!String.IsNullOrEmpty(item.ItemDefaultWarehouse))
                                    {
                                        defaultWarehouse = item.ItemDefaultWarehouse;
                                    }
                                    else if (!String.IsNullOrEmpty(Comp.WarehouseId))
                                    {
                                        defaultWarehouse = Comp.WarehouseId;

                                    }

                                    var orderWarehouse = await _DBContext.Warehouses.Where(x => x.CompanyId == token.CompanyId &&
                                                x.DepartmentId == token.DepartmentId &&
                                                x.DivisionId == token.DivisionId &&
                                                x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                    if (orderWarehouse != null)
                                    {
                                        var itemWarehouse = await _DBContext.InventoryItems.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.ItemId == ordDetail.ItemId &&
                                                     x.ItemDefaultWarehouse == defaultWarehouse).FirstOrDefaultAsync();

                                        if (itemWarehouse == null)
                                        {
                                            var Comps = await _DBContext.Companies.Where(x =>
                                                    x.CompanyId == token.CompanyId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                            if (Comps != null)
                                            {
                                                defaultWarehouseBin = Comps.WarehouseBinId;
                                            }
                                        }
                                        else if (itemWarehouse != null)
                                        {
                                            defaultWarehouseBin = itemWarehouse.ItemDefaultWarehouseBin;
                                        }
                                    }

                                    if (orderDetailObj != null)
                                    {
                                        orderDetailObj.ItemId = ordDetail.ItemId;
                                        orderDetailObj.ItemUpccode = ordDetail.ItemUpccode;
                                        orderDetailObj.WarehouseBinId = defaultWarehouseBin;
                                        orderDetailObj.WarehouseId = defaultWarehouse;
                                        //orderDetailObj.SerialNumber = ordDetail.SerialNumber;
                                        orderDetailObj.Description = item.ItemName;
                                        orderDetailObj.OrderQty = ordDetail.OrderQty;
                                        orderDetailObj.BackOrdered = false;
                                        orderDetailObj.BackOrderQyyty = ordDetail.BackOrderQyyty;
                                        orderDetailObj.ItemUom = ordDetail.ItemUom;
                                        orderDetailObj.ItemWeight = ordDetail.ItemWeight;
                                        orderDetailObj.DiscountPerc = ordDetail.DiscountPerc;
                                        orderDetailObj.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable); ;
                                        orderDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                        orderDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                        orderDetailObj.ItemCost = ordDetail.ItemCost;
                                        orderDetailObj.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == ordDetail.ItemId).FirstOrDefault().Price; ;
                                        orderDetailObj.TaxGroupId = ordDetail.TaxGroupId;
                                        orderDetailObj.TaxAmount = ordDetail.TaxAmount;
                                        orderDetailObj.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        orderDetailObj.SubTotal = (decimal)tot;
                                        orderDetailObj.Total = (decimal)GrandTotal;
                                        orderDetailObj.TotalWeight = ordDetail.TotalWeight;
                                        orderDetailObj.GlsalesAccount = ordDetail.GlsalesAccount;
                                        orderDetailObj.Glcogaccount = ordDetail.Glcogaccount;
                                        orderDetailObj.ProjectId = ordDetail.ProjectId;
                                        orderDetailObj.TrackingNumber = ordDetail.TrackingNumber;
                                        orderDetailObj.WarehouseBinZone = ordDetail.WarehouseBinZone;
                                        orderDetailObj.PalletLevel = ordDetail.PalletLevel;
                                        orderDetailObj.CartonLevel = ordDetail.CartonLevel;
                                        orderDetailObj.PackLevelA = ordDetail.PackLevelA;
                                        orderDetailObj.PackLevelB = ordDetail.PackLevelB;
                                        orderDetailObj.PackLevelC = ordDetail.PackLevelC;
                                        orderDetailObj.ScheduledStartDate = ordDetail.ScheduledStartDate;
                                        orderDetailObj.ScheduledEndDate = ordDetail.ScheduledEndDate;
                                        orderDetailObj.ServiceStartDate = ordDetail.ServiceStartDate;
                                        orderDetailObj.ServiceEndDate = ordDetail.ServiceEndDate;
                                        orderDetailObj.PerformedBy = ordDetail.PerformedBy;
                                        orderDetailObj.DetailMemo1 = ordDetail.DetailMemo1;
                                        orderDetailObj.DetailMemo2 = ordDetail.DetailMemo2;
                                        orderDetailObj.DetailMemo3 = ordDetail.DetailMemo3;
                                        orderDetailObj.DetailMemo4 = ordDetail.DetailMemo4;
                                        orderDetailObj.DetailMemo5 = ordDetail.ItemId;
                                        orderDetailObj.DetailMemo5 = ordDetail.DetailMemo5;
                                        orderDetailObj.LockedBy = ordDetail.LockedBy;
                                        orderDetailObj.LockTs = ordDetail.LockTs;
                                        orderDetailObj.Invoiced = false;
                                        orderDetailObj.InvoicedDate = ordDetail.InvoicedDate;
                                        orderDetailObj.InvoicedQty = ordDetail.OrderQty;
                                        orderDetailObj.DeliveryNumber = ordDetail.DeliveryNumber;
                                        orderDetailObj.GlanalysisType1 = ordDetail.GlanalysisType1;
                                        orderDetailObj.GlanalysisType2 = ordDetail.GlanalysisType2;
                                        orderDetailObj.AssetId = ordDetail.AssetId;
                                        orderDetailObj.MultipleDiscountAmount = ordDetail.MultipleDiscountAmount;
                                        orderDetailObj.MultipleDiscountGroupId = ordDetail.MultipleDiscountGroupId;
                                        orderDetailObj.MultipleDiscountPercent = ordDetail.MultipleDiscountPercent;
                                        orderDetailObj.DiscountAmount = ordDetail.DiscountAmount;
                                        orderDetailObj.MarkUponCost = ordDetail.MarkUponCost;
                                        orderDetailObj.MarkUpRate = ordDetail.MarkUpRate;
                                        orderDetailObj.ItemUnitCost = ordDetail.ItemUnitCost;
                                        orderDetailObj.BranchCode = ordDetail.BranchCode;
                                        orderDetailObj.ProductTypeId = ordDetail.ProductTypeId;
                                        orderDetailObj.AdvertTypeId = ordDetail.ItemId;
                                        orderDetailObj.BackOrderBooked = false;
                                        orderDetailObj.BackOrderBookedBy = ordDetail.BackOrderBookedBy;
                                        orderDetailObj.BackOrderBookedDate = ordDetail.BackOrderBookedDate;

                                        _DBContext.Entry(orderDetailObj).State = EntityState.Modified;
                                        _DBContext.SaveChanges();
                                    }
                                    else
                                    {
                                        ordDetail.CompanyId = token.CompanyId;
                                        ordDetail.DivisionId = token.DivisionId;
                                        ordDetail.DepartmentId = token.DepartmentId;
                                        ordDetail.OrderNumber = orderNumber;
                                        ordDetail.OrderLineNumber = 0;
                                        ordDetail.WarehouseId = defaultWarehouse;
                                        ordDetail.WarehouseBinId = defaultWarehouseBin;
                                        ordDetail.Description = item.ItemName;
                                        ordDetail.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);
                                        ordDetail.TaxGroupId = item.TaxGroupId;
                                        ordDetail.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        ordDetail.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == ordDetail.ItemId).FirstOrDefault().Price;
                                        ordDetail.SubTotal = (decimal)tot;
                                        ordDetail.Total = (decimal)GrandTotal;
                                        ordDetail.Invoiced = false;
                                        ordDetail.BackOrderBooked = false;
                                        ordDetail.BackOrdered = false;

                                        _DBContext.Entry(ordDetail).State = EntityState.Added;
                                        _DBContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Invalid Item ID";
                                }
                            }
                        }

                        //call order booking
                        await OrderBooking(orderNumber, token);

                        statusMessage.Status = "Success";
                        statusMessage.Message = orderNumber;
                        statusMessage.data = await GetOrderById(orderNumber, token);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Order Information";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<string> getNextEntityID(string nextNumberName, ApiToken token)
        {
            string sEntityID = "";
            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sNextNumberName = new SqlParameter("@Entity", nextNumberName);
                var EntityID = new SqlParameter("@EntityID", SqlDbType.NVarChar, 255);
                EntityID.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.GetNextEntityID @CompanyID, @DivisionID, @DepartmentID, @Entity, @EntityID Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sNextNumberName, EntityID });

                sEntityID = EntityID.Value.ToString();
            }
            catch (Exception ex)
            {

            }
            return sEntityID;
        }

        public async Task<bool> order_Post(string OrderNumber, ApiToken token)
        {
            bool status = false;

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sOrderNumber = new SqlParameter("@OrderNumber", OrderNumber);
                var sEmployeeID = new SqlParameter("@EmployeeID", token.Username);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.Order_Post @CompanyID, @DivisionID, @DepartmentID, @OrderNumber, @EmployeeID , @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sOrderNumber, sEmployeeID, PostingResult });

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<StatusMessage> OrderBooking(string Id, ApiToken tokenObj)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                bool status = await order_Post(Id, tokenObj);

                if (status)
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Order Successfully Booked";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Order Booking Failed.Try again or contact system administrator";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        private async Task<List<Receipt>> receipts(List<ReceiptsHeader> receiptsHeader, ApiToken token)
        {
            List<Receipt> receipts = new List<Receipt>();
            try
            {
                if (receiptsHeader != null)
                {
                    foreach (ReceiptsHeader receiptHeader in receiptsHeader)
                    {
                        Receipt receiptObj = new Receipt();

                        receiptObj.CompanyId = receiptHeader.CompanyId;
                        receiptObj.DivisionId = receiptHeader.DivisionId;
                        receiptObj.DepartmentId = receiptHeader.DepartmentId;
                        receiptObj.ReceiptId = receiptHeader.ReceiptId;
                        receiptObj.ReceiptTypeId = receiptHeader.ReceiptTypeId;
                        receiptObj.ReceiptClassId = receiptHeader.ReceiptClassId;
                        receiptObj.CheckNumber = receiptHeader.CheckNumber;
                        receiptObj.CustomerId = receiptHeader.CustomerId;
                        receiptObj.Memorize = receiptHeader.Memorize;
                        receiptObj.TransactionDate = receiptHeader.TransactionDate;
                        receiptObj.SystemDate = receiptHeader.SystemDate;
                        receiptObj.DueToDate = receiptHeader.DueToDate;
                        receiptObj.OrderDate = receiptHeader.OrderDate;
                        receiptObj.CurrencyId = receiptHeader.CurrencyId;
                        receiptObj.CurrencyExchangeRate = receiptHeader.CurrencyExchangeRate;
                        receiptObj.Amount = receiptHeader.Amount;
                        receiptObj.UnAppliedAmount = receiptHeader.UnAppliedAmount;
                        receiptObj.GlbankAccount = receiptHeader.GlbankAccount;
                        receiptObj.BankId = receiptHeader.BankId;
                        receiptObj.Status = receiptHeader.Status;
                        receiptObj.Nsf = receiptHeader.Nsf;
                        receiptObj.Notes = receiptHeader.Notes;
                        receiptObj.CreditAmount = receiptHeader.CreditAmount;
                        receiptObj.Cleared = receiptHeader.Cleared;
                        receiptObj.Posted = receiptHeader.Posted;
                        receiptObj.Reconciled = receiptHeader.Reconciled;
                        receiptObj.Deposited = receiptHeader.Deposited;
                        receiptObj.HeaderMemo1 = receiptHeader.HeaderMemo1;
                        receiptObj.HeaderMemo2 = receiptHeader.HeaderMemo2;
                        receiptObj.HeaderMemo3 = receiptHeader.HeaderMemo3;
                        receiptObj.HeaderMemo4 = receiptHeader.HeaderMemo4;
                        receiptObj.HeaderMemo5 = receiptHeader.HeaderMemo5;
                        receiptObj.HeaderMemo6 = receiptHeader.HeaderMemo6;
                        receiptObj.HeaderMemo7 = receiptHeader.HeaderMemo7;
                        receiptObj.HeaderMemo8 = receiptHeader.HeaderMemo8;
                        receiptObj.HeaderMemo9 = receiptHeader.HeaderMemo9;
                        receiptObj.Approved = receiptHeader.Approved;
                        receiptObj.ApprovedBy = receiptHeader.ApprovedBy;
                        receiptObj.ApprovedDate = receiptHeader.ApprovedDate;
                        receiptObj.EnteredBy = receiptHeader.EnteredBy;
                        receiptObj.BatchControlNumber = receiptHeader.BatchControlNumber;
                        receiptObj.BatchControlTotal = receiptHeader.BatchControlTotal;
                        receiptObj.Signature = receiptHeader.Signature;
                        receiptObj.SignaturePassword = receiptHeader.SignaturePassword;
                        receiptObj.SupervisorSignature = receiptHeader.SupervisorSignature;
                        receiptObj.SupervisorPassword = receiptHeader.SupervisorPassword;
                        receiptObj.ManagerSignature = receiptHeader.ManagerSignature;
                        receiptObj.ManagerPassword = receiptHeader.ManagerPassword;
                        receiptObj.LockedBy = receiptHeader.LockedBy;
                        receiptObj.LockTs = receiptHeader.LockTs;
                        receiptObj.TaxGroupId = receiptHeader.TaxGroupId;
                        receiptObj.TaxAmount = receiptHeader.TaxAmount;
                        receiptObj.BranchCode = receiptHeader.BranchCode;
                        receiptObj.CustomerName = receiptHeader.CustomerName;
                        receiptObj.MarkedForAllocation = receiptHeader.MarkedForAllocation;
                        receiptObj.Allocated = receiptHeader.Allocated;

                        receiptObj.receiptsDetails = await _DBContext.ReceiptsDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.ReceiptId == receiptHeader.ReceiptId).ToListAsync();

                        receipts.Add(receiptObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return receipts;

        }
        public async Task<StatusMessage> AddReceiptPost(Receipt receipt, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<ReceiptsDetail> receiptsDetail = new List<ReceiptsDetail>();
            Data.Models.CurrencyTypes currency = new Data.Models.CurrencyTypes();
            Data.Models.BankAccounts defaultBank = new Data.Models.BankAccounts();
            Data.Models.BankAccounts receiptBank = new Data.Models.BankAccounts();


            try
            {
                var company = await _DBContext.Companies.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();

                var customer = await _DBContext.CustomerInformation.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.CustomerId == receipt.CustomerId).FirstOrDefaultAsync();

                if (company != null)
                {
                    currency = await _DBContext.CurrencyTypes.Where(x =>
                                                      x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.CurrencyId == company.CurrencyId).FirstOrDefaultAsync();

                    defaultBank = await _DBContext.BankAccounts.Where(x =>
                                                      x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.GlbankAccount == company.BankAccount).FirstOrDefaultAsync();

                    receiptBank = await _DBContext.BankAccounts.Where(x =>
                                                      x.CompanyId == token.CompanyId &&
                                                      x.DivisionId == token.DivisionId &&
                                                      x.DepartmentId == token.DepartmentId &&
                                                      x.BankId == receipt.BankId).FirstOrDefaultAsync();

                }

                //check if object empty
                if (receipt != null)
                {
                    var receiptHead = await _DBContext.ReceiptsHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReceiptId == receipt.ReceiptId).FirstOrDefaultAsync();


                    if (receiptHead != null) // for update payroll policy
                    {
                        receiptHead.ReceiptTypeId = receipt.ReceiptTypeId;
                        receiptHead.ReceiptTypeId = receipt.ReceiptTypeId;
                        receiptHead.ReceiptClassId = "Customer";
                        receiptHead.CheckNumber = receipt.CheckNumber;
                        receiptHead.CustomerId = receipt.CustomerId;
                        receiptHead.Memorize = false;
                        receiptHead.TransactionDate = DateTime.Now;
                        receiptHead.SystemDate = DateTime.Now;
                        receiptHead.DueToDate = receipt.DueToDate;
                        receiptHead.OrderDate = receipt.OrderDate;
                        receiptHead.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        receiptHead.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        receiptHead.Amount = receipt.Amount;
                        receiptHead.UnAppliedAmount = receipt.Amount;
                        receiptHead.GlbankAccount = receipt.GlbankAccount;
                        receiptHead.BankId = receipt.BankId;
                        receiptHead.Status = receipt.Status;
                        receiptHead.Nsf = false;
                        receiptHead.Notes = receipt.Notes;
                        receiptHead.CreditAmount = receipt.Amount;
                        receiptHead.Cleared = false;
                        receiptHead.Posted = false;
                        receiptHead.Reconciled = false;
                        receiptHead.Deposited = false;
                        receiptHead.HeaderMemo1 = receipt.HeaderMemo1;
                        receiptHead.HeaderMemo2 = receipt.HeaderMemo2;
                        receiptHead.HeaderMemo3 = receipt.HeaderMemo3;
                        receiptHead.HeaderMemo4 = receipt.HeaderMemo4;
                        receiptHead.HeaderMemo5 = receipt.HeaderMemo5;
                        receiptHead.HeaderMemo6 = receipt.HeaderMemo6;
                        receiptHead.HeaderMemo7 = receipt.HeaderMemo7;
                        receiptHead.HeaderMemo8 = receipt.HeaderMemo8;
                        receiptHead.HeaderMemo9 = receipt.HeaderMemo9;
                        receiptHead.Approved = false;
                        receiptHead.ApprovedBy = receipt.ApprovedBy;
                        receiptHead.ApprovedDate = receipt.ApprovedDate;
                        receiptHead.EnteredBy = receipt.EnteredBy;
                        receiptHead.BatchControlNumber = receipt.BatchControlNumber;
                        receiptHead.BatchControlTotal = receipt.BatchControlTotal;
                        receiptHead.Signature = receipt.Signature;
                        receiptHead.SignaturePassword = receipt.SignaturePassword;
                        receiptHead.SupervisorSignature = receipt.SupervisorSignature;
                        receiptHead.SupervisorPassword = receipt.SupervisorPassword;
                        receiptHead.ManagerSignature = receipt.ManagerSignature;
                        receiptHead.ManagerPassword = receipt.ManagerPassword;
                        receiptHead.LockedBy = receipt.LockedBy;
                        receiptHead.LockTs = receipt.LockTs;
                        receiptHead.TaxGroupId = receipt.TaxGroupId;
                        receiptHead.TaxAmount = receipt.TaxAmount;
                        receiptHead.BranchCode = receipt.BranchCode;
                        receiptHead.CustomerName = receipt.CustomerName;
                        receiptHead.MarkedForAllocation = false;
                        receiptHead.Allocated = false;

                        _DBContext.Entry(receiptHead).State = EntityState.Modified;

                        if (receipt.receiptsDetails != null && receipt.receiptsDetails.Count > 0)
                        {
                            foreach (ReceiptsDetail recDetail in receipt.receiptsDetails)
                            {
                                var receiptDetailObj = await _DBContext.ReceiptsDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReceiptId == recDetail.ReceiptId &&
                                                        x.ReceiptDetailId == recDetail.ReceiptDetailId).FirstOrDefaultAsync();

                                if (receiptDetailObj != null)
                                {
                                    receiptDetailObj.CustomerId = recDetail.CustomerId;
                                    receiptDetailObj.DocumentNumber = recDetail.DocumentNumber;
                                    receiptDetailObj.DocumentDate = recDetail.DocumentDate;
                                    receiptDetailObj.PaymentId = recDetail.PaymentId;
                                    receiptDetailObj.PayedId = recDetail.PayedId;
                                    receiptDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                    receiptDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                    receiptDetailObj.DiscountTaken = recDetail.DiscountTaken;
                                    receiptDetailObj.WriteOffAmount = recDetail.WriteOffAmount;
                                    receiptDetailObj.AppliedAmount = recDetail.AppliedAmount;
                                    receiptDetailObj.Cleared = false;
                                    receiptDetailObj.ProjectId = recDetail.ProjectId;
                                    receiptDetailObj.DetailMemo1 = recDetail.DetailMemo1;
                                    receiptDetailObj.DetailMemo2 = recDetail.DetailMemo2;
                                    receiptDetailObj.DetailMemo3 = recDetail.DetailMemo3;
                                    receiptDetailObj.DetailMemo4 = recDetail.DetailMemo4;
                                    receiptDetailObj.DetailMemo5 = recDetail.DetailMemo5;
                                    receiptDetailObj.LockedBy = null;
                                    receiptDetailObj.LockTs = null;
                                    receiptDetailObj.TaxGroupId = recDetail.TaxGroupId;
                                    receiptDetailObj.TaxAmount = recDetail.TaxAmount;
                                    receiptDetailObj.TaxRate = recDetail.TaxRate;
                                    receiptDetailObj.GlanalysisType1 = recDetail.GlanalysisType1;
                                    receiptDetailObj.GlanalysisType2 = recDetail.GlanalysisType2;
                                    receiptDetailObj.AssetId = recDetail.AssetId;
                                    receiptDetailObj.CommissionRate = recDetail.CommissionRate;
                                    receiptDetailObj.CommissionType = recDetail.CommissionType;
                                    receiptDetailObj.BranchCode = recDetail.BranchCode;
                                    receiptDetailObj.BankId = recDetail.BankId;
                                    receiptDetailObj.PaidAmount = recDetail.PaidAmount;

                                    _DBContext.Entry(receiptDetailObj).State = EntityState.Modified;
                                }
                                else
                                {
                                    recDetail.CompanyId = token.CompanyId;
                                    recDetail.DivisionId = token.DivisionId;
                                    recDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(recDetail).State = EntityState.Added;
                                }

                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        ReceiptsHeader receiptsHeader = new ReceiptsHeader();
                        _NextNumberName = "NextReceiptNumber";
                        string receiptId = await getNextEntityID(_NextNumberName, token);

                        receiptsHeader.CompanyId = token.CompanyId;
                        receiptsHeader.DivisionId = token.DivisionId;
                        receiptsHeader.DepartmentId = token.DepartmentId;
                        receiptsHeader.ReceiptId = receiptId;
                        receiptsHeader.ReceiptTypeId = receipt.ReceiptTypeId;
                        receiptsHeader.ReceiptClassId = "Customer";
                        receiptsHeader.CheckNumber = receipt.CheckNumber == null || receipt.CheckNumber == "" ? "NA" : receipt.CheckNumber;
                        receiptsHeader.CustomerId = receipt.CustomerId;
                        receiptsHeader.Memorize = false;
                        receiptsHeader.TransactionDate = DateTime.Now;
                        receiptsHeader.SystemDate = DateTime.Now;
                        receiptsHeader.DueToDate = receipt.DueToDate;
                        receiptsHeader.OrderDate = receipt.OrderDate;
                        receiptsHeader.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        receiptsHeader.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        receiptsHeader.Amount = receipt.Amount;
                        receiptsHeader.UnAppliedAmount = receipt.Amount;
                        receiptsHeader.GlbankAccount = receiptBank != null ? receiptBank.GlbankAccount : company.BankAccount;
                        receiptsHeader.BankId = receiptBank != null ? receiptBank.BankId : defaultBank.BankId;
                        receiptsHeader.Status = receipt.Status;
                        receiptsHeader.Nsf = false;
                        receiptsHeader.Notes = receipt.Notes == null || receipt.Notes == "" ? "Payment Made Online" : receipt.Notes;
                        receiptsHeader.CreditAmount = receipt.Amount;
                        receiptsHeader.Cleared = false;
                        receiptsHeader.Posted = false;
                        receiptsHeader.Reconciled = false;
                        receiptsHeader.Deposited = false;
                        receiptsHeader.HeaderMemo1 = receipt.HeaderMemo1;
                        receiptsHeader.HeaderMemo2 = receipt.HeaderMemo2;
                        receiptsHeader.HeaderMemo3 = "Mobile Sales";
                        receiptsHeader.HeaderMemo4 = receipt.HeaderMemo4;
                        receiptsHeader.HeaderMemo5 = receipt.HeaderMemo5;
                        receiptsHeader.HeaderMemo6 = receipt.HeaderMemo6;
                        receiptsHeader.HeaderMemo7 = receipt.HeaderMemo7;
                        receiptsHeader.HeaderMemo8 = receipt.HeaderMemo8;
                        receiptsHeader.HeaderMemo9 = receipt.HeaderMemo9;
                        receiptsHeader.Approved = false;
                        receiptsHeader.ApprovedBy = receipt.ApprovedBy;
                        receiptsHeader.ApprovedDate = receipt.ApprovedDate;
                        receiptsHeader.EnteredBy = receipt.EnteredBy;
                        receiptsHeader.BatchControlNumber = receipt.BatchControlNumber;
                        receiptsHeader.BatchControlTotal = receipt.BatchControlTotal;
                        receiptsHeader.Signature = receipt.Signature;
                        receiptsHeader.SignaturePassword = receipt.SignaturePassword;
                        receiptsHeader.SupervisorSignature = receipt.SupervisorSignature;
                        receiptsHeader.SupervisorPassword = receipt.SupervisorPassword;
                        receiptsHeader.ManagerSignature = receipt.ManagerSignature;
                        receiptsHeader.ManagerPassword = receipt.ManagerPassword;
                        receiptsHeader.LockedBy = null;
                        receiptsHeader.LockTs = null;
                        receiptsHeader.TaxGroupId = receipt.TaxGroupId;
                        receiptsHeader.TaxAmount = receipt.TaxAmount;
                        receiptsHeader.BranchCode = receipt.BranchCode;
                        receiptsHeader.CustomerName = customer.CustomerName;
                        receiptsHeader.MarkedForAllocation = false;
                        receiptsHeader.Allocated = false;

                        _DBContext.Entry(receiptsHeader).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (receipt.receiptsDetails != null && receipt.receiptsDetails.Count > 0)
                        {
                            foreach (ReceiptsDetail recDetail in receipt.receiptsDetails)
                            {
                                var receiptDetailObj = await _DBContext.ReceiptsDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReceiptId == recDetail.ReceiptId &&
                                                        x.ReceiptDetailId == recDetail.ReceiptDetailId).FirstOrDefaultAsync();

                                if (receiptDetailObj != null)
                                {
                                    receiptDetailObj.CustomerId = recDetail.CustomerId;
                                    receiptDetailObj.DocumentNumber = recDetail.DocumentNumber;
                                    receiptDetailObj.DocumentDate = recDetail.DocumentDate;
                                    receiptDetailObj.PaymentId = recDetail.PaymentId;
                                    receiptDetailObj.PayedId = recDetail.PayedId;
                                    receiptDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                    receiptDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                    receiptDetailObj.DiscountTaken = recDetail.DiscountTaken;
                                    receiptDetailObj.WriteOffAmount = recDetail.WriteOffAmount;
                                    receiptDetailObj.AppliedAmount = recDetail.AppliedAmount;
                                    receiptDetailObj.Cleared = false;
                                    receiptDetailObj.ProjectId = recDetail.ProjectId;
                                    receiptDetailObj.DetailMemo1 = recDetail.DetailMemo1;
                                    receiptDetailObj.DetailMemo2 = recDetail.DetailMemo2;
                                    receiptDetailObj.DetailMemo3 = recDetail.DetailMemo3;
                                    receiptDetailObj.DetailMemo4 = recDetail.DetailMemo4;
                                    receiptDetailObj.DetailMemo5 = recDetail.DetailMemo5;
                                    receiptDetailObj.LockedBy = null;
                                    receiptDetailObj.LockTs = null;
                                    receiptDetailObj.TaxGroupId = recDetail.TaxGroupId;
                                    receiptDetailObj.TaxAmount = recDetail.TaxAmount;
                                    receiptDetailObj.TaxRate = recDetail.TaxRate;
                                    receiptDetailObj.GlanalysisType1 = recDetail.GlanalysisType1;
                                    receiptDetailObj.GlanalysisType2 = recDetail.GlanalysisType2;
                                    receiptDetailObj.AssetId = recDetail.AssetId;
                                    receiptDetailObj.CommissionRate = recDetail.CommissionRate;
                                    receiptDetailObj.CommissionType = recDetail.CommissionType;
                                    receiptDetailObj.BranchCode = recDetail.BranchCode;
                                    receiptDetailObj.BankId = recDetail.BankId;
                                    receiptDetailObj.PaidAmount = recDetail.PaidAmount;

                                    _DBContext.Entry(receiptDetailObj).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                }
                                else
                                {
                                    recDetail.CompanyId = token.CompanyId;
                                    recDetail.DivisionId = token.DivisionId;
                                    recDetail.DepartmentId = token.DepartmentId;
                                    recDetail.ReceiptId = receiptId;
                                    recDetail.ReceiptDetailId = 0;
                                    recDetail.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                    recDetail.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                    recDetail.Cleared = false;
                                    recDetail.BankId = receiptBank != null ? receiptBank.BankId : defaultBank.BankId;




                                    _DBContext.Entry(recDetail).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                }

                            }
                        }

                        _DBContext.SaveChanges();

                        //call order booking

                        await ReceiptPosting(receiptId, token);

                        statusMessage.Status = "Success";
                        statusMessage.Message = receiptId; ;
                        //statusMessage.data = await ReceiptPosting(receiptId, token);
                        // statusMessage.Message = receiptId;
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Receipt Information";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }


        public async Task<StatusMessage> ReceiptPosting(string Id, ApiToken tokenObj)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                bool status = await receipt_Post(Id, tokenObj);

                if (status)
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Receipt Successfully Posted";
                    //statusMessage.data = getreceipbyID(receiptId, token);

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Posting Receipt Failed.Try again or contact system administrator";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<bool> receipt_Post(string ReceiptId, ApiToken token)
        {
            bool status = false;

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sReceiptID = new SqlParameter("@ReceiptID", ReceiptId);
                var sEmployeeID = new SqlParameter("@EmployeeID", token.Username);
                var sSuccess = new SqlParameter("@Success", SqlDbType.Int);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                sSuccess.Direction = ParameterDirection.Output;
                PostingResult.Direction = ParameterDirection.Output;


                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.Receipt_Post @CompanyID, @DivisionID, @DepartmentID, @ReceiptID, @EmployeeID, @Success, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sReceiptID, sEmployeeID, sSuccess, PostingResult });

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<IEnumerable<Receipt>> GetReceiptByCusId(string customerId, ApiToken token)
        {
            List<Data.ViewModels.Receipt> receipt = new List<Data.ViewModels.Receipt>();
            try
            {

                var rec = await _DBContext.ReceiptsHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == customerId)
                                                         .OrderByDescending(x => x.ReceiptId).ToListAsync();
                receipt = await receipts(rec, token);
            }
            catch (Exception ex)
            {

            }
            return receipt;
        }

        public async Task<Data.Models.InventoryItems> GetInventoryItemById(string ItemId, ApiToken token)
        {
            Data.Models.InventoryItems Item = new Data.Models.InventoryItems();
            try
            {
                Item = await _DBContext.InventoryItems.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.ItemId == ItemId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

            }
            return Item;
        }

        public async Task<CustomerInform> GetCustomerById(string id, ApiToken token)
        {
            CustomerInform cusById = new CustomerInform();
            try
            {
                var cusId = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.CustomerId == id).FirstOrDefaultAsync();
                cusById = await cusInfo(cusId, token);
            }
            catch (Exception ex)
            {

            }
            return cusById;
        }

        //private async Task<CustomerInform> customer(CustomerInformation customersInformation, ApiToken token)
        //{
        //    CustomerInform customers = new CustomerInform();
        //    try
        //    {
        //        //if (customersInformation != null)
        //        //{
        //        //    foreach (CustomerInformation customerInformation in customersInformation)
        //        //    {
        //        //        CustomersInfo customerObj = new Data.ViewModels.CustomersInfo();

        //                customers.CompanyId = customersInformation.CompanyId;
        //                customers.DivisionId = customersInformation.DivisionId;
        //                customers.DepartmentId = customersInformation.DepartmentId;
        //                customers.CustomerId = customersInformation.CustomerId;
        //                customers.AccountStatus = customersInformation.AccountStatus;
        //                customers.CustomerName = customersInformation.CustomerName;
        //                customers.CustomerAddress1 = customersInformation.CustomerAddress1;
        //                customers.CustomerAddress2 = customersInformation.CustomerAddress2;
        //                customers.CustomerAddress3 = customersInformation.CustomerAddress3;
        //                customers.CustomerCity = customersInformation.CustomerCity;
        //                customers.CustomerState = customersInformation.CustomerState;
        //                customers.CustomerZip = customersInformation.CustomerZip;
        //                customers.CustomerCountry = customersInformation.CustomerCountry;
        //                customers.CustomerPhone = customersInformation.CustomerPhone;
        //                customers.CustomerFax = customersInformation.CustomerFax;
        //                customers.CustomerEmail = customersInformation.CustomerEmail;
        //                customers.CustomerWebPage = customersInformation.CustomerWebPage;
        //                customers.CustomerLogin = customersInformation.CustomerLogin;
        //                customers.CustomerPasswordDate = customersInformation.CustomerPasswordDate;
        //                customers.CustomerPasswordExpires = customersInformation.CustomerPasswordExpires;
        //                customers.CustomerPasswordExpiresDate = customersInformation.CustomerPasswordExpiresDate;
        //                customers.CustomerFirstName = customersInformation.CustomerFirstName;
        //                customers.CustomerSalutation = customersInformation.CustomerSalutation;
        //                customers.Attention = customersInformation.Attention;
        //                customers.CustomerTypeId = customersInformation.CustomerTypeId;
        //                customers.TaxIdno = customersInformation.TaxIdno;
        //                customers.VattaxIdnumber = customersInformation.VattaxIdnumber;
        //                customers.VatTaxOtherNumber = customersInformation.VatTaxOtherNumber;
        //                customers.GlsalesAccount = customersInformation.GlsalesAccount;
        //                customers.TermsId = customersInformation.TermsId;
        //                customers.TermsStart = customersInformation.TermsStart;
        //                customers.EmployeeId = customersInformation.EmployeeId;
        //                customers.TaxGroupId = customersInformation.TaxGroupId;
        //                customers.PriceMatrix = customersInformation.PriceMatrix;
        //                customers.PriceMatrixCurrent = customersInformation.PriceMatrixCurrent;
        //                customers.CreditRating = customersInformation.CreditRating;
        //                customers.CreditLimit = customersInformation.CreditLimit;
        //                customers.CreditComments = customersInformation.CreditComments;
        //                customers.PaymentDay = customersInformation.PaymentDay;
        //                customers.ApprovalDate = customersInformation.ApprovalDate;
        //                customers.CustomerSince = customersInformation.CustomerSince;
        //                customers.SendCreditMemos = customersInformation.SendCreditMemos;
        //                customers.SendDebitMemos = customersInformation.SendDebitMemos;
        //                customers.Statements = customersInformation.Statements;
        //                customers.StatementCycleCode = customersInformation.StatementCycleCode;
        //                customers.CustomerSpecialInstructions = customersInformation.CustomerSpecialInstructions;
        //                customers.CustomerShipToId = customersInformation.CustomerShipToId;
        //                customers.ShipMethodId = customersInformation.ShipMethodId;
        //                customers.WarehouseId = customersInformation.WarehouseId;
        //                customers.RoutingInfo1 = customersInformation.RoutingInfo1;
        //                customers.RoutingInfo2 = customersInformation.RoutingInfo2;
        //                customers.RoutingInfo3 = customersInformation.RoutingInfo3;
        //                customers.RoutingInfoCurrent = customersInformation.RoutingInfoCurrent;
        //                customers.FreightPayment = customersInformation.FreightPayment;
        //                customers.PickTicketsNeeded = customersInformation.PickTicketsNeeded;
        //                customers.PackingListNeeded = customersInformation.PackingListNeeded;
        //                customers.SpecialLabelsNeeded = customersInformation.SpecialLabelsNeeded;
        //                customers.CustomerItemCodes = customersInformation.CustomerItemCodes;
        //                customers.ConfirmBeforeShipping = customersInformation.ConfirmBeforeShipping;
        //                customers.Backorders = customersInformation.Backorders;
        //                customers.UseStoreNumbers = customersInformation.UseStoreNumbers;
        //                customers.UseDepartmentNumbers = customersInformation.UseDepartmentNumbers;
        //                customers.SpecialShippingInstructions = customersInformation.SpecialShippingInstructions;
        //                customers.RoutingNotes = customersInformation.RoutingNotes;
        //                customers.ApplyRebate = customersInformation.ApplyRebate;
        //                customers.RebateAmount = customersInformation.RebateAmount;
        //                customers.RebateGlaccount = customersInformation.RebateGlaccount;
        //                customers.RebateGlaccount = customersInformation.RebateGlaccount;
        //                customers.ApplyNewStore = customersInformation.ApplyNewStore;
        //                customers.NewStoreGlaccount = customersInformation.NewStoreGlaccount;
        //                customers.NewStoreDiscount = customersInformation.NewStoreDiscount;
        //                customers.NewStoreDiscountNotes = customersInformation.NewStoreDiscountNotes;
        //                customers.ApplyWarehouse = customersInformation.ApplyWarehouse;
        //                customers.WarehouseAllowance = customersInformation.WarehouseAllowance;
        //                customers.WarehouseGlaccount = customersInformation.WarehouseGlaccount;
        //                customers.WarehouseAllowanceNotes = customersInformation.WarehouseAllowanceNotes;
        //                customers.ApplyAdvertising = customersInformation.ApplyAdvertising;
        //                customers.AdvertisingDiscount = customersInformation.AdvertisingDiscount;
        //                customers.AdvertisingGlaccount = customersInformation.AdvertisingGlaccount;
        //                customers.ApplyManualAdvert = customersInformation.ApplyManualAdvert;
        //                customers.ManualAdvertising = customersInformation.ManualAdvertising;
        //                customers.RebateGlaccount = customersInformation.RebateGlaccount;
        //                customers.ManualAdvertisingGlaccount = customersInformation.ManualAdvertisingGlaccount;
        //                customers.ManualAdvertisingNotes = customersInformation.ManualAdvertisingNotes;
        //                customers.ApplyTrade = customersInformation.ApplyTrade;
        //                customers.TradeDiscount = customersInformation.TradeDiscount;
        //                customers.TradeDiscountGlaccount = customersInformation.TradeDiscountGlaccount;
        //                customers.TradeDiscountNotes = customersInformation.TradeDiscountNotes;
        //                customers.SpecialTerms = customersInformation.SpecialTerms;
        //                customers.Ediqualifier = customersInformation.Ediqualifier;
        //                customers.Ediid = customersInformation.Ediid;
        //                customers.EditestQualifier = customersInformation.EditestQualifier;
        //                customers.EditestId = customersInformation.EditestId;
        //                customers.EdicontactName = customersInformation.EdicontactName;
        //                customers.EdicontactAgentFax = customersInformation.EdicontactAgentFax;
        //                customers.EdicontactAgentPhone = customersInformation.EdicontactAgentPhone;
        //                customers.EdicontactAddressLine = customersInformation.EdicontactAddressLine;
        //                customers.EdipurchaseOrders = customersInformation.EdipurchaseOrders;
        //                customers.Ediinvoices = customersInformation.Ediinvoices;
        //                customers.Edipayments = customersInformation.Edipayments;
        //                customers.EdiorderStatus = customersInformation.EdiorderStatus;
        //                customers.EdishippingNotices = customersInformation.EdishippingNotices;
        //                customers.Approved = customersInformation.Approved;
        //                customers.ApprovedBy = customersInformation.ApprovedBy;
        //                customers.ApprovedDate = customersInformation.ApprovedDate;
        //                customers.EnteredBy = customersInformation.EnteredBy;
        //                customers.ConvertedFromVendor = customersInformation.ConvertedFromVendor;
        //                customers.ConvertedFromLead = customersInformation.ConvertedFromLead;
        //                customers.CustomerRegionId = customersInformation.CustomerRegionId;
        //                customers.CustomerSourceId = customersInformation.CustomerSourceId;
        //                customers.CustomerIndustryId = customersInformation.CustomerIndustryId;
        //                customers.Confirmed = customersInformation.Confirmed;
        //                customers.FirstContacted = customersInformation.FirstContacted;
        //                customers.LastFollowUp = customersInformation.LastFollowUp;
        //                customers.NextFollowUp = customersInformation.NextFollowUp;
        //                customers.ReferedByExistingCustomer = customersInformation.ReferedByExistingCustomer;
        //                customers.ReferedBy = customersInformation.ReferedBy;
        //                customers.ReferalUrl = customersInformation.ReferalUrl;
        //                customers.Hot = customersInformation.Hot;
        //                customers.PrimaryInterest = customersInformation.PrimaryInterest;
        //                customers.LockedBy = customersInformation.LockedBy;
        //                customers.LockTs = customersInformation.LockTs;
        //                customers.AccountBalance = customersInformation.AccountBalance;
        //                customers.BranchCode = customersInformation.BranchCode;
        //                customers.KnowYourCustomer = customersInformation.KnowYourCustomer;
        //                customers.Smsalert = customersInformation.Smsalert;
        //                customers.EmailAlert = customersInformation.EmailAlert;

        //                customers.customerFinancials = await _DBContext.CustomerFinancials.Where(x => x.CompanyId == token.CompanyId &&
        //                                                   x.DivisionId == token.DivisionId &&
        //                                                   x.DepartmentId == token.DepartmentId &&
        //                                                   x.CustomerId == customersInformation.CustomerId).ToListAsync();

        //        //        customers.Add(customers);
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return customers;

        //}

        private async Task<Data.ViewModels.CustomerInform> cusInfo(CustomerInformation customerInfo, ApiToken token)
        {
            Data.ViewModels.CustomerInform customers = new Data.ViewModels.CustomerInform();
            try
            {
                customers.CompanyId = customerInfo.CompanyId;
                customers.DivisionId = customerInfo.DivisionId;
                customers.DepartmentId = customerInfo.DepartmentId;
                customers.CustomerId = customerInfo.CustomerId;
                customers.AccountStatus = customerInfo.AccountStatus;
                customers.CustomerName = customerInfo.CustomerName;
                customers.CustomerAddress1 = customerInfo.CustomerAddress1;
                customers.CustomerAddress2 = customerInfo.CustomerAddress2;
                customers.CustomerAddress3 = customerInfo.CustomerAddress3;
                customers.CustomerCity = customerInfo.CustomerCity;
                customers.CustomerState = customerInfo.CustomerState;
                customers.CustomerZip = customerInfo.CustomerZip;
                customers.CustomerCountry = customerInfo.CustomerCountry;
                customers.CustomerPhone = customerInfo.CustomerPhone;
                customers.CustomerFax = customerInfo.CustomerFax;
                customers.CustomerEmail = customerInfo.CustomerEmail;
                customers.CustomerWebPage = customerInfo.CustomerWebPage;
                customers.CustomerLogin = customerInfo.CustomerLogin;
                customers.CustomerPasswordDate = customerInfo.CustomerPasswordDate;
                customers.CustomerPasswordExpires = customerInfo.CustomerPasswordExpires;
                customers.CustomerPasswordExpiresDate = customerInfo.CustomerPasswordExpiresDate;
                customers.CustomerFirstName = customerInfo.CustomerFirstName;
                customers.CustomerSalutation = customerInfo.CustomerSalutation;
                customers.Attention = customerInfo.Attention;
                customers.CustomerTypeId = customerInfo.CustomerTypeId;
                customers.TaxIdno = customerInfo.TaxIdno;
                customers.VattaxIdnumber = customerInfo.VattaxIdnumber;
                customers.VatTaxOtherNumber = customerInfo.VatTaxOtherNumber;
                customers.GlsalesAccount = customerInfo.GlsalesAccount;
                customers.TermsId = customerInfo.TermsId;
                customers.TermsStart = customerInfo.TermsStart;
                customers.EmployeeId = customerInfo.EmployeeId;
                customers.TaxGroupId = customerInfo.TaxGroupId;
                customers.PriceMatrix = customerInfo.PriceMatrix;
                customers.PriceMatrixCurrent = customerInfo.PriceMatrixCurrent;
                customers.CreditRating = customerInfo.CreditRating;
                customers.CreditLimit = customerInfo.CreditLimit;
                customers.CreditComments = customerInfo.CreditComments;
                customers.PaymentDay = customerInfo.PaymentDay;
                customers.ApprovalDate = customerInfo.ApprovalDate;
                customers.CustomerSince = customerInfo.CustomerSince;
                customers.SendCreditMemos = customerInfo.SendCreditMemos;
                customers.SendDebitMemos = customerInfo.SendDebitMemos;
                customers.Statements = customerInfo.Statements;
                customers.StatementCycleCode = customerInfo.StatementCycleCode;
                customers.CustomerSpecialInstructions = customerInfo.CustomerSpecialInstructions;
                customers.CustomerShipToId = customerInfo.CustomerShipToId;
                customers.ShipMethodId = customerInfo.ShipMethodId;
                customers.WarehouseId = customerInfo.WarehouseId;
                customers.RoutingInfo1 = customerInfo.RoutingInfo1;
                customers.RoutingInfo2 = customerInfo.RoutingInfo2;
                customers.RoutingInfo3 = customerInfo.RoutingInfo3;
                customers.RoutingInfoCurrent = customerInfo.RoutingInfoCurrent;
                customers.FreightPayment = customerInfo.FreightPayment;
                customers.PickTicketsNeeded = customerInfo.PickTicketsNeeded;
                customers.PackingListNeeded = customerInfo.PackingListNeeded;
                customers.SpecialLabelsNeeded = customerInfo.SpecialLabelsNeeded;
                customers.CustomerItemCodes = customerInfo.CustomerItemCodes;
                customers.ConfirmBeforeShipping = customerInfo.ConfirmBeforeShipping;
                customers.Backorders = customerInfo.Backorders;
                customers.UseStoreNumbers = customerInfo.UseStoreNumbers;
                customers.UseDepartmentNumbers = customerInfo.UseDepartmentNumbers;
                customers.SpecialShippingInstructions = customerInfo.SpecialShippingInstructions;
                customers.RoutingNotes = customerInfo.RoutingNotes;
                customers.ApplyRebate = customerInfo.ApplyRebate;
                customers.RebateAmount = customerInfo.RebateAmount;
                customers.RebateGlaccount = customerInfo.RebateGlaccount;
                customers.RebateGlaccount = customerInfo.RebateGlaccount;
                customers.ApplyNewStore = customerInfo.ApplyNewStore;
                customers.NewStoreGlaccount = customerInfo.NewStoreGlaccount;
                customers.NewStoreDiscount = customerInfo.NewStoreDiscount;
                customers.NewStoreDiscountNotes = customerInfo.NewStoreDiscountNotes;
                customers.ApplyWarehouse = customerInfo.ApplyWarehouse;
                customers.WarehouseAllowance = customerInfo.WarehouseAllowance;
                customers.WarehouseGlaccount = customerInfo.WarehouseGlaccount;
                customers.WarehouseAllowanceNotes = customerInfo.WarehouseAllowanceNotes;
                customers.ApplyAdvertising = customerInfo.ApplyAdvertising;
                customers.AdvertisingDiscount = customerInfo.AdvertisingDiscount;
                customers.AdvertisingGlaccount = customerInfo.AdvertisingGlaccount;
                customers.ApplyManualAdvert = customerInfo.ApplyManualAdvert;
                customers.ManualAdvertising = customerInfo.ManualAdvertising;
                customers.RebateGlaccount = customerInfo.RebateGlaccount;
                customers.ManualAdvertisingGlaccount = customerInfo.ManualAdvertisingGlaccount;
                customers.ManualAdvertisingNotes = customerInfo.ManualAdvertisingNotes;
                customers.ApplyTrade = customerInfo.ApplyTrade;
                customers.TradeDiscount = customerInfo.TradeDiscount;
                customers.TradeDiscountGlaccount = customerInfo.TradeDiscountGlaccount;
                customers.TradeDiscountNotes = customerInfo.TradeDiscountNotes;
                customers.SpecialTerms = customerInfo.SpecialTerms;
                customers.Ediqualifier = customerInfo.Ediqualifier;
                customers.Ediid = customerInfo.Ediid;
                customers.EditestQualifier = customerInfo.EditestQualifier;
                customers.EditestId = customerInfo.EditestId;
                customers.EdicontactName = customerInfo.EdicontactName;
                customers.EdicontactAgentFax = customerInfo.EdicontactAgentFax;
                customers.EdicontactAgentPhone = customerInfo.EdicontactAgentPhone;
                customers.EdicontactAddressLine = customerInfo.EdicontactAddressLine;
                customers.EdipurchaseOrders = customerInfo.EdipurchaseOrders;
                customers.Ediinvoices = customerInfo.Ediinvoices;
                customers.Edipayments = customerInfo.Edipayments;
                customers.EdiorderStatus = customerInfo.EdiorderStatus;
                customers.EdishippingNotices = customerInfo.EdishippingNotices;
                customers.Approved = customerInfo.Approved;
                customers.ApprovedBy = customerInfo.ApprovedBy;
                customers.ApprovedDate = customerInfo.ApprovedDate;
                customers.EnteredBy = customerInfo.EnteredBy;
                customers.ConvertedFromVendor = customerInfo.ConvertedFromVendor;
                customers.ConvertedFromLead = customerInfo.ConvertedFromLead;
                customers.CustomerRegionId = customerInfo.CustomerRegionId;
                customers.CustomerSourceId = customerInfo.CustomerSourceId;
                customers.CustomerIndustryId = customerInfo.CustomerIndustryId;
                customers.Confirmed = customerInfo.Confirmed;
                customers.FirstContacted = customerInfo.FirstContacted;
                customers.LastFollowUp = customerInfo.LastFollowUp;
                customers.NextFollowUp = customerInfo.NextFollowUp;
                customers.ReferedByExistingCustomer = customerInfo.ReferedByExistingCustomer;
                customers.ReferedBy = customerInfo.ReferedBy;
                customers.ReferalUrl = customerInfo.ReferalUrl;
                customers.Hot = customerInfo.Hot;
                customers.PrimaryInterest = customerInfo.PrimaryInterest;
                customers.LockedBy = customerInfo.LockedBy;
                customers.LockTs = customerInfo.LockTs;
                customers.AccountBalance = customerInfo.AccountBalance;
                customers.BranchCode = customerInfo.BranchCode;
                customers.KnowYourCustomer = customerInfo.KnowYourCustomer;
                customers.Smsalert = customerInfo.Smsalert;
                customers.EmailAlert = customerInfo.EmailAlert;

                customers.customerFinancials = await _DBContext.CustomerFinancials.Where(x => x.CompanyId == token.CompanyId &&
                           x.DivisionId == token.DivisionId &&
                           x.DepartmentId == token.DepartmentId &&
                           x.CustomerId == customerInfo.CustomerId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

            }

            return customers;

        }

        public async Task<CustomerInform> GetCustomerByEmail(string Email, ApiToken token)
        {
            CustomerInform cusById = new CustomerInform();
            try
            {
                var cusId = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.CustomerEmail == Email).FirstOrDefaultAsync();
                //if (cusId != null)
                //{

                //var cusFinancial = await _DBContext.CustomerFinancials.Where(x => x.CompanyId == token.CompanyId &&
                //                                                     x.DivisionId == token.DivisionId &&
                //                                                     x.DepartmentId == token.DepartmentId &&
                //                                                     x.CustomerId == cusId.CustomerId).FirstOrDefaultAsync();
                //}
                cusById = await cusInfo(cusId, token);
                // = await cusInfo(customerByEmail, token);


            }
            catch (Exception ex)
            {

            }
            return cusById;
        }

        public async Task<double> CalcSubTot(Order order, ApiToken token)
        {
            PayStackInitialize initialize = new PayStackInitialize();
            StatusMessage statusMessage = new StatusMessage();
            CustomerInform CustomerInfo = new CustomerInform();

            var Total = 0.0;

            var inventoryItems = await GetItemsByCustomerId(order.CustomerId, token);
            var cus = await GetCustomerById(order.CustomerId, token);

            //var Final;
            foreach (OrderDetail orderDetailObj in order.orderDetail)
            {

                //double price = (double)inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault().Price;
                var item = inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault();

                double price = (double)item.Price;
                var itemTotal = orderDetailObj.OrderQty * price;

                if (cus.ApplyRebate == true)
                {
                    var RebateTotal = (cus.RebateAmount * 0.01) * itemTotal;
                    itemTotal = (double)(itemTotal - RebateTotal);
                }

                bool isTaxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);

                if (isTaxable)
                {
                    //call stored procedure to get tax percent;
                    double taxPercent = await getTotalTaxPercent(item.TaxGroupId, token);

                    //apply tax on item total
                    itemTotal += (itemTotal * taxPercent * 0.01);
                }


                double ItemTotal = (double)itemTotal;
                Total += ItemTotal;
            }

            return Total;
        }

        public async Task<double> CalcTotalWithAvailableCredit(Order order, ApiToken token)
        {
            PayStackInitialize initialize = new PayStackInitialize();
            StatusMessage statusMessage = new StatusMessage();
            CustomerInform CustomerInfo = new CustomerInform();

            var Total = 0.0;
            var FinalTotal = 0.0;

            var inventoryItems = await GetItemsByCustomerId(order.CustomerId, token);
            var cus = await GetCustomerById(order.CustomerId, token);

            //var Final;
            foreach (OrderDetail orderDetailObj in order.orderDetail)
            {

                //double price = (double)inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault().Price;
                var item = inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault();

                double price = (double)item.Price;
                var itemTotal = orderDetailObj.OrderQty * price;

                if (cus.ApplyRebate == true)
                {
                    var RebateTotal = (cus.RebateAmount * 0.01) * itemTotal;
                    itemTotal = (double)(itemTotal - RebateTotal);
                }

                bool isTaxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);

                if (isTaxable)
                {
                    //call stored procedure to get tax percent;
                    double taxPercent = await getTotalTaxPercent(item.TaxGroupId, token);

                    //apply tax on item total
                    itemTotal += (itemTotal * taxPercent * 0.01);
                }


                double ItemTotal = (double)itemTotal;
                Total += ItemTotal;
            }



            double availCred = ((double)(cus.customerFinancials.AvailibleCredit ?? 0));

            //if (cus.ApplyRebate == true)
            //{
            //    var RebateTotal = (cus.RebateAmount * 0.01) * Total;
            //    Total = (double)(Total - RebateTotal);
            //}

            if (availCred >= Total)
            {
                //set payable amount to zero - the customer do not need to pay
                FinalTotal = 0;
            }
            else //compute paystack commission
            {

                FinalTotal = Total - availCred;
            }

            // FinalTotal = Math.Ceiling(FinalTotal);

            return FinalTotal;
        }

        public async Task<double> CalcAmountToPayToPaystack(Order order, string email, ApiToken token)
        {
            PayStackInitialize initialize = new PayStackInitialize();
            StatusMessage statusMessage = new StatusMessage();
            CustomerInform CustomerInfo = new CustomerInform();

            var Total = 0.0;
            var FinTotal = 0.0;
            var FinalTotal = 0.0;
            var creditTotal = 0.0;

            var inventoryItems = await GetItemsByCustomerId(order.CustomerId, token);
            var cus = await GetCustomerById(order.CustomerId, token);


            //var Final;
            foreach (OrderDetail orderDetailObj in order.orderDetail)
            {

                var item = inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault();

                double price = (double)item.Price;
                var itemTotal = orderDetailObj.OrderQty * price;

                if (cus.ApplyRebate == true)
                {
                    var RebateTotal = (cus.RebateAmount * 0.01) * itemTotal;
                    itemTotal = (double)(itemTotal - RebateTotal);
                }

                bool isTaxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);

                if (isTaxable)
                {
                    //call stored procedure to get tax percent;
                    double taxPercent = await getTotalTaxPercent(item.TaxGroupId, token);

                    //apply tax on item total
                    itemTotal += (itemTotal * taxPercent * 0.01);
                }

                double ItemTotal = (double)itemTotal;
                Total += ItemTotal;
            }


            double availCred = ((double)(cus.customerFinancials.AvailibleCredit ?? 0));


            if (availCred >= Total)
            {
                //set payable amount to zero - the customer do not need to pay
                FinalTotal = 0;
            }
            else //compute paystack commission
            {

                creditTotal = Total - availCred;

                if (creditTotal < 2500)
                {
                    FinTotal = (creditTotal / 0.985);
                }
                else if (creditTotal >= 2500)
                {
                    FinTotal = ((creditTotal + 100) / 0.985);
                }
                else if (creditTotal >= 124667.00)
                {
                    FinTotal = (creditTotal + 2000);
                }

                FinalTotal = FinTotal * 100;
                FinalTotal = Math.Ceiling(FinalTotal);
            }

            return FinalTotal;
        }

        /// <summary>
        /// Initializes Payment Order
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="email"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private async Task<PayStackInitialize> PaystackPaymentInitialization(string reference, string email, double amount, ApiToken token)
        {
            PayStackInitialize payStackInitialize = new PayStackInitialize();

            var tok = await GetPaymentToken(token);
            string bearerToken = tok.ApiKey;

            PayInitialize myReqObj = new PayInitialize();

            myReqObj.email = email;
            myReqObj.amount = amount;

            try
            {
                var url = tok.BaseUrl;
                //var url = "https://api.paystack.co/transaction/"; // + reference + "";

                var uri = new Uri(url);
                string initializeEndPoint = "initialize";

                var integrationHelper = new IntegrationHelper(uri);

                var requestUrl = integrationHelper.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                initializeEndPoint));

                payStackInitialize = await integrationHelper.PostAsync<PayStackInitialize, PayInitialize>(requestUrl, myReqObj, bearerToken);


            }
            catch (Exception ex)
            {
                payStackInitialize.status = false;
                payStackInitialize.message = "Try Again." + ex.Message;
            }
            return payStackInitialize;
        }

        public async Task<StatusMessage> PaymentInitialization(Order order, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            OrderReferenceLog orderLog = new OrderReferenceLog();
            try
            {

                var company = await _DBContext.Companies.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();

                var customer = await _DBContext.CustomerInformation.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.CustomerId == order.CustomerId).FirstOrDefaultAsync();

                var dEmail = customer.CustomerEmail == null || customer.CustomerEmail == "" ? company.CompanyEmail : customer.CustomerEmail;

                if (dEmail == null || dEmail == "")
                {
                    dEmail = "0@gmail.com";
                }

                bool isEmailValid = Regex.IsMatch(dEmail, pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$", options: RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));



                var subTotal = await CalcAmountToPayToPaystack(order, dEmail, token);

                if (subTotal > 0)
                {

                    var reference = await AddOrderReference(orderLog, token);


                    var paymentInitialize = await PaystackPaymentInitialization(reference.RefNumber, dEmail, subTotal, token);

                    if (paymentInitialize.status)
                    {
                        statusMessage.Status = "Success";
                        statusMessage.Message = paymentInitialize.message;
                        statusMessage.data = paymentInitialize.data;
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = paymentInitialize.message;
                        statusMessage.data = paymentInitialize.data;
                    }
                }
                else
                {
                    auth authInfo = new auth();

                    authInfo.access_code = "";
                    authInfo.authorization_url = null;
                    authInfo.reference = "";

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = authInfo;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }
        public async Task<OrderReferenceLog> AddOrderReference(OrderReferenceLog orderLog, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            OrderReferenceLog orderRefLog = new OrderReferenceLog();

            try
            {
                string newGuid = Guid.NewGuid().ToString();

                //string newGuid = "Asfdmnf2xfcn";
                //check if object empty
                if (orderLog != null)
                {
                    var OrderRefLog = await _DBContext.OrderReferenceLog.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.RefNumber == orderLog.RefNumber).FirstOrDefaultAsync();


                    if (OrderRefLog != null) // for update payroll policy
                    {


                        OrderRefLog.OrderId = orderLog.OrderId;
                        OrderRefLog.Used = false;
                        OrderRefLog.DateUsed = DateTime.Now;
                        OrderRefLog.TransSource = "";

                        _DBContext.Entry(OrderRefLog).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll loan policy
                    {

                        orderLog = new OrderReferenceLog();

                        orderLog.CompanyId = token.CompanyId;
                        orderLog.DivisionId = token.DivisionId;
                        orderLog.DepartmentId = token.DepartmentId;
                        orderLog.RefNumber = newGuid;
                        orderLog.OrderId = await getNextEntityID(_NextNumberName, token);
                        orderLog.Used = false;
                        orderLog.DateUsed = DateTime.Now;
                        orderLog.TransSource = "";

                        _DBContext.Entry(orderLog).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid POS Shift Type Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return orderLog;
        }

        public async Task<Paging> GetRMA(PaginationParams Param, ApiToken token)
        {
            List<RMA> rmaDetail = new List<RMA>();

            try
            {
                var totalCount = await _DBContext.RmatransactionsHeader.CountAsync();

                var rmaDet = await _DBContext.RmatransactionsHeader.OrderByDescending(x => x.InvoiceDate).Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)
                                                        .OrderBy(x => x.InvoiceNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage)
                                                        .ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                rmaDetail = await Rma(rmaDet, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    RmaList = rmaDetail
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RMA>> GetRmaByCustomerId(string customerId, ApiToken token)
        {
            List<RMA> rmaDetail = new List<RMA>();

            try
            {
                var rmaDet = await _DBContext.RmatransactionsHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CustomerId == customerId)
                                                        .OrderByDescending(x => x.InvoiceNumber).ToListAsync();

                rmaDetail = await Rma(rmaDet, token);
            }
            catch (Exception ex)
            {

            }
            return rmaDetail;
        }

        public async Task<IEnumerable<RMA>> GetRmaByInvoiceId(string id, ApiToken token)
        {
            List<RMA> rmaDetails = new List<RMA>();
            try
            {
                var rma = await _DBContext.RmatransactionsHeader.OrderByDescending(x => x.InvoiceDate).Where(x =>
                                                           x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.InvoiceNumber == id).ToListAsync();
                rmaDetails = await Rma(rma, token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return rmaDetails;
        }

        public async Task<IEnumerable<OrderRetrieveDto>> GetOrderByInvoiceId(string Id, ApiToken token)
        {
            List<OrderRetrieveDto> orderdetail = new List<OrderRetrieveDto>();
            try
            {
                var Order = await _DBContext.OrderHeader.OrderByDescending(x => x.OrderDate).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.InvoiceNumber == Id &&
                                                           x.TransactionTypeId == "Order").ToListAsync();
                orderdetail = await order(Order, token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return orderdetail;
        }

        public async Task<double> CalcRmaSubTotal(RmatransactionsDetail detail, ApiToken token, OrderDetail originalItem)
        {
            var ItemTotal = 0.0;
            ;
            if (originalItem != null)
            {
                if (originalItem.OrderQty >= detail.OrderQty)
                {
                    double price = (double)originalItem.ItemUnitPrice;
                    var itemTotal = detail.OrderQty * price;
                    ItemTotal = (double)itemTotal;
                }

            }
            else
            {
                throw new InvalidOperationException("Invalid Rma Detail");
            }
            return ItemTotal;
        }

        private async Task<List<RMA>> Rma(List<RmatransactionsHeader> rmaDetail, ApiToken token)
        {
            List<RMA> rmas = new List<RMA>();
            try
            {
                if (rmaDetail != null)
                {
                    foreach (RmatransactionsHeader rmaDetails in rmaDetail)
                    {
                        RMA rmaObj = new RMA();

                        rmaObj.CompanyId = rmaDetails.CompanyId;
                        rmaObj.DivisionId = rmaDetails.DivisionId;
                        rmaObj.DepartmentId = rmaDetails.DepartmentId;
                        rmaObj.InvoiceNumber = rmaDetails.InvoiceNumber;
                        rmaObj.OrderNumber = rmaDetails.OrderNumber;
                        rmaObj.TransactionTypeId = rmaDetails.TransactionTypeId;
                        rmaObj.TransOpen = rmaDetails.TransOpen;
                        rmaObj.InvoiceDate = rmaDetails.InvoiceDate;
                        rmaObj.InvoiceDueDate = rmaDetails.InvoiceDueDate;
                        rmaObj.InvoiceShipDate = rmaDetails.InvoiceShipDate;
                        rmaObj.InvoiceCancelDate = rmaDetails.InvoiceCancelDate;
                        rmaObj.SystemDate = rmaDetails.SystemDate;
                        rmaObj.Memorize = rmaDetails.Memorize;
                        rmaObj.PurchaseOrderNumber = rmaDetails.PurchaseOrderNumber;
                        rmaObj.TaxExemptId = rmaDetails.TaxExemptId;
                        rmaObj.TaxGroupId = rmaDetails.TaxGroupId;
                        rmaObj.CustomerId = rmaDetails.CustomerId;
                        rmaObj.TermsId = rmaDetails.TermsId;
                        rmaObj.CurrencyId = rmaDetails.CurrencyId;
                        rmaObj.CurrencyExchangeRate = rmaDetails.CurrencyExchangeRate;
                        rmaObj.Subtotal = rmaDetails.Subtotal;
                        rmaObj.DiscountAmount = rmaDetails.DiscountAmount;
                        rmaObj.DiscountPers = rmaDetails.DiscountPers;
                        rmaObj.TaxPercent = rmaDetails.TaxPercent;
                        rmaObj.TaxAmount = rmaDetails.TaxAmount;
                        rmaObj.TaxableSubTotal = rmaDetails.TaxableSubTotal;
                        rmaObj.Freight = rmaDetails.Freight;
                        rmaObj.TaxFreight = rmaDetails.TaxFreight;
                        rmaObj.Handling = rmaDetails.Handling;
                        rmaObj.Total = rmaDetails.Total;
                        rmaObj.EmployeeId = rmaDetails.EmployeeId;
                        rmaObj.CommissionPaid = rmaDetails.CommissionPaid;
                        rmaObj.CommissionSelectToPay = rmaDetails.CommissionSelectToPay;
                        rmaObj.Commission = rmaDetails.Commission;
                        rmaObj.CommissionableSales = rmaDetails.CommissionableSales;
                        rmaObj.ComissionalbleCost = rmaDetails.ComissionalbleCost;
                        rmaObj.CustomerDropShipment = rmaDetails.CustomerDropShipment;
                        rmaObj.ShipMethodId = rmaDetails.ShipMethodId;
                        rmaObj.WarehouseId = rmaDetails.WarehouseId;
                        rmaObj.ShipToId = rmaDetails.ShipToId;
                        rmaObj.ShipForId = rmaDetails.ShipForId;
                        rmaObj.ShippingName = rmaDetails.ShippingName;
                        rmaObj.ShippingAddress1 = rmaDetails.ShippingAddress1;
                        rmaObj.ShippingAddress2 = rmaDetails.ShippingAddress2;
                        rmaObj.ShippingAddress3 = rmaDetails.ShippingAddress3;
                        rmaObj.ShippingCity = rmaDetails.ShippingCity;
                        rmaObj.ShippingState = rmaDetails.ShippingState;
                        rmaObj.ShippingZip = rmaDetails.ShippingZip;
                        rmaObj.ShippingCountry = rmaDetails.ShippingCountry;
                        rmaObj.ScheduledEndDate = rmaDetails.ScheduledEndDate;
                        rmaObj.ScheduledStartDate = rmaDetails.ScheduledStartDate;
                        rmaObj.ServiceEndDate = rmaDetails.ServiceEndDate;
                        rmaObj.ServiceStartDate = rmaDetails.ServiceStartDate;
                        rmaObj.PerformedBy = rmaDetails.PerformedBy;
                        rmaObj.GlsalesAccount = rmaDetails.GlsalesAccount;
                        rmaObj.Glcogaccount = rmaDetails.Glcogaccount;
                        rmaObj.PaymentMethodId = rmaDetails.PaymentMethodId;
                        rmaObj.AmountPaid = rmaDetails.AmountPaid;
                        rmaObj.UndistributedAmount = rmaDetails.UndistributedAmount;
                        rmaObj.BalanceDue = rmaDetails.BalanceDue;
                        rmaObj.CheckNumber = rmaDetails.CheckNumber;
                        rmaObj.CheckDate = rmaDetails.CheckDate;
                        rmaObj.CreditCardTypeId = rmaDetails.CreditCardTypeId;
                        rmaObj.CreditCardName = rmaDetails.CreditCardName;
                        rmaObj.CreditCardTypeId = rmaDetails.CreditCardNumber;
                        rmaObj.CreditCardExpDate = rmaDetails.CreditCardExpDate;
                        rmaObj.CreditCardBillToZip = rmaDetails.CreditCardBillToZip;
                        rmaObj.CreditCardValidationCode = rmaDetails.CreditCardValidationCode;
                        rmaObj.CreditCardApprovalNumber = rmaDetails.CreditCardApprovalNumber;
                        rmaObj.Picked = rmaDetails.Picked;
                        rmaObj.PickedDate = rmaDetails.PickedDate;
                        rmaObj.Printed = rmaDetails.Printed;
                        rmaObj.PrintedDate = rmaDetails.PrintedDate;
                        rmaObj.Shipped = rmaDetails.Shipped;
                        rmaObj.ShipDate = rmaDetails.ShipDate;
                        rmaObj.TrackingNumber = rmaDetails.TrackingNumber;
                        rmaObj.Billed = rmaDetails.Billed;
                        rmaObj.BilledDate = rmaDetails.BilledDate;
                        rmaObj.Backordered = rmaDetails.Backordered;
                        rmaObj.Posted = rmaDetails.Posted;
                        rmaObj.PostedDate = rmaDetails.PostedDate;
                        rmaObj.AllowanceDiscountPerc = rmaDetails.AllowanceDiscountPerc;
                        rmaObj.CashTendered = rmaDetails.CashTendered;
                        rmaObj.MasterBillOfLading = rmaDetails.MasterBillOfLading;
                        rmaObj.MasterBillOfLadingDate = rmaDetails.MasterBillOfLadingDate;
                        rmaObj.TrailerNumber = rmaDetails.TrailerNumber;
                        rmaObj.TrailerPrefix = rmaDetails.TrailerPrefix;
                        rmaObj.HeaderMemo1 = rmaDetails.HeaderMemo1;
                        rmaObj.HeaderMemo2 = rmaDetails.HeaderMemo2;
                        rmaObj.HeaderMemo3 = rmaDetails.HeaderMemo3;
                        rmaObj.HeaderMemo4 = rmaDetails.HeaderMemo4;
                        rmaObj.HeaderMemo5 = rmaDetails.HeaderMemo5;
                        rmaObj.HeaderMemo6 = rmaDetails.HeaderMemo6;
                        rmaObj.HeaderMemo7 = rmaDetails.HeaderMemo7;
                        rmaObj.HeaderMemo8 = rmaDetails.HeaderMemo8;
                        rmaObj.HeaderMemo9 = rmaDetails.HeaderMemo9;
                        rmaObj.Approved = rmaDetails.Approved;
                        rmaObj.ApprovedBy = rmaDetails.ApprovedBy;
                        rmaObj.ApprovedDate = rmaDetails.ApprovedDate;
                        rmaObj.PrintedDate = rmaDetails.PrintedDate;
                        rmaObj.EnteredBy = rmaDetails.EnteredBy;
                        rmaObj.Signature = rmaDetails.Signature;
                        rmaObj.SignaturePassword = rmaDetails.SignaturePassword;
                        rmaObj.SupervisorSignature = rmaDetails.SupervisorSignature;
                        rmaObj.SupervisorPassword = rmaDetails.SupervisorPassword;
                        rmaObj.ManagerSignature = rmaDetails.ManagerSignature;
                        rmaObj.ManagerPassword = rmaDetails.ManagerPassword;
                        rmaObj.LockedBy = rmaDetails.LockedBy;
                        rmaObj.LockTs = rmaDetails.LockTs;
                        rmaObj.BankId = rmaDetails.BankId;
                        rmaObj.DiscountLine1 = rmaDetails.DiscountLine1;
                        rmaObj.DiscountLine2 = rmaDetails.DiscountLine2;
                        rmaObj.DiscountLine3 = rmaDetails.DiscountLine3;
                        rmaObj.DiscountLine4 = rmaDetails.DiscountLine4;
                        rmaObj.DiscountLine5 = rmaDetails.DiscountLine5;
                        rmaObj.DiscountGroupId1 = rmaDetails.DiscountGroupId1;
                        rmaObj.DiscountGroupId2 = rmaDetails.DiscountGroupId2;
                        rmaObj.DiscountGroupId3 = rmaDetails.DiscountGroupId3;
                        rmaObj.DiscountGroupId4 = rmaDetails.DiscountGroupId4;
                        rmaObj.DiscountGroupId5 = rmaDetails.DiscountGroupId5;
                        rmaObj.BranchCode = rmaDetails.BranchCode;

                        rmaObj.RmaDetail = await _DBContext.RmatransactionsDetail.Where(x =>
                                                                        x.CompanyId == token.CompanyId &&
                                                                        x.DivisionId == token.DivisionId &&
                                                                        x.DepartmentId == token.DepartmentId &&
                                                                        x.InvoiceNumber == rmaDetails.InvoiceNumber).ToListAsync();
                        //x.CurrencyId == rmaDetails.CurrencyId &&
                        //x.WarehouseId == rmaDetails.WarehouseId


                        rmas.Add(rmaObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return rmas;

        }

        public async Task<StatusMessage> AddRMA(RMA returnMA, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<RmatransactionsDetail> RmaDetail = new List<RmatransactionsDetail>();


            RMA rma = new RMA();

            var tot = 0.0;
            var GrandTotal = 0.0;

            try
            {
                var company = await _DBContext.Companies.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();


                if (company != null)
                {
                    var currency = await _DBContext.CurrencyTypes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CurrencyId == company.CurrencyId).FirstOrDefaultAsync();
                }

                var orderInvoice = await _DBContext.InvoiceHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.InvoiceNumber == returnMA.OrderNumber &&
                                                        x.CustomerId == returnMA.CustomerId).FirstOrDefaultAsync();
                if (orderInvoice != null)
                {
                    //check if object empty
                    if (returnMA != null)
                    {

                        var InvItems = await GetOrderByInvoiceId(returnMA.OrderNumber, token);
                        Order originalOrder = InvItems.FirstOrDefault();
                        var InvDate = DateTime.Now.Date;

                        if (returnMA.InvoiceDate >= originalOrder.InvoiceDate)
                        {
                            InvDate = (DateTime)returnMA.InvoiceDate;
                        }
                        else
                        {
                            throw new InvalidOperationException("Invalid Rma Detail");
                        }
                        var rMerchandise = await _DBContext.RmatransactionsHeader.Where(x =>
                                                            x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.InvoiceNumber == returnMA.InvoiceNumber).FirstOrDefaultAsync();

                        if (rMerchandise != null) // for update payroll policy
                        {
                            rMerchandise.TransactionTypeId = "RMA";
                            rMerchandise.TransOpen = returnMA.TransOpen;
                            rMerchandise.InvoiceDate = InvDate;
                            rMerchandise.InvoiceDueDate = returnMA.InvoiceDueDate;
                            rMerchandise.InvoiceShipDate = returnMA.InvoiceShipDate;
                            rMerchandise.InvoiceCancelDate = returnMA.InvoiceCancelDate;
                            rMerchandise.OrderNumber = returnMA.OrderNumber;
                            rMerchandise.SystemDate = returnMA.SystemDate;
                            rMerchandise.Memorize = returnMA.Memorize;
                            rMerchandise.PurchaseOrderNumber = returnMA.PurchaseOrderNumber;
                            rMerchandise.TaxExemptId = returnMA.TaxExemptId;
                            rMerchandise.TaxGroupId = returnMA.TaxGroupId;
                            rMerchandise.CustomerId = returnMA.CustomerId;
                            rMerchandise.TermsId = returnMA.TermsId;
                            rMerchandise.CurrencyId = originalOrder.CurrencyId;
                            rMerchandise.CurrencyExchangeRate = originalOrder.CurrencyExchangeRate;
                            rMerchandise.DiscountAmount = returnMA.DiscountAmount;
                            rMerchandise.DiscountPers = returnMA.DiscountPers;
                            rMerchandise.TaxPercent = returnMA.TaxPercent;
                            rMerchandise.TaxAmount = returnMA.TaxAmount;
                            rMerchandise.TaxableSubTotal = returnMA.TaxableSubTotal;
                            rMerchandise.Freight = returnMA.Freight;
                            rMerchandise.TaxFreight = returnMA.TaxFreight;
                            rMerchandise.Handling = returnMA.Handling;
                            //rMerchandise.Total = GrandTotal;
                            rMerchandise.EmployeeId = returnMA.EmployeeId;
                            rMerchandise.CommissionPaid = returnMA.CommissionPaid;
                            rMerchandise.CommissionSelectToPay = returnMA.CommissionSelectToPay;
                            rMerchandise.Commission = returnMA.Commission;
                            rMerchandise.CommissionableSales = returnMA.CommissionableSales;
                            rMerchandise.ComissionalbleCost = returnMA.ComissionalbleCost;
                            rMerchandise.CustomerDropShipment = returnMA.CustomerDropShipment;
                            rMerchandise.ShipMethodId = returnMA.ShipMethodId;
                            rMerchandise.WarehouseId = originalOrder.WarehouseId;
                            rMerchandise.ShipToId = returnMA.ShipToId;
                            rMerchandise.ShipForId = returnMA.ShipForId;
                            rMerchandise.ShippingName = returnMA.ShippingName;
                            rMerchandise.ShippingAddress1 = returnMA.ShippingAddress1;
                            rMerchandise.ShippingAddress2 = returnMA.ShippingAddress2;
                            rMerchandise.ShippingAddress3 = returnMA.ShippingAddress3;
                            rMerchandise.ShippingCity = returnMA.ShippingCity;
                            rMerchandise.ShippingState = returnMA.ShippingState;
                            rMerchandise.ShippingZip = returnMA.ShippingZip;
                            rMerchandise.ShippingCountry = returnMA.ShippingCountry;
                            rMerchandise.ScheduledEndDate = returnMA.ScheduledEndDate;
                            rMerchandise.ScheduledStartDate = returnMA.ScheduledStartDate;
                            rMerchandise.ServiceEndDate = returnMA.ServiceEndDate;
                            rMerchandise.ServiceStartDate = returnMA.ServiceStartDate;
                            rMerchandise.PerformedBy = returnMA.PerformedBy;
                            rMerchandise.GlsalesAccount = returnMA.GlsalesAccount;
                            rMerchandise.Glcogaccount = returnMA.Glcogaccount;
                            rMerchandise.PaymentMethodId = returnMA.PaymentMethodId;
                            rMerchandise.AmountPaid = returnMA.AmountPaid;
                            rMerchandise.UndistributedAmount = returnMA.UndistributedAmount;
                            rMerchandise.BalanceDue = returnMA.BalanceDue;
                            rMerchandise.CheckNumber = returnMA.CheckNumber;
                            rMerchandise.CheckDate = returnMA.CheckDate;
                            rMerchandise.CreditCardTypeId = returnMA.CreditCardTypeId;
                            rMerchandise.CreditCardName = returnMA.CreditCardName;
                            rMerchandise.CreditCardNumber = returnMA.CreditCardNumber;
                            rMerchandise.CreditCardExpDate = returnMA.CreditCardExpDate;
                            rMerchandise.CreditCardBillToZip = returnMA.CreditCardBillToZip;
                            rMerchandise.CreditCardValidationCode = returnMA.CreditCardValidationCode;
                            rMerchandise.CreditCardApprovalNumber = returnMA.CreditCardApprovalNumber;
                            rMerchandise.Picked = returnMA.Picked;
                            rMerchandise.PickedDate = returnMA.PickedDate;
                            rMerchandise.Printed = returnMA.Printed;
                            rMerchandise.PrintedDate = returnMA.PrintedDate;
                            rMerchandise.Shipped = returnMA.Shipped;
                            rMerchandise.ShipDate = returnMA.ShipDate;
                            rMerchandise.TrackingNumber = returnMA.TrackingNumber;
                            rMerchandise.Billed = returnMA.Billed;
                            rMerchandise.BilledDate = returnMA.BilledDate;
                            rMerchandise.Backordered = returnMA.Backordered;
                            rMerchandise.Posted = returnMA.Posted;
                            rMerchandise.PostedDate = returnMA.PostedDate;
                            rMerchandise.AllowanceDiscountPerc = returnMA.AllowanceDiscountPerc;
                            rMerchandise.CashTendered = returnMA.CashTendered;
                            rMerchandise.MasterBillOfLading = returnMA.MasterBillOfLading;
                            rMerchandise.MasterBillOfLadingDate = returnMA.MasterBillOfLadingDate;
                            rMerchandise.TrailerNumber = returnMA.TrailerNumber;
                            rMerchandise.TrailerPrefix = returnMA.TrailerPrefix;
                            rMerchandise.HeaderMemo1 = returnMA.HeaderMemo1;
                            rMerchandise.HeaderMemo2 = returnMA.HeaderMemo2;
                            rMerchandise.HeaderMemo3 = returnMA.HeaderMemo3;
                            rMerchandise.HeaderMemo4 = returnMA.HeaderMemo4;
                            rMerchandise.HeaderMemo5 = returnMA.HeaderMemo5;
                            rMerchandise.HeaderMemo6 = returnMA.HeaderMemo6;
                            rMerchandise.HeaderMemo7 = returnMA.HeaderMemo7;
                            rMerchandise.HeaderMemo8 = returnMA.HeaderMemo8;
                            rMerchandise.HeaderMemo9 = "Mobile Sales";
                            rMerchandise.Approved = returnMA.Approved;
                            rMerchandise.ApprovedBy = returnMA.ApprovedBy;
                            rMerchandise.ApprovedDate = returnMA.ApprovedDate;
                            rMerchandise.PrintedDate = returnMA.PrintedDate;
                            rMerchandise.EnteredBy = returnMA.EnteredBy;
                            rMerchandise.Signature = returnMA.Signature;
                            rMerchandise.SignaturePassword = returnMA.SignaturePassword;
                            rMerchandise.SupervisorSignature = returnMA.SupervisorSignature;
                            rMerchandise.SupervisorPassword = returnMA.SupervisorPassword;
                            rMerchandise.ManagerSignature = returnMA.ManagerSignature;
                            rMerchandise.ManagerPassword = returnMA.ManagerPassword;
                            rMerchandise.LockedBy = returnMA.LockedBy;
                            rMerchandise.LockTs = returnMA.LockTs;
                            rMerchandise.BankId = returnMA.BankId;
                            rMerchandise.DiscountLine1 = returnMA.DiscountLine1;
                            rMerchandise.DiscountLine2 = returnMA.DiscountLine2;
                            rMerchandise.DiscountLine3 = returnMA.DiscountLine3;
                            rMerchandise.DiscountLine4 = returnMA.DiscountLine4;
                            rMerchandise.DiscountLine5 = returnMA.DiscountLine5;
                            rMerchandise.DiscountGroupId1 = returnMA.DiscountGroupId1;
                            rMerchandise.DiscountGroupId2 = returnMA.DiscountGroupId2;
                            rMerchandise.DiscountGroupId3 = returnMA.DiscountGroupId3;
                            rMerchandise.DiscountGroupId4 = returnMA.DiscountGroupId4;
                            rMerchandise.DiscountGroupId5 = returnMA.DiscountGroupId5;
                            rMerchandise.BranchCode = returnMA.BranchCode;


                            if (returnMA.RmaDetail != null && returnMA.RmaDetail.Count > 0)
                            {
                                var InvItem = await GetOrderByInvoiceId(rma.OrderNumber, token);
                                List<OrderDetail> originalorderHeaders = InvItem.FirstOrDefault().orderDetail;

                                foreach (RmatransactionsDetail returnDetail in returnMA.RmaDetail)
                                {
                                    var originalItem = originalorderHeaders.Find(elem => elem.ItemId == returnDetail.ItemId);
                                    tot = await CalcRmaSubTotal(returnDetail, token, originalItem);
                                    GrandTotal += tot;

                                    var rmatransactionsDetailObj = await _DBContext.RmatransactionsDetail.Where(x =>
                                    x.CompanyId == token.CompanyId &&
                                    x.DivisionId == token.DivisionId &&
                                    x.DepartmentId == token.DepartmentId &&
                                    x.InvoiceNumber == returnDetail.InvoiceNumber &&
                                    x.InvoiceLineNumber == returnDetail.InvoiceLineNumber).FirstOrDefaultAsync();
                                    var item = await _DBContext.InventoryItems.Where(x =>
                                                            x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.ItemId == returnDetail.ItemId).FirstOrDefaultAsync();
                                    if (item != null && returnDetail.OrderQty > 0)
                                    {
                                        //var tots = returnDetail.ItemUnitPrice * returnDetail.OrderQty;

                                        if (rmatransactionsDetailObj != null)
                                        {
                                            rmatransactionsDetailObj.ItemId = returnDetail.ItemId;
                                            rmatransactionsDetailObj.ItemUpccode = returnDetail.ItemUpccode;
                                            rmatransactionsDetailObj.WarehouseId = originalItem.WarehouseId;
                                            rmatransactionsDetailObj.SerialNumber = returnDetail.SerialNumber;
                                            rmatransactionsDetailObj.OrderQty = returnDetail.OrderQty;
                                            rmatransactionsDetailObj.BackOrdered = returnDetail.BackOrdered;
                                            rmatransactionsDetailObj.BackOrderQty = returnDetail.BackOrderQty;
                                            rmatransactionsDetailObj.ItemUom = returnDetail.ItemUom;
                                            rmatransactionsDetailObj.ItemWeight = returnDetail.ItemWeight;
                                            rmatransactionsDetailObj.Description = originalItem.Description;
                                            rmatransactionsDetailObj.DiscountPerc = returnDetail.DiscountPerc;
                                            rmatransactionsDetailObj.Taxable = originalItem.Taxable;
                                            rmatransactionsDetailObj.CurrencyId = returnDetail.CurrencyId;
                                            rmatransactionsDetailObj.CurrencyExchangeRate = returnDetail.CurrencyExchangeRate;
                                            rmatransactionsDetailObj.ItemCost = returnDetail.ItemCost;
                                            rmatransactionsDetailObj.ItemUnitPrice = (double)originalItem.ItemUnitPrice;
                                            rmatransactionsDetailObj.TaxGroupId = originalItem.TaxGroupId;
                                            rmatransactionsDetailObj.TaxAmount = returnDetail.TaxAmount;
                                            rmatransactionsDetailObj.TaxPercent = originalItem.TaxPercent;
                                            rmatransactionsDetailObj.SubTotal = (decimal)tot;
                                            rmatransactionsDetailObj.Total = (decimal)tot;
                                            rmatransactionsDetailObj.TotalWeight = returnDetail.TotalWeight;
                                            rmatransactionsDetailObj.GlsalesAccount = originalItem.GlsalesAccount;
                                            rmatransactionsDetailObj.Glcogaccount = originalItem.Glcogaccount;
                                            rmatransactionsDetailObj.ProjectId = originalItem.ProjectId;
                                            rmatransactionsDetailObj.WarehouseBinId = originalItem.WarehouseBinId;
                                            rmatransactionsDetailObj.PalletLevel = returnDetail.PalletLevel;
                                            rmatransactionsDetailObj.CartonLevel = returnDetail.CartonLevel;
                                            rmatransactionsDetailObj.PackLevelA = returnDetail.PackLevelA;
                                            rmatransactionsDetailObj.PackLevelB = returnDetail.PackLevelB;
                                            rmatransactionsDetailObj.PackLevelC = returnDetail.PackLevelC;
                                            rmatransactionsDetailObj.TrackingNumber = originalItem.OrderNumber;
                                            rmatransactionsDetailObj.ScheduledStartDate = returnDetail.ScheduledStartDate;
                                            rmatransactionsDetailObj.ScheduledStartDate = returnDetail.ScheduledEndDate;
                                            rmatransactionsDetailObj.ServiceStartDate = returnDetail.ServiceStartDate;
                                            rmatransactionsDetailObj.ServiceEndDate = returnDetail.ServiceEndDate;
                                            rmatransactionsDetailObj.PerformedBy = returnDetail.PerformedBy;
                                            rmatransactionsDetailObj.DetailMemo1 = returnDetail.DetailMemo1;
                                            rmatransactionsDetailObj.DetailMemo2 = returnDetail.DetailMemo2;
                                            rmatransactionsDetailObj.DetailMemo3 = returnDetail.DetailMemo3;
                                            rmatransactionsDetailObj.DetailMemo4 = returnDetail.DetailMemo4;
                                            rmatransactionsDetailObj.DetailMemo5 = returnDetail.DetailMemo5;
                                            rmatransactionsDetailObj.LockedBy = returnDetail.LockedBy;
                                            rmatransactionsDetailObj.LockTs = returnDetail.LockTs;
                                            rmatransactionsDetailObj.ItemPricingCode = returnDetail.ItemPricingCode;
                                            rmatransactionsDetailObj.DeliveryNumber = returnDetail.DeliveryNumber;
                                            rmatransactionsDetailObj.GlanalysisType1 = returnDetail.GlanalysisType1;
                                            rmatransactionsDetailObj.GlanalysisType2 = returnDetail.GlanalysisType2;
                                            rmatransactionsDetailObj.AssetId = returnDetail.AssetId;
                                            rmatransactionsDetailObj.BudgetId = returnDetail.BudgetId;
                                            rmatransactionsDetailObj.MultipleDiscountGroupId = originalItem.MultipleDiscountGroupId;
                                            rmatransactionsDetailObj.MultipleDiscountAmount = originalItem.MultipleDiscountAmount;
                                            rmatransactionsDetailObj.MultipleDiscountPercent = originalItem.MultipleDiscountPercent;
                                            rmatransactionsDetailObj.DiscountAmount = originalItem.DiscountAmount;
                                            rmatransactionsDetailObj.MarkUponCost = originalItem.MarkUponCost;
                                            rmatransactionsDetailObj.MarkUpRate = originalItem.MarkUpRate;
                                            rmatransactionsDetailObj.ItemUnitCost = originalItem.ItemUnitCost;
                                            rmatransactionsDetailObj.InvoicedDate = originalItem.InvoicedDate;
                                            rmatransactionsDetailObj.BranchCode = originalItem.BranchCode;
                                            rmatransactionsDetailObj.ProductTypeId = originalItem.ProductTypeId;
                                            rmatransactionsDetailObj.AdvertTypeId = originalItem.AdvertTypeId;
                                            rmatransactionsDetailObj.UnAppliedTotal = returnDetail.UnAppliedTotal;

                                            _DBContext.Entry(rmatransactionsDetailObj).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            returnDetail.CompanyId = token.CompanyId;
                                            returnDetail.DivisionId = token.DivisionId;
                                            returnDetail.DepartmentId = token.DepartmentId;

                                            _DBContext.Entry(returnDetail).State = EntityState.Added;
                                        }
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "Invalid Item To Be Reversed";
                                    }
                                }
                            }

                            rMerchandise.Subtotal = (decimal)GrandTotal;
                            rMerchandise.Total = (decimal)GrandTotal;
                            _DBContext.Entry(rMerchandise).State = EntityState.Modified;
                            _DBContext.SaveChanges();

                            statusMessage.Status = "Success";
                            statusMessage.Message = "Success";
                        }
                        else // insert or create payroll policy
                        {

                            RmatransactionsHeader rmatransactionsHeader = new RmatransactionsHeader();

                            _NextNumberName = "NextInvoiceNumber";
                            string InvoiceNumber = await getNextEntityID(_NextNumberName, token);

                            rmatransactionsHeader.CompanyId = token.CompanyId;
                            rmatransactionsHeader.DivisionId = token.DivisionId;
                            rmatransactionsHeader.DepartmentId = token.DepartmentId; ;
                            rmatransactionsHeader.InvoiceNumber = InvoiceNumber;
                            rmatransactionsHeader.TransactionTypeId = "RMA";
                            rmatransactionsHeader.TransOpen = returnMA.TransOpen;
                            rmatransactionsHeader.InvoiceDate = InvDate;
                            rmatransactionsHeader.InvoiceDueDate = returnMA.InvoiceDueDate;
                            rmatransactionsHeader.InvoiceShipDate = returnMA.InvoiceShipDate;
                            rmatransactionsHeader.InvoiceCancelDate = returnMA.InvoiceCancelDate;
                            rmatransactionsHeader.OrderNumber = returnMA.OrderNumber;
                            rmatransactionsHeader.SystemDate = returnMA.SystemDate;
                            rmatransactionsHeader.Memorize = returnMA.Memorize;
                            rmatransactionsHeader.PurchaseOrderNumber = returnMA.PurchaseOrderNumber;
                            rmatransactionsHeader.TaxExemptId = returnMA.TaxExemptId;
                            rmatransactionsHeader.TaxGroupId = returnMA.TaxGroupId;
                            rmatransactionsHeader.CustomerId = returnMA.CustomerId;
                            rmatransactionsHeader.TermsId = returnMA.TermsId;
                            rmatransactionsHeader.CurrencyId = originalOrder.CurrencyId;
                            rmatransactionsHeader.CurrencyExchangeRate = originalOrder.CurrencyExchangeRate;
                            rmatransactionsHeader.Subtotal = (decimal)tot;
                            rmatransactionsHeader.Subtotal = (decimal)GrandTotal;
                            rmatransactionsHeader.DiscountAmount = returnMA.DiscountAmount;
                            rmatransactionsHeader.DiscountPers = returnMA.DiscountPers;
                            rmatransactionsHeader.TaxPercent = returnMA.TaxPercent;
                            rmatransactionsHeader.TaxAmount = returnMA.TaxAmount;
                            rmatransactionsHeader.TaxableSubTotal = returnMA.TaxableSubTotal;
                            rmatransactionsHeader.Freight = returnMA.Freight;
                            rmatransactionsHeader.TaxFreight = returnMA.TaxFreight;
                            rmatransactionsHeader.Handling = returnMA.Handling;
                            rmatransactionsHeader.Total = (decimal)tot;
                            rmatransactionsHeader.Total = (decimal)GrandTotal;
                            rmatransactionsHeader.EmployeeId = returnMA.EmployeeId;
                            rmatransactionsHeader.CommissionPaid = returnMA.CommissionPaid;
                            rmatransactionsHeader.CommissionSelectToPay = returnMA.CommissionSelectToPay;
                            rmatransactionsHeader.Commission = returnMA.Commission;
                            rmatransactionsHeader.CommissionableSales = returnMA.CommissionableSales;
                            rmatransactionsHeader.ComissionalbleCost = returnMA.ComissionalbleCost;
                            rmatransactionsHeader.CustomerDropShipment = returnMA.CustomerDropShipment;
                            rmatransactionsHeader.ShipMethodId = returnMA.ShipMethodId;
                            rmatransactionsHeader.WarehouseId = originalOrder.WarehouseId;
                            rmatransactionsHeader.ShipToId = returnMA.ShipToId;
                            rmatransactionsHeader.ShipForId = returnMA.ShipForId;
                            rmatransactionsHeader.ShippingName = returnMA.ShippingName;
                            rmatransactionsHeader.ShippingAddress1 = returnMA.ShippingAddress1;
                            rmatransactionsHeader.ShippingAddress2 = returnMA.ShippingAddress2;
                            rmatransactionsHeader.ShippingAddress3 = returnMA.ShippingAddress3;
                            rmatransactionsHeader.ShippingCity = returnMA.ShippingCity;
                            rmatransactionsHeader.ShippingState = returnMA.ShippingState;
                            rmatransactionsHeader.ShippingZip = returnMA.ShippingZip;
                            rmatransactionsHeader.ShippingCountry = returnMA.ShippingCountry;
                            rmatransactionsHeader.ScheduledEndDate = returnMA.ScheduledEndDate;
                            rmatransactionsHeader.ScheduledStartDate = returnMA.ScheduledStartDate;
                            rmatransactionsHeader.ServiceEndDate = returnMA.ServiceEndDate;
                            rmatransactionsHeader.ServiceStartDate = returnMA.ServiceStartDate;
                            rmatransactionsHeader.PerformedBy = returnMA.PerformedBy;
                            rmatransactionsHeader.GlsalesAccount = returnMA.GlsalesAccount;
                            rmatransactionsHeader.Glcogaccount = returnMA.Glcogaccount;
                            rmatransactionsHeader.PaymentMethodId = returnMA.PaymentMethodId;
                            rmatransactionsHeader.AmountPaid = returnMA.AmountPaid;
                            rmatransactionsHeader.UndistributedAmount = returnMA.UndistributedAmount;
                            rmatransactionsHeader.BalanceDue = returnMA.BalanceDue;
                            rmatransactionsHeader.CheckNumber = returnMA.CheckNumber;
                            rmatransactionsHeader.CheckDate = returnMA.CheckDate;
                            rmatransactionsHeader.CreditCardTypeId = returnMA.CreditCardTypeId;
                            rmatransactionsHeader.CreditCardName = returnMA.CreditCardName;
                            rmatransactionsHeader.CreditCardNumber = returnMA.CreditCardNumber;
                            rmatransactionsHeader.CreditCardExpDate = returnMA.CreditCardExpDate;
                            rmatransactionsHeader.CreditCardBillToZip = returnMA.CreditCardBillToZip;
                            rmatransactionsHeader.CreditCardValidationCode = returnMA.CreditCardValidationCode;
                            rmatransactionsHeader.CreditCardApprovalNumber = returnMA.CreditCardApprovalNumber;
                            rmatransactionsHeader.Picked = returnMA.Picked;
                            rmatransactionsHeader.PickedDate = returnMA.PickedDate;
                            rmatransactionsHeader.Printed = returnMA.Printed;
                            rmatransactionsHeader.PrintedDate = returnMA.PrintedDate;
                            rmatransactionsHeader.Shipped = returnMA.Shipped;
                            rmatransactionsHeader.ShipDate = returnMA.ShipDate;
                            rmatransactionsHeader.TrackingNumber = originalOrder.OrderNumber;
                            rmatransactionsHeader.Billed = returnMA.Billed;
                            rmatransactionsHeader.BilledDate = returnMA.BilledDate;
                            rmatransactionsHeader.Backordered = returnMA.Backordered;
                            rmatransactionsHeader.Posted = returnMA.Posted;
                            rmatransactionsHeader.PostedDate = returnMA.PostedDate;
                            rmatransactionsHeader.AllowanceDiscountPerc = returnMA.AllowanceDiscountPerc;
                            rmatransactionsHeader.CashTendered = returnMA.CashTendered;
                            rmatransactionsHeader.MasterBillOfLading = returnMA.MasterBillOfLading;
                            rmatransactionsHeader.MasterBillOfLadingDate = returnMA.MasterBillOfLadingDate;
                            rmatransactionsHeader.TrailerNumber = returnMA.TrailerNumber;
                            rmatransactionsHeader.TrailerPrefix = returnMA.TrailerPrefix;
                            rmatransactionsHeader.HeaderMemo1 = returnMA.HeaderMemo1;
                            rmatransactionsHeader.HeaderMemo2 = returnMA.HeaderMemo2;
                            rmatransactionsHeader.HeaderMemo3 = returnMA.HeaderMemo3;
                            rmatransactionsHeader.HeaderMemo4 = returnMA.HeaderMemo4;
                            rmatransactionsHeader.HeaderMemo5 = returnMA.HeaderMemo5;
                            rmatransactionsHeader.HeaderMemo6 = returnMA.HeaderMemo6;
                            rmatransactionsHeader.HeaderMemo7 = returnMA.HeaderMemo7;
                            rmatransactionsHeader.HeaderMemo8 = returnMA.HeaderMemo8;
                            rmatransactionsHeader.HeaderMemo9 = "Mobile Sales";
                            rmatransactionsHeader.Approved = returnMA.Approved;
                            rmatransactionsHeader.ApprovedBy = returnMA.ApprovedBy;
                            rmatransactionsHeader.ApprovedDate = returnMA.ApprovedDate;
                            rmatransactionsHeader.PrintedDate = returnMA.PrintedDate;
                            rmatransactionsHeader.EnteredBy = returnMA.EnteredBy;
                            rmatransactionsHeader.Signature = returnMA.Signature;
                            rmatransactionsHeader.SignaturePassword = returnMA.SignaturePassword;
                            rmatransactionsHeader.SupervisorSignature = returnMA.SupervisorSignature;
                            rmatransactionsHeader.SupervisorPassword = returnMA.SupervisorPassword;
                            rmatransactionsHeader.ManagerSignature = returnMA.ManagerSignature;
                            rmatransactionsHeader.ManagerPassword = returnMA.ManagerPassword;
                            rmatransactionsHeader.LockedBy = returnMA.LockedBy;
                            rmatransactionsHeader.LockTs = returnMA.LockTs;
                            rmatransactionsHeader.BankId = returnMA.BankId;
                            rmatransactionsHeader.DiscountLine1 = returnMA.DiscountLine1;
                            rmatransactionsHeader.DiscountLine2 = returnMA.DiscountLine2;
                            rmatransactionsHeader.DiscountLine3 = returnMA.DiscountLine3;
                            rmatransactionsHeader.DiscountLine4 = returnMA.DiscountLine4;
                            rmatransactionsHeader.DiscountLine5 = returnMA.DiscountLine5;
                            rmatransactionsHeader.DiscountGroupId1 = returnMA.DiscountGroupId1;
                            rmatransactionsHeader.DiscountGroupId2 = returnMA.DiscountGroupId2;
                            rmatransactionsHeader.DiscountGroupId3 = returnMA.DiscountGroupId3;
                            rmatransactionsHeader.DiscountGroupId4 = returnMA.DiscountGroupId4;
                            rmatransactionsHeader.DiscountGroupId5 = returnMA.DiscountGroupId5;
                            rmatransactionsHeader.BranchCode = returnMA.BranchCode;

                            //_DBContext.Entry(rmatransactionsHeader).State = EntityState.Added;
                            // _DBContext.SaveChanges();

                            if (returnMA.RmaDetail != null && returnMA.RmaDetail.Count > 0)
                            {

                                var InvItem = await GetOrderByInvoiceId(returnMA.OrderNumber, token);
                                List<OrderDetail> originalorderHeaders = InvItem.FirstOrDefault().orderDetail;

                                foreach (RmatransactionsDetail returnDetail in returnMA.RmaDetail)
                                {
                                    var originalItem = originalorderHeaders.Find(elem => elem.ItemId == returnDetail.ItemId);
                                    tot = await CalcRmaSubTotal(returnDetail, token, originalItem);
                                    GrandTotal += tot;

                                    var rmatransactionsDetailObj = await _DBContext.RmatransactionsDetail.Where(x =>
                                                            x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.InvoiceNumber == returnDetail.InvoiceNumber &&
                                                            x.InvoiceLineNumber == returnDetail.InvoiceLineNumber).FirstOrDefaultAsync();
                                    var item = await _DBContext.InventoryItems.Where(x =>
                                                            x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.ItemId == returnDetail.ItemId).FirstOrDefaultAsync();

                                    if (item != null && returnDetail.OrderQty > 0)
                                    {
                                        //var tots = returnDetail.ItemUnitPrice * returnDetail.OrderQty;

                                        if (rmatransactionsDetailObj != null)
                                        {
                                            rmatransactionsDetailObj.ItemId = returnDetail.ItemId;
                                            rmatransactionsDetailObj.ItemUpccode = returnDetail.ItemUpccode;
                                            rmatransactionsDetailObj.WarehouseId = originalItem.WarehouseId;
                                            rmatransactionsDetailObj.SerialNumber = returnDetail.SerialNumber;
                                            rmatransactionsDetailObj.OrderQty = returnDetail.OrderQty;
                                            rmatransactionsDetailObj.BackOrdered = returnDetail.BackOrdered;
                                            rmatransactionsDetailObj.BackOrderQty = returnDetail.BackOrderQty;
                                            rmatransactionsDetailObj.ItemUom = returnDetail.ItemUom;
                                            rmatransactionsDetailObj.ItemWeight = returnDetail.ItemWeight;
                                            rmatransactionsDetailObj.Description = originalItem.Description;
                                            rmatransactionsDetailObj.DiscountPerc = returnDetail.DiscountPerc;
                                            rmatransactionsDetailObj.Taxable = originalItem.Taxable;
                                            rmatransactionsDetailObj.CurrencyId = returnDetail.CurrencyId;
                                            rmatransactionsDetailObj.CurrencyExchangeRate = returnDetail.CurrencyExchangeRate;
                                            rmatransactionsDetailObj.ItemCost = returnDetail.ItemCost;
                                            rmatransactionsDetailObj.ItemUnitPrice = (double)originalItem.ItemUnitPrice;
                                            rmatransactionsDetailObj.TaxGroupId = originalItem.TaxGroupId;
                                            rmatransactionsDetailObj.TaxAmount = returnDetail.TaxAmount;
                                            rmatransactionsDetailObj.TaxPercent = originalItem.TaxPercent;
                                            rmatransactionsDetailObj.SubTotal = (decimal)tot;
                                            rmatransactionsDetailObj.Total = (decimal)tot;
                                            rmatransactionsDetailObj.TotalWeight = returnDetail.TotalWeight;
                                            rmatransactionsDetailObj.GlsalesAccount = originalItem.GlsalesAccount;
                                            rmatransactionsDetailObj.Glcogaccount = originalItem.Glcogaccount;
                                            rmatransactionsDetailObj.ProjectId = originalItem.ProjectId;
                                            rmatransactionsDetailObj.WarehouseBinId = originalItem.WarehouseBinId;
                                            rmatransactionsDetailObj.PalletLevel = returnDetail.PalletLevel;
                                            rmatransactionsDetailObj.CartonLevel = returnDetail.CartonLevel;
                                            rmatransactionsDetailObj.PackLevelA = returnDetail.PackLevelA;
                                            rmatransactionsDetailObj.PackLevelB = returnDetail.PackLevelB;
                                            rmatransactionsDetailObj.PackLevelC = returnDetail.PackLevelC;
                                            rmatransactionsDetailObj.TrackingNumber = originalItem.OrderNumber;
                                            rmatransactionsDetailObj.ScheduledStartDate = returnDetail.ScheduledStartDate;
                                            rmatransactionsDetailObj.ScheduledStartDate = returnDetail.ScheduledEndDate;
                                            rmatransactionsDetailObj.ServiceStartDate = returnDetail.ServiceStartDate;
                                            rmatransactionsDetailObj.ServiceEndDate = returnDetail.ServiceEndDate;
                                            rmatransactionsDetailObj.PerformedBy = returnDetail.PerformedBy;
                                            rmatransactionsDetailObj.DetailMemo1 = returnDetail.DetailMemo1;
                                            rmatransactionsDetailObj.DetailMemo2 = returnDetail.DetailMemo2;
                                            rmatransactionsDetailObj.DetailMemo3 = returnDetail.DetailMemo3;
                                            rmatransactionsDetailObj.DetailMemo4 = returnDetail.DetailMemo4;
                                            rmatransactionsDetailObj.DetailMemo5 = returnDetail.DetailMemo5;
                                            rmatransactionsDetailObj.LockedBy = returnDetail.LockedBy;
                                            rmatransactionsDetailObj.LockTs = returnDetail.LockTs;
                                            rmatransactionsDetailObj.ItemPricingCode = returnDetail.ItemPricingCode;
                                            rmatransactionsDetailObj.DeliveryNumber = returnDetail.DeliveryNumber;
                                            rmatransactionsDetailObj.GlanalysisType1 = returnDetail.GlanalysisType1;
                                            rmatransactionsDetailObj.GlanalysisType2 = returnDetail.GlanalysisType2;
                                            rmatransactionsDetailObj.AssetId = returnDetail.AssetId;
                                            rmatransactionsDetailObj.BudgetId = returnDetail.BudgetId;
                                            rmatransactionsDetailObj.MultipleDiscountGroupId = originalItem.MultipleDiscountGroupId;
                                            rmatransactionsDetailObj.MultipleDiscountAmount = originalItem.MultipleDiscountAmount;
                                            rmatransactionsDetailObj.MultipleDiscountPercent = originalItem.MultipleDiscountPercent;
                                            rmatransactionsDetailObj.DiscountAmount = originalItem.DiscountAmount;
                                            rmatransactionsDetailObj.MarkUponCost = originalItem.MarkUponCost;
                                            rmatransactionsDetailObj.MarkUpRate = originalItem.MarkUpRate;
                                            rmatransactionsDetailObj.ItemUnitCost = originalItem.ItemUnitCost;
                                            rmatransactionsDetailObj.InvoicedDate = originalItem.InvoicedDate;
                                            rmatransactionsDetailObj.BranchCode = originalItem.BranchCode;
                                            rmatransactionsDetailObj.ProductTypeId = originalItem.ProductTypeId;
                                            rmatransactionsDetailObj.AdvertTypeId = originalItem.AdvertTypeId;
                                            rmatransactionsDetailObj.UnAppliedTotal = returnDetail.UnAppliedTotal;

                                        }
                                        else
                                        {
                                            returnDetail.CompanyId = token.CompanyId;
                                            returnDetail.DivisionId = token.DivisionId;
                                            returnDetail.DepartmentId = token.DepartmentId;
                                            returnDetail.InvoiceNumber = InvoiceNumber;
                                            returnDetail.InvoiceLineNumber = 0;
                                            returnDetail.ItemId = returnDetail.ItemId;
                                            returnDetail.ItemUpccode = returnDetail.ItemUpccode;
                                            returnDetail.WarehouseId = originalItem.WarehouseId;
                                            returnDetail.SerialNumber = returnDetail.SerialNumber;
                                            returnDetail.OrderQty = returnDetail.OrderQty;
                                            returnDetail.BackOrdered = returnDetail.BackOrdered;
                                            returnDetail.BackOrderQty = returnDetail.BackOrderQty;
                                            returnDetail.ItemUom = returnDetail.ItemUom;
                                            returnDetail.ItemWeight = returnDetail.ItemWeight;
                                            returnDetail.Description = originalItem.Description;
                                            returnDetail.DiscountPerc = returnDetail.DiscountPerc;
                                            returnDetail.Taxable = originalItem.Taxable;
                                            returnDetail.CurrencyId = returnDetail.CurrencyId;
                                            returnDetail.CurrencyExchangeRate = returnDetail.CurrencyExchangeRate;
                                            returnDetail.ItemCost = returnDetail.ItemCost;
                                            returnDetail.ItemUnitPrice = (double)originalItem.ItemUnitPrice;
                                            returnDetail.TaxGroupId = originalItem.TaxGroupId;
                                            returnDetail.TaxAmount = returnDetail.TaxAmount;
                                            returnDetail.TaxPercent = originalItem.TaxPercent;
                                            returnDetail.SubTotal = (decimal)tot;
                                            returnDetail.Total = (decimal)tot;
                                            returnDetail.TotalWeight = returnDetail.TotalWeight;
                                            returnDetail.GlsalesAccount = originalItem.GlsalesAccount;
                                            returnDetail.Glcogaccount = originalItem.Glcogaccount;
                                            returnDetail.ProjectId = originalItem.ProjectId;
                                            returnDetail.WarehouseBinId = originalItem.WarehouseBinId;
                                            returnDetail.PalletLevel = returnDetail.PalletLevel;
                                            returnDetail.CartonLevel = returnDetail.CartonLevel;
                                            returnDetail.PackLevelA = returnDetail.PackLevelA;
                                            returnDetail.PackLevelB = returnDetail.PackLevelB;
                                            returnDetail.PackLevelC = returnDetail.PackLevelC;
                                            returnDetail.TrackingNumber = originalItem.OrderNumber;
                                            returnDetail.ScheduledStartDate = returnDetail.ScheduledStartDate;
                                            returnDetail.ScheduledStartDate = returnDetail.ScheduledEndDate;
                                            returnDetail.ServiceStartDate = returnDetail.ServiceStartDate;
                                            returnDetail.ServiceEndDate = returnDetail.ServiceEndDate;
                                            returnDetail.PerformedBy = returnDetail.PerformedBy;
                                            returnDetail.DetailMemo1 = returnDetail.DetailMemo1;
                                            returnDetail.DetailMemo2 = returnDetail.DetailMemo2;
                                            returnDetail.DetailMemo3 = returnDetail.DetailMemo3;
                                            returnDetail.DetailMemo4 = returnDetail.DetailMemo4;
                                            returnDetail.DetailMemo5 = returnDetail.DetailMemo5;
                                            returnDetail.LockedBy = returnDetail.LockedBy;
                                            returnDetail.LockTs = returnDetail.LockTs;
                                            returnDetail.ItemPricingCode = returnDetail.ItemPricingCode;
                                            returnDetail.DeliveryNumber = returnDetail.DeliveryNumber;
                                            returnDetail.GlanalysisType1 = returnDetail.GlanalysisType1;
                                            returnDetail.GlanalysisType2 = returnDetail.GlanalysisType2;
                                            returnDetail.AssetId = returnDetail.AssetId;
                                            returnDetail.BudgetId = returnDetail.BudgetId;
                                            returnDetail.MultipleDiscountGroupId = originalItem.MultipleDiscountGroupId;
                                            returnDetail.MultipleDiscountAmount = originalItem.MultipleDiscountAmount;
                                            returnDetail.MultipleDiscountPercent = originalItem.MultipleDiscountPercent;
                                            returnDetail.DiscountAmount = originalItem.DiscountAmount;
                                            returnDetail.MarkUponCost = originalItem.MarkUponCost;
                                            returnDetail.MarkUpRate = originalItem.MarkUpRate;
                                            returnDetail.ItemUnitCost = originalItem.ItemUnitCost;
                                            returnDetail.InvoicedDate = originalItem.InvoicedDate;
                                            returnDetail.BranchCode = originalItem.BranchCode;
                                            returnDetail.ProductTypeId = originalItem.ProductTypeId;
                                            returnDetail.AdvertTypeId = originalItem.AdvertTypeId;
                                            returnDetail.UnAppliedTotal = returnDetail.UnAppliedTotal;

                                            _DBContext.Entry(returnDetail).State = EntityState.Added;
                                            _DBContext.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "Invalid Item To Be Reversed";
                                    }

                                }
                            }

                            rmatransactionsHeader.Subtotal = (decimal)GrandTotal;
                            rmatransactionsHeader.Total = (decimal)GrandTotal;
                            _DBContext.Entry(rmatransactionsHeader).State = EntityState.Added;
                            _DBContext.SaveChanges();
                            //call RMA Recalc
                            await RMARecalc(InvoiceNumber, token);

                            statusMessage.Status = "Success";
                            statusMessage.Message = InvoiceNumber;
                            statusMessage.data = await GetRmaByInvoiceId(InvoiceNumber, token);


                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Return Merchandise Information";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invoice does not exist for this invoice or customer";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }

        public async Task<bool> InvoiceRMATransactions_Recalc(string InvoiceNumber, ApiToken token)
        {
            bool status = false;

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sInvoiceNumber = new SqlParameter("@InvoiceNumber", InvoiceNumber);
                var sEmployeeID = new SqlParameter("@EmployeeID", token.Username);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.InvoiceTransactions_Post @CompanyID, @DivisionID, @DepartmentID, @InvoiceNumber, @EmployeeID , @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sInvoiceNumber, sEmployeeID, PostingResult });

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<StatusMessage> RMARecalc(string Id, ApiToken tokenObj)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                bool status = await InvoiceRMATransactions_Recalc(Id, tokenObj);

                if (status)
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "RMA Successfully Booked";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "RMA Booking Failed.Try again or contact system administrator";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<bool> InvoiceRMATransactions_Post(string InvoiceNumber, ApiToken token)
        {
            bool status = false;

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sInvoiceNumber = new SqlParameter("@InvoiceNumber", InvoiceNumber);
                var sEmployeeID = new SqlParameter("@EmployeeID", token.Username);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.InvoiceTransactions_Post @CompanyID, @DivisionID, @DepartmentID, @InvoiceNumber, @EmployeeID , @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sInvoiceNumber, sEmployeeID, PostingResult });

                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<StatusMessage> RMAPost(string Id, ApiToken tokenObj)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                bool status = await InvoiceRMATransactions_Post(Id, tokenObj);

                if (status)
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "RMA Successfully Booked";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "RMA Booking Failed.Try again or contact system administrator";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<Paging> GetQuotes(PaginationParams Param, ApiToken token)
        {
            List<OrderRetrieveDto> orderHeaders = new List<OrderRetrieveDto>();
            try
            {
                var totalCount = await _DBContext.OrderHeader
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TransactionTypeId == "Quote")
                    .CountAsync();

                var quotess = await _DBContext.OrderHeader.OrderByDescending(x => x.OrderDate).Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TransactionTypeId == "Quote")
                                                        .OrderBy(x => x.InvoiceNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                orderHeaders = await order(quotess, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    QuoteList = orderHeaders
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<IEnumerable<OrderRetrieveDto>> GetQuotesByCustomerId(string customerId, ApiToken token)
        {
            List<Data.ViewModels.OrderRetrieveDto> orderHeaders = new List<Data.ViewModels.OrderRetrieveDto>();
            try
            {

                var quote = await _DBContext.OrderHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == customerId &&
                                                         x.TransactionTypeId == "Quote")
                                                         .OrderByDescending(x => x.OrderNumber).ToListAsync();
                orderHeaders = await order(quote, token);
            }
            catch (Exception ex)
            {

            }
            return orderHeaders;
        }

        public async Task<IEnumerable<OrderRetrieveDto>> GetQuotesById(string Id, ApiToken token)
        {
            List<OrderRetrieveDto> quotesdetail = new List<OrderRetrieveDto>();
            try
            {
                var Quotes = await _DBContext.OrderHeader.OrderByDescending(x => x.OrderDate).Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.OrderNumber == Id &&
                                                           x.TransactionTypeId == "Quote").ToListAsync();
                quotesdetail = await order(Quotes, token);
            }
            catch (Exception ex)
            {
                throw;
            }
            return quotesdetail;
        }

        public async Task<StatusMessage> AddQuotes(Order salesPolicy, ApiToken token)
        {
            double tot = 0.0;
            double GrandTotal = 0.0;

            StatusMessage statusMessage = new StatusMessage();

            List<OrderDetail> orderDetail = new List<OrderDetail>();
            Data.Models.CurrencyTypes currency = new Data.Models.CurrencyTypes();
            try
            {
                var company = await _DBContext.Companies.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId).FirstOrDefaultAsync();
                if (company != null)
                {
                    currency = await _DBContext.CurrencyTypes.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.CurrencyId == company.CurrencyId).FirstOrDefaultAsync();
                }
                //check if object empty
                if (salesPolicy != null)
                {
                    var quoteHead = await _DBContext.OrderHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == salesPolicy.OrderNumber &&
                                                        x.TransactionTypeId == "Quote").FirstOrDefaultAsync();

                    var customer = await _DBContext.CustomerInformation.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                    if (quoteHead != null) // for update sales policy
                    {

                        quoteHead.CompanyId = salesPolicy.CompanyId;
                        quoteHead.DivisionId = salesPolicy.DivisionId;
                        quoteHead.DepartmentId = salesPolicy.DepartmentId;
                        quoteHead.OrderNumber = salesPolicy.OrderNumber;
                        quoteHead.TransactionTypeId = "Quote";
                        quoteHead.OrderTypeId = "Quote";
                        quoteHead.OrderDate = salesPolicy.OrderDate;
                        quoteHead.OrderDueDate = salesPolicy.OrderDueDate;
                        quoteHead.OrderShipDate = salesPolicy.OrderShipDate;
                        quoteHead.OrderCancelDate = salesPolicy.OrderCancelDate;
                        quoteHead.SystemDate = salesPolicy.SystemDate;
                        quoteHead.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        quoteHead.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        quoteHead.Subtotal = salesPolicy.Subtotal;
                        quoteHead.DiscountPers = salesPolicy.DiscountPers;
                        quoteHead.DiscountAmount = salesPolicy.DiscountAmount;
                        quoteHead.TaxPercent = salesPolicy.TaxPercent;
                        quoteHead.TaxAmount = salesPolicy.TaxAmount;
                        quoteHead.TaxableSubTotal = salesPolicy.TaxableSubTotal;
                        quoteHead.Freight = salesPolicy.Freight;
                        quoteHead.TaxFreight = salesPolicy.TaxFreight;
                        quoteHead.Handling = salesPolicy.Handling;
                        quoteHead.Advertising = salesPolicy.Advertising;
                        quoteHead.Total = salesPolicy.Total;
                        quoteHead.ShipMethodId = salesPolicy.ShipMethodId;
                        quoteHead.ShipForId = salesPolicy.ShipForId;
                        quoteHead.ShipToId = salesPolicy.ShipToId;
                        quoteHead.ShippingName = customer.CustomerName;
                        quoteHead.ShippingAddress1 = customer.CustomerAddress1;
                        quoteHead.ShippingAddress2 = customer.CustomerAddress2;
                        quoteHead.ShippingAddress3 = customer.CustomerAddress3;
                        quoteHead.ShippingCity = customer.CustomerCity;
                        quoteHead.ShippingState = customer.CustomerState;
                        quoteHead.ShippingZip = salesPolicy.ShippingZip;
                        quoteHead.ShippingCountry = customer.CustomerCountry;
                        quoteHead.PaymentMethodId = salesPolicy.PaymentMethodId;
                        quoteHead.AmountPaid = salesPolicy.AmountPaid;
                        quoteHead.BalanceDue = salesPolicy.BalanceDue;
                        quoteHead.CheckNumber = salesPolicy.CheckNumber;
                        quoteHead.CheckDate = salesPolicy.CheckDate;
                        quoteHead.CreditCardTypeId = salesPolicy.CreditCardTypeId;
                        quoteHead.CreditCardName = salesPolicy.CreditCardName;
                        quoteHead.CreditCardNumber = salesPolicy.CreditCardNumber;
                        quoteHead.CreditCardCsvnumber = salesPolicy.CreditCardCsvnumber;
                        quoteHead.CreditCardExpDate = salesPolicy.CreditCardExpDate;
                        quoteHead.CreditCardBillToZip = salesPolicy.CreditCardBillToZip;
                        quoteHead.CreditCardValidationCode = salesPolicy.CreditCardValidationCode;
                        quoteHead.HeaderMemo12 = "Mobile Sales";
                        quoteHead.BankId = "CASH";
                        quoteHead.BranchCode = salesPolicy.BranchCode;

                        _DBContext.Entry(quoteHead).State = EntityState.Modified;

                        if (salesPolicy.orderDetail != null && salesPolicy.orderDetail.Count > 0)
                        {
                            foreach (OrderDetail quoDetail in salesPolicy.orderDetail)
                            {
                                var quoteDetailObj = await _DBContext.OrderDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == quoDetail.OrderNumber &&
                                                        x.OrderLineNumber == quoDetail.OrderLineNumber).FirstOrDefaultAsync();
                                var item = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == quoDetail.ItemId).FirstOrDefaultAsync();
                                if (item != null)
                                {
                                    if (quoteDetailObj != null)
                                    {
                                        quoteDetailObj.ItemId = quoDetail.ItemId;
                                        quoteDetailObj.WarehouseBinId = quoDetail.WarehouseBinId;
                                        quoteDetailObj.WarehouseId = quoDetail.WarehouseId;
                                        quoteDetailObj.Description = quoDetail.Description;
                                        quoteDetailObj.OrderQty = quoDetail.OrderQty;
                                        quoteDetailObj.DiscountPerc = quoDetail.DiscountPerc;
                                        quoteDetailObj.Taxable = quoDetail.Taxable;
                                        quoteDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                        quoteDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                        quoteDetailObj.ItemUnitPrice = quoDetail.ItemUnitPrice;
                                        quoteDetailObj.TaxGroupId = quoDetail.TaxGroupId;
                                        quoteDetailObj.TaxAmount = quoDetail.TaxAmount;
                                        quoteDetailObj.TaxPercent = quoDetail.TaxPercent;
                                        quoteDetailObj.SubTotal = quoDetail.SubTotal;
                                        quoteDetailObj.Total = quoDetail.Total;
                                        quoteDetailObj.TotalWeight = quoDetail.TotalWeight;
                                        quoteDetailObj.GlsalesAccount = quoDetail.GlsalesAccount;
                                        quoteDetailObj.Glcogaccount = quoDetail.Glcogaccount;
                                        quoteDetailObj.ProjectId = quoDetail.ProjectId;
                                        quoteDetailObj.TrackingNumber = quoDetail.TrackingNumber;
                                        quoteDetailObj.InvoicedDate = quoDetail.InvoicedDate;
                                        quoteDetailObj.InvoicedQty = quoDetail.OrderQty;
                                        quoteDetailObj.AssetId = quoDetail.AssetId;
                                        quoteDetailObj.MultipleDiscountAmount = quoDetail.MultipleDiscountAmount;
                                        quoteDetailObj.MultipleDiscountGroupId = quoDetail.MultipleDiscountGroupId;
                                        quoteDetailObj.MultipleDiscountPercent = quoDetail.MultipleDiscountPercent;
                                        quoteDetailObj.DiscountAmount = quoDetail.DiscountAmount;
                                        quoteDetailObj.MarkUponCost = quoDetail.MarkUponCost;
                                        quoteDetailObj.MarkUpRate = quoDetail.MarkUpRate;
                                        quoteDetailObj.ItemUnitCost = quoDetail.ItemUnitCost;
                                        quoteDetailObj.BranchCode = quoDetail.BranchCode;
                                        quoteDetailObj.ProductTypeId = quoDetail.ProductTypeId;
                                        quoteDetailObj.AdvertTypeId = quoDetail.ItemId;

                                        _DBContext.Entry(quoteDetailObj).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        quoDetail.CompanyId = token.CompanyId;
                                        quoDetail.DivisionId = token.DivisionId;
                                        quoDetail.DepartmentId = token.DepartmentId;

                                        _DBContext.Entry(quoDetail).State = EntityState.Added;
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Invalid Item ID";
                                }
                            }
                        }
                        if (salesPolicy.orderDetail != null && salesPolicy.orderDetail.Count > 0)
                        {
                            var Cust = await _DBContext.CustomerInformation.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                            var Comp = await _DBContext.Companies.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId).FirstOrDefaultAsync();
                            //x.WarehouseId == salesPolicy.WarehouseId


                            var inventoryItems = await GetItemsByCustomerId(salesPolicy.CustomerId, token);

                            foreach (OrderDetail quoDetail in salesPolicy.orderDetail)
                            {
                                var quoteDetailObj = await _DBContext.OrderDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == quoDetail.OrderNumber &&
                                                        x.ItemId == quoDetail.ItemId).FirstOrDefaultAsync();

                                var item = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == quoDetail.ItemId).FirstOrDefaultAsync();
                                // x.ItemDefaultWarehouse == quoteDetail.WarehouseId


                                if (item != null)
                                {
                                    var orderWarehouseBin = await _DBContext.WarehouseBins.Where(x => x.CompanyId == token.CompanyId &&
                                                                                              x.DepartmentId == token.DepartmentId &&
                                                                                              x.DivisionId == token.DivisionId &&
                                                                                              x.WarehouseId == quoDetail.WarehouseId &&
                                                                                              x.WarehouseBinId == quoDetail.WarehouseBinId).FirstOrDefaultAsync();

                                    string defaultWarehouseBin = "";

                                    string defaultWarehouse = "";

                                    if (!String.IsNullOrEmpty(Cust.WarehouseId))
                                    {
                                        defaultWarehouse = Cust.WarehouseId;

                                    }
                                    else if (!String.IsNullOrEmpty(item.ItemDefaultWarehouse))
                                    {
                                        defaultWarehouse = item.ItemDefaultWarehouse;
                                    }
                                    else if (!String.IsNullOrEmpty(Comp.WarehouseId))
                                    {
                                        defaultWarehouse = Comp.WarehouseId;

                                    }

                                    var orderWarehouse = await _DBContext.Warehouses.Where(x => x.CompanyId == token.CompanyId &&
                                                x.DepartmentId == token.DepartmentId &&
                                                x.DivisionId == token.DivisionId &&
                                                x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                    if (orderWarehouse != null)
                                    {
                                        var itemWarehouse = await _DBContext.InventoryItems.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.ItemId == quoDetail.ItemId &&
                                                     x.ItemDefaultWarehouse == defaultWarehouse).FirstOrDefaultAsync();

                                        if (itemWarehouse == null)
                                        {
                                            var Comps = await _DBContext.Companies.Where(x =>
                                                    x.CompanyId == token.CompanyId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                            if (Comps != null)
                                            {
                                                defaultWarehouseBin = Comps.WarehouseBinId;
                                            }
                                        }
                                        else if (itemWarehouse != null)
                                        {
                                            defaultWarehouseBin = itemWarehouse.ItemDefaultWarehouseBin;
                                        }
                                    }
                                    tot = await CalcSubTot(salesPolicy, token);
                                    GrandTotal += tot;

                                    if (quoteDetailObj != null)
                                    {
                                        quoteDetailObj.ItemId = quoDetail.ItemId;
                                        quoteDetailObj.ItemUpccode = quoDetail.ItemUpccode;
                                        quoteDetailObj.WarehouseBinId = defaultWarehouseBin;
                                        quoteDetailObj.WarehouseId = defaultWarehouse;
                                        //quoteDetailObj.SerialNumber = quoteDetail.SerialNumber;
                                        quoteDetailObj.Description = item.ItemName;
                                        quoteDetailObj.OrderQty = quoDetail.OrderQty;
                                        quoteDetailObj.BackOrdered = false;
                                        quoteDetailObj.BackOrderQyyty = quoDetail.BackOrderQyyty;
                                        quoteDetailObj.ItemUom = quoDetail.ItemUom;
                                        quoteDetailObj.ItemWeight = quoDetail.ItemWeight;
                                        quoteDetailObj.DiscountPerc = quoDetail.DiscountPerc;
                                        quoteDetailObj.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable); ;
                                        quoteDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                        quoteDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                        quoteDetailObj.ItemCost = quoDetail.ItemCost;
                                        quoteDetailObj.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == quoDetail.ItemId).FirstOrDefault().Price; ;
                                        quoteDetailObj.TaxGroupId = quoDetail.TaxGroupId;
                                        quoteDetailObj.TaxAmount = quoDetail.TaxAmount;
                                        quoteDetailObj.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        quoteDetailObj.SubTotal = (decimal)tot;
                                        quoteDetailObj.Total = (decimal)GrandTotal;
                                        quoteDetailObj.TotalWeight = quoDetail.TotalWeight;
                                        quoteDetailObj.GlsalesAccount = quoDetail.GlsalesAccount;
                                        quoteDetailObj.Glcogaccount = quoDetail.Glcogaccount;
                                        quoteDetailObj.ProjectId = quoDetail.ProjectId;
                                        quoteDetailObj.TrackingNumber = quoDetail.TrackingNumber;
                                        quoteDetailObj.WarehouseBinZone = quoDetail.WarehouseBinZone;
                                        quoteDetailObj.PalletLevel = quoDetail.PalletLevel;
                                        quoteDetailObj.CartonLevel = quoDetail.CartonLevel;
                                        quoteDetailObj.PackLevelA = quoDetail.PackLevelA;
                                        quoteDetailObj.PackLevelB = quoDetail.PackLevelB;
                                        quoteDetailObj.PackLevelC = quoDetail.PackLevelC;
                                        quoteDetailObj.ScheduledStartDate = quoDetail.ScheduledStartDate;
                                        quoteDetailObj.ScheduledEndDate = quoDetail.ScheduledEndDate;
                                        quoteDetailObj.ServiceStartDate = quoDetail.ServiceStartDate;
                                        quoteDetailObj.ServiceEndDate = quoDetail.ServiceEndDate;
                                        quoteDetailObj.PerformedBy = quoDetail.PerformedBy;
                                        quoteDetailObj.DetailMemo1 = quoDetail.DetailMemo1;
                                        quoteDetailObj.DetailMemo2 = quoDetail.DetailMemo2;
                                        quoteDetailObj.DetailMemo3 = quoDetail.DetailMemo3;
                                        quoteDetailObj.DetailMemo4 = quoDetail.DetailMemo4;
                                        quoteDetailObj.DetailMemo5 = quoDetail.ItemId;
                                        quoteDetailObj.DetailMemo5 = quoDetail.DetailMemo5;
                                        quoteDetailObj.LockedBy = quoDetail.LockedBy;
                                        quoteDetailObj.LockTs = quoDetail.LockTs;
                                        quoteDetailObj.Invoiced = false;
                                        quoteDetailObj.InvoicedDate = quoDetail.InvoicedDate;
                                        quoteDetailObj.InvoicedQty = quoDetail.OrderQty;
                                        quoteDetailObj.DeliveryNumber = quoDetail.DeliveryNumber;
                                        quoteDetailObj.GlanalysisType1 = quoDetail.GlanalysisType1;
                                        quoteDetailObj.GlanalysisType2 = quoDetail.GlanalysisType2;
                                        quoteDetailObj.AssetId = quoDetail.AssetId;
                                        quoteDetailObj.MultipleDiscountAmount = quoDetail.MultipleDiscountAmount;
                                        quoteDetailObj.MultipleDiscountGroupId = quoDetail.MultipleDiscountGroupId;
                                        quoteDetailObj.MultipleDiscountPercent = quoDetail.MultipleDiscountPercent;
                                        quoteDetailObj.DiscountAmount = quoDetail.DiscountAmount;
                                        quoteDetailObj.MarkUponCost = quoDetail.MarkUponCost;
                                        quoteDetailObj.MarkUpRate = quoDetail.MarkUpRate;
                                        quoteDetailObj.ItemUnitCost = quoDetail.ItemUnitCost;
                                        quoteDetailObj.BranchCode = quoDetail.BranchCode;
                                        quoteDetailObj.ProductTypeId = quoDetail.ProductTypeId;
                                        quoteDetailObj.AdvertTypeId = quoDetail.ItemId;
                                        quoteDetailObj.BackOrderBooked = false;
                                        quoteDetailObj.BackOrderBookedBy = quoDetail.BackOrderBookedBy;
                                        quoteDetailObj.BackOrderBookedDate = quoDetail.BackOrderBookedDate;

                                        _DBContext.Entry(quoteDetailObj).State = EntityState.Modified;
                                        _DBContext.SaveChanges();
                                    }
                                    else
                                    {
                                        quoDetail.CompanyId = token.CompanyId;
                                        quoDetail.DivisionId = token.DivisionId;
                                        quoDetail.DepartmentId = token.DepartmentId;
                                        quoDetail.WarehouseId = defaultWarehouse;
                                        quoDetail.WarehouseBinId = defaultWarehouseBin;
                                        quoDetail.Description = item.ItemName;
                                        quoDetail.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);
                                        quoDetail.TaxGroupId = item.TaxGroupId;
                                        quoDetail.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        quoDetail.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == quoDetail.ItemId).FirstOrDefault().Price;
                                        quoDetail.SubTotal = (decimal)tot;
                                        quoDetail.Total = (decimal)GrandTotal;
                                        quoDetail.Invoiced = false;
                                        quoDetail.BackOrderBooked = false;
                                        quoDetail.BackOrdered = false;

                                        _DBContext.Entry(quoDetail).State = EntityState.Added;
                                        _DBContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Invalid Item ID";
                                }
                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create sales policy
                    {
                        OrderHeader quoteHeader = new OrderHeader();

                        string orderNumber = await getNextEntityID(_NextNumberName, token);
                        string BankId = "CASH";
                        string HeaderMemo12 = "Mobile Sale";

                        quoteHeader.CompanyId = token.CompanyId;
                        quoteHeader.DivisionId = token.DivisionId;
                        quoteHeader.DepartmentId = token.DepartmentId;
                        quoteHeader.OrderNumber = orderNumber;
                        quoteHeader.TransactionTypeId = "Quote";
                        quoteHeader.OrderTypeId = "Quote";
                        quoteHeader.OrderDate = DateTime.Now;
                        quoteHeader.OrderDueDate = salesPolicy.OrderDueDate;
                        quoteHeader.OrderShipDate = salesPolicy.OrderShipDate;
                        quoteHeader.OrderCancelDate = salesPolicy.OrderCancelDate;
                        quoteHeader.SystemDate = DateTime.Now;
                        quoteHeader.Memorize = false;
                        quoteHeader.PurchaseOrderNumber = salesPolicy.PurchaseOrderNumber;
                        quoteHeader.TaxExemptId = salesPolicy.TaxExemptId;
                        quoteHeader.TaxGroupId = salesPolicy.TaxGroupId;
                        quoteHeader.CustomerId = salesPolicy.CustomerId;
                        quoteHeader.TermsId = salesPolicy.TermsId;
                        quoteHeader.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                        quoteHeader.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                        quoteHeader.Subtotal = salesPolicy.Subtotal;
                        quoteHeader.DiscountPers = salesPolicy.DiscountPers;
                        quoteHeader.DiscountAmount = salesPolicy.DiscountAmount;
                        quoteHeader.TaxPercent = salesPolicy.TaxPercent;
                        quoteHeader.TaxAmount = salesPolicy.TaxAmount;
                        quoteHeader.TaxableSubTotal = salesPolicy.TaxableSubTotal;
                        quoteHeader.Freight = salesPolicy.Freight;
                        quoteHeader.TaxFreight = false;
                        quoteHeader.Handling = salesPolicy.Handling;
                        quoteHeader.Advertising = salesPolicy.Advertising;
                        quoteHeader.Total = salesPolicy.Total;
                        quoteHeader.EmployeeId = salesPolicy.EmployeeId;
                        quoteHeader.Commission = salesPolicy.Commission;
                        quoteHeader.CommissionableSales = salesPolicy.CommissionableSales;
                        quoteHeader.ComissionalbleCost = salesPolicy.ComissionalbleCost;
                        quoteHeader.CustomerDropShipment = false;
                        quoteHeader.ShipMethodId = salesPolicy.ShipMethodId;
                        quoteHeader.WarehouseId = salesPolicy.WarehouseId;
                        quoteHeader.ShipForId = salesPolicy.ShipForId;
                        quoteHeader.ShipToId = salesPolicy.ShipToId;
                        quoteHeader.ShippingName = customer.CustomerName;
                        quoteHeader.ShippingAddress1 = customer.CustomerAddress1;
                        quoteHeader.ShippingAddress2 = customer.CustomerAddress2;
                        quoteHeader.ShippingAddress3 = customer.CustomerAddress3;
                        quoteHeader.ShippingCity = customer.CustomerCity;
                        quoteHeader.ShippingState = customer.CustomerState;
                        quoteHeader.ShippingZip = salesPolicy.ShippingZip;
                        quoteHeader.ShippingCountry = customer.CustomerCountry;
                        //quoteHeader.ScheduledStartDate = salesPolicy.ScheduledStartDate;
                        //quoteHeader.ScheduledEndDate = salesPolicy.ScheduledEndDate;
                        quoteHeader.ServiceStartDate = salesPolicy.ServiceStartDate;
                        quoteHeader.ServiceEndDate = salesPolicy.ServiceEndDate;
                        quoteHeader.PerformedBy = salesPolicy.PerformedBy;
                        quoteHeader.GlsalesAccount = salesPolicy.GlsalesAccount;
                        quoteHeader.Glcogaccount = salesPolicy.Glcogaccount;
                        quoteHeader.PaymentMethodId = salesPolicy.PaymentMethodId;
                        quoteHeader.AmountPaid = salesPolicy.AmountPaid;
                        quoteHeader.BalanceDue = salesPolicy.BalanceDue;
                        quoteHeader.UndistributedAmount = salesPolicy.UndistributedAmount;
                        quoteHeader.CheckNumber = salesPolicy.CheckNumber;
                        quoteHeader.CheckDate = salesPolicy.CheckDate;
                        quoteHeader.CreditCardTypeId = salesPolicy.CreditCardTypeId;
                        quoteHeader.CreditCardName = salesPolicy.CreditCardName;
                        quoteHeader.CreditCardNumber = salesPolicy.CreditCardNumber;
                        quoteHeader.CreditCardCsvnumber = salesPolicy.CreditCardCsvnumber;
                        quoteHeader.CreditCardExpDate = salesPolicy.CreditCardExpDate;
                        quoteHeader.CreditCardBillToZip = salesPolicy.CreditCardBillToZip;
                        quoteHeader.CreditCardValidationCode = salesPolicy.CreditCardValidationCode;
                        quoteHeader.Backordered = false;
                        quoteHeader.Picked = false;
                        quoteHeader.PickedDate = salesPolicy.PickedDate;
                        quoteHeader.Printed = false;
                        quoteHeader.PrintedDate = salesPolicy.PrintedDate;
                        quoteHeader.Shipped = false;
                        quoteHeader.ShipDate = salesPolicy.ShipDate;
                        quoteHeader.TrackingNumber = salesPolicy.TrackingNumber;
                        quoteHeader.Billed = false;
                        quoteHeader.Invoiced = false;
                        quoteHeader.InvoiceDate = salesPolicy.InvoiceDate;
                        quoteHeader.InvoiceNumber = salesPolicy.InvoiceNumber;
                        quoteHeader.Posted = false;
                        quoteHeader.PostedDate = salesPolicy.PostedDate;
                        quoteHeader.AllowanceDiscountPerc = salesPolicy.AllowanceDiscountPerc;
                        quoteHeader.CashTendered = salesPolicy.CashTendered;
                        quoteHeader.MasterBillOfLading = salesPolicy.MasterBillOfLading;
                        quoteHeader.MasterBillOfLadingDate = salesPolicy.MasterBillOfLadingDate;
                        quoteHeader.TrailerNumber = salesPolicy.TrailerNumber;
                        quoteHeader.TrailerPrefix = salesPolicy.TrailerPrefix;
                        quoteHeader.HeaderMemo1 = salesPolicy.HeaderMemo1;
                        quoteHeader.HeaderMemo2 = salesPolicy.HeaderMemo2;
                        quoteHeader.HeaderMemo3 = salesPolicy.HeaderMemo3;
                        quoteHeader.HeaderMemo4 = salesPolicy.HeaderMemo4;
                        quoteHeader.HeaderMemo5 = salesPolicy.HeaderMemo5;
                        quoteHeader.HeaderMemo6 = salesPolicy.HeaderMemo6;
                        quoteHeader.HeaderMemo7 = salesPolicy.HeaderMemo7;
                        quoteHeader.HeaderMemo8 = salesPolicy.HeaderMemo8;
                        quoteHeader.HeaderMemo9 = salesPolicy.HeaderMemo9;
                        quoteHeader.HeaderMemo10 = salesPolicy.HeaderMemo10;
                        quoteHeader.HeaderMemo11 = salesPolicy.HeaderMemo11;
                        quoteHeader.HeaderMemo12 = HeaderMemo12;
                        quoteHeader.Approved = false;
                        quoteHeader.ApprovedBy = salesPolicy.ApprovedBy;
                        quoteHeader.ApprovedDate = salesPolicy.ApprovedDate;
                        quoteHeader.Signature = salesPolicy.Signature;
                        quoteHeader.SignaturePassword = salesPolicy.SignaturePassword;
                        quoteHeader.SupervisorPassword = salesPolicy.SupervisorPassword;
                        quoteHeader.SupervisorSignature = salesPolicy.SupervisorSignature;
                        quoteHeader.ManagerSignature = salesPolicy.ManagerSignature;
                        quoteHeader.ManagerPassword = salesPolicy.ManagerPassword;
                        quoteHeader.LockedBy = salesPolicy.LockedBy;
                        quoteHeader.LockTs = salesPolicy.LockTs;
                        quoteHeader.BankId = BankId;
                        quoteHeader.OriginalOrderNumber = salesPolicy.OriginalOrderNumber;
                        quoteHeader.OriginalOrderDate = salesPolicy.OriginalOrderDate;
                        quoteHeader.DeliveryNumber = salesPolicy.DeliveryNumber;
                        //quoteHeader.Ullage1 = salesPolicy.Ullage1;
                        //quoteHeader.Ullage2 = salesPolicy.Ullage2;
                        //quoteHeader.Ullage3 = salesPolicy.Ullage3;
                        //quoteHeader.Ullage4 = salesPolicy.Ullage4;
                        //quoteHeader.Ullage5 = salesPolicy.Ullage5;
                        //quoteHeader.Ullage6 = salesPolicy.Ullage6;
                        //quoteHeader.Ullage7 = salesPolicy.Ullage7;
                        //quoteHeader.Ullage8 = salesPolicy.Ullage8;
                        //quoteHeader.Ullage9 = salesPolicy.Ullage9;
                        //quoteHeader.Ullage10 = salesPolicy.Ullage10;
                        //quoteHeader.Ullage11 = salesPolicy.Ullage11;
                        //quoteHeader.Ullage12 = salesPolicy.Ullage12;
                        quoteHeader.BranchCode = salesPolicy.BranchCode;
                        quoteHeader.Merged = false; // salesPolicy.Merged;
                        quoteHeader.Created = false;// salesPolicy.Created;
                        //quoteHeader.FinanceApproved = false; //salesPolicy.FinanceApproved;
                        //quoteHeader.FinanceApprovedDate = salesPolicy.FinanceApprovedDate;
                        //quoteHeader.FinanceComment = salesPolicy.FinanceComment;
                        //quoteHeader.FinanceReturnedDate = salesPolicy.FinanceReturnedDate;
                        //quoteHeader.Bdmapproved = false; // salesPolicy.Bdmapproved;
                        //quoteHeader.BdmapprovedDate = salesPolicy.BdmapprovedDate;
                        //quoteHeader.Bdmcomment = salesPolicy.Bdmcomment;
                        //quoteHeader.Fmapproved = false; // salesPolicy.Fmapproved;
                        //quoteHeader.FmapprovedDate = salesPolicy.FmapprovedDate;
                        //quoteHeader.Fmcomment = salesPolicy.Fmcomment;
                        //quoteHeader.Mdapproved = false; // salesPolicy.Mdapproved;
                        //quoteHeader.MdapprovedDate = salesPolicy.MdapprovedDate;
                        //quoteHeader.Mdcomment = salesPolicy.Mdcomment;
                        //quoteHeader.Regularized = false;
                        //quoteHeader.Fmvoid = false;
                        //quoteHeader.FmvoidedDate = salesPolicy.FmvoidedDate;
                        //quoteHeader.ReceiptId = salesPolicy.ReceiptId;
                        //quoteHeader.CommercialComment = salesPolicy.CommercialComment;
                        //quoteHeader.FinanceApprovedBy = salesPolicy.FinanceApprovedBy;
                        //quoteHeader.FmapprovedBy = salesPolicy.FmapprovedBy;
                        //quoteHeader.CooapprovedBy = salesPolicy.CooapprovedBy;


                        _DBContext.Entry(quoteHeader).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (salesPolicy.orderDetail != null && salesPolicy.orderDetail.Count > 0)
                        {
                            var Cust = await _DBContext.CustomerInformation.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.CustomerId == salesPolicy.CustomerId).FirstOrDefaultAsync();

                            var Comp = await _DBContext.Companies.Where(x =>
                                                       x.CompanyId == token.CompanyId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.DivisionId == token.DivisionId).FirstOrDefaultAsync();
                            //x.WarehouseId == salesPolicy.WarehouseId


                            var inventoryItems = await GetItemsByCustomerId(salesPolicy.CustomerId, token);

                            foreach (OrderDetail quoDetail in salesPolicy.orderDetail)
                            {
                                var quoteDetailObj = await _DBContext.OrderDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OrderNumber == quoDetail.OrderNumber &&
                                                        x.ItemId == quoDetail.ItemId).FirstOrDefaultAsync();

                                var item = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == quoDetail.ItemId).FirstOrDefaultAsync();
                                // x.ItemDefaultWarehouse == quoteDetail.WarehouseId


                                if (item != null)
                                {
                                    var orderWarehouseBin = await _DBContext.WarehouseBins.Where(x => x.CompanyId == token.CompanyId &&
                                                                                              x.DepartmentId == token.DepartmentId &&
                                                                                              x.DivisionId == token.DivisionId &&
                                                                                              x.WarehouseId == quoDetail.WarehouseId &&
                                                                                              x.WarehouseBinId == quoDetail.WarehouseBinId).FirstOrDefaultAsync();

                                    string defaultWarehouseBin = "";

                                    string defaultWarehouse = "";

                                    if (!String.IsNullOrEmpty(Cust.WarehouseId))
                                    {
                                        defaultWarehouse = Cust.WarehouseId;

                                    }
                                    else if (!String.IsNullOrEmpty(item.ItemDefaultWarehouse))
                                    {
                                        defaultWarehouse = item.ItemDefaultWarehouse;
                                    }
                                    else if (!String.IsNullOrEmpty(Comp.WarehouseId))
                                    {
                                        defaultWarehouse = Comp.WarehouseId;

                                    }

                                    var orderWarehouse = await _DBContext.Warehouses.Where(x => x.CompanyId == token.CompanyId &&
                                                x.DepartmentId == token.DepartmentId &&
                                                x.DivisionId == token.DivisionId &&
                                                x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                    if (orderWarehouse != null)
                                    {
                                        var itemWarehouse = await _DBContext.InventoryItems.Where(x =>
                                                     x.CompanyId == token.CompanyId &&
                                                     x.DivisionId == token.DivisionId &&
                                                     x.DepartmentId == token.DepartmentId &&
                                                     x.ItemId == quoDetail.ItemId &&
                                                     x.ItemDefaultWarehouse == defaultWarehouse).FirstOrDefaultAsync();

                                        if (itemWarehouse == null)
                                        {
                                            var Comps = await _DBContext.Companies.Where(x =>
                                                    x.CompanyId == token.CompanyId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.WarehouseId == defaultWarehouse).FirstOrDefaultAsync();
                                            if (Comps != null)
                                            {
                                                defaultWarehouseBin = Comps.WarehouseBinId;
                                            }
                                        }
                                        else if (itemWarehouse != null)
                                        {
                                            defaultWarehouseBin = itemWarehouse.ItemDefaultWarehouseBin;
                                        }
                                    }
                                    tot = await CalcSubTot(salesPolicy, token);
                                    GrandTotal += tot;

                                    if (quoteDetailObj != null)
                                    {
                                        quoteDetailObj.ItemId = quoDetail.ItemId;
                                        quoteDetailObj.ItemUpccode = quoDetail.ItemUpccode;
                                        quoteDetailObj.WarehouseBinId = defaultWarehouseBin;
                                        quoteDetailObj.WarehouseId = defaultWarehouse;
                                        //quoteDetailObj.SerialNumber = quoteDetail.SerialNumber;
                                        quoteDetailObj.Description = item.ItemName;
                                        quoteDetailObj.OrderQty = quoDetail.OrderQty;
                                        quoteDetailObj.BackOrdered = false;
                                        quoteDetailObj.BackOrderQyyty = quoDetail.BackOrderQyyty;
                                        quoteDetailObj.ItemUom = quoDetail.ItemUom;
                                        quoteDetailObj.ItemWeight = quoDetail.ItemWeight;
                                        quoteDetailObj.DiscountPerc = quoDetail.DiscountPerc;
                                        quoteDetailObj.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable); ;
                                        quoteDetailObj.CurrencyId = currency != null ? currency.CurrencyId : "NGN";
                                        quoteDetailObj.CurrencyExchangeRate = currency != null ? currency.CurrencyExchangeRate : 1;
                                        quoteDetailObj.ItemCost = quoDetail.ItemCost;
                                        quoteDetailObj.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == quoDetail.ItemId).FirstOrDefault().Price; ;
                                        quoteDetailObj.TaxGroupId = quoDetail.TaxGroupId;
                                        quoteDetailObj.TaxAmount = quoDetail.TaxAmount;
                                        quoteDetailObj.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        quoteDetailObj.SubTotal = (decimal)tot;
                                        quoteDetailObj.Total = (decimal)GrandTotal;
                                        quoteDetailObj.TotalWeight = quoDetail.TotalWeight;
                                        quoteDetailObj.GlsalesAccount = quoDetail.GlsalesAccount;
                                        quoteDetailObj.Glcogaccount = quoDetail.Glcogaccount;
                                        quoteDetailObj.ProjectId = quoDetail.ProjectId;
                                        quoteDetailObj.TrackingNumber = quoDetail.TrackingNumber;
                                        quoteDetailObj.WarehouseBinZone = quoDetail.WarehouseBinZone;
                                        quoteDetailObj.PalletLevel = quoDetail.PalletLevel;
                                        quoteDetailObj.CartonLevel = quoDetail.CartonLevel;
                                        quoteDetailObj.PackLevelA = quoDetail.PackLevelA;
                                        quoteDetailObj.PackLevelB = quoDetail.PackLevelB;
                                        quoteDetailObj.PackLevelC = quoDetail.PackLevelC;
                                        quoteDetailObj.ScheduledStartDate = quoDetail.ScheduledStartDate;
                                        quoteDetailObj.ScheduledEndDate = quoDetail.ScheduledEndDate;
                                        quoteDetailObj.ServiceStartDate = quoDetail.ServiceStartDate;
                                        quoteDetailObj.ServiceEndDate = quoDetail.ServiceEndDate;
                                        quoteDetailObj.PerformedBy = quoDetail.PerformedBy;
                                        quoteDetailObj.DetailMemo1 = quoDetail.DetailMemo1;
                                        quoteDetailObj.DetailMemo2 = quoDetail.DetailMemo2;
                                        quoteDetailObj.DetailMemo3 = quoDetail.DetailMemo3;
                                        quoteDetailObj.DetailMemo4 = quoDetail.DetailMemo4;
                                        quoteDetailObj.DetailMemo5 = quoDetail.ItemId;
                                        quoteDetailObj.DetailMemo5 = quoDetail.DetailMemo5;
                                        quoteDetailObj.LockedBy = quoDetail.LockedBy;
                                        quoteDetailObj.LockTs = quoDetail.LockTs;
                                        quoteDetailObj.Invoiced = false;
                                        quoteDetailObj.InvoicedDate = quoDetail.InvoicedDate;
                                        quoteDetailObj.InvoicedQty = quoDetail.OrderQty;
                                        quoteDetailObj.DeliveryNumber = quoDetail.DeliveryNumber;
                                        quoteDetailObj.GlanalysisType1 = quoDetail.GlanalysisType1;
                                        quoteDetailObj.GlanalysisType2 = quoDetail.GlanalysisType2;
                                        quoteDetailObj.AssetId = quoDetail.AssetId;
                                        quoteDetailObj.MultipleDiscountAmount = quoDetail.MultipleDiscountAmount;
                                        quoteDetailObj.MultipleDiscountGroupId = quoDetail.MultipleDiscountGroupId;
                                        quoteDetailObj.MultipleDiscountPercent = quoDetail.MultipleDiscountPercent;
                                        quoteDetailObj.DiscountAmount = quoDetail.DiscountAmount;
                                        quoteDetailObj.MarkUponCost = quoDetail.MarkUponCost;
                                        quoteDetailObj.MarkUpRate = quoDetail.MarkUpRate;
                                        quoteDetailObj.ItemUnitCost = quoDetail.ItemUnitCost;
                                        quoteDetailObj.BranchCode = quoDetail.BranchCode;
                                        quoteDetailObj.ProductTypeId = quoDetail.ProductTypeId;
                                        quoteDetailObj.AdvertTypeId = quoDetail.ItemId;
                                        quoteDetailObj.BackOrderBooked = false;
                                        quoteDetailObj.BackOrderBookedBy = quoDetail.BackOrderBookedBy;
                                        quoteDetailObj.BackOrderBookedDate = quoDetail.BackOrderBookedDate;

                                        _DBContext.Entry(quoteDetailObj).State = EntityState.Modified;
                                        _DBContext.SaveChanges();
                                    }
                                    else
                                    {
                                        quoDetail.CompanyId = token.CompanyId;
                                        quoDetail.DivisionId = token.DivisionId;
                                        quoDetail.DepartmentId = token.DepartmentId;
                                        quoDetail.OrderNumber = orderNumber;
                                        quoDetail.OrderLineNumber = 0;
                                        quoDetail.WarehouseId = defaultWarehouse;
                                        quoDetail.WarehouseBinId = defaultWarehouseBin;
                                        quoDetail.Description = item.ItemName;
                                        quoDetail.Taxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);
                                        quoDetail.TaxGroupId = item.TaxGroupId;
                                        quoDetail.TaxPercent = await getTotalTaxPercent(item.TaxGroupId, token);
                                        quoDetail.ItemUnitPrice = (decimal)inventoryItems.Where(x => x.ItemId == quoDetail.ItemId).FirstOrDefault().Price;
                                        quoDetail.SubTotal = (decimal)tot;
                                        quoDetail.Total = (decimal)GrandTotal;
                                        quoDetail.Invoiced = false;
                                        quoDetail.BackOrderBooked = false;
                                        quoDetail.BackOrdered = false;

                                        _DBContext.Entry(quoDetail).State = EntityState.Added;
                                        _DBContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Invalid Item ID";
                                }
                            }
                        }

                        statusMessage.Status = "Success";
                        statusMessage.Message = orderNumber;
                        statusMessage.data = await GetQuotesById(orderNumber, token);
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Order Information";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<Data.Models.InventoryItems>> GetInventoryItemByName(string itemName, ApiToken token)
        {
            List<Data.Models.InventoryItems> Item = new List<Data.Models.InventoryItems>();
            try
            {
                Item = await _DBContext.InventoryItems.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.ItemName.Contains(itemName)).ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return Item;
        }

        public async Task<Warehouses> GetWarehouseById(string Id, ApiToken token)
        {
            Warehouses Ware = new Warehouses();
            try
            {
                Ware = await _DBContext.Warehouses.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.WarehouseId == Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

            }
            return Ware;

        }

        public async Task<WarehouseBins> GetWarehouseByBinId(string Id, ApiToken token)
        {
            WarehouseBins Ware = new WarehouseBins();
            try
            {
                Ware = await _DBContext.WarehouseBins.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.WarehouseBinId == Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

            }
            return Ware;

        }

        public async Task<IEnumerable<Data.Models.InventoryItems>> GetItemsByCustomerId(string Id, ApiToken token)
        {
            List<Data.Models.InventoryItems> salesItems = new List<Data.Models.InventoryItems>();
            List<InventoryCustomerPricingCode> customerPrice = new List<InventoryCustomerPricingCode>();

            try
            {
                var salesItemsObj = await _DBContext.InventoryItems.Where(x => x.CompanyId == token.CompanyId &&
                                                                      x.DivisionId == token.DivisionId &&
                                                                      x.DepartmentId == token.DepartmentId &&
                                                                      x.AllowSalesTrans == true &&
                                                                      x.IsActive == true).ToListAsync();

                customerPrice = await _DBContext.InventoryCustomerPricingCode.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.CustomerId == Id).ToListAsync();
                if (customerPrice.Count > 0)
                {
                    foreach (InventoryCustomerPricingCode customerPricingCode in customerPrice)
                    {
                        //var itemToUpdate = salesItemsObj.Where(x => x.ItemId == customerPricingCode.ItemId).FirstOrDefault();

                        //itemToUpdate.Price = customerPricingCode.Price;

                        foreach (Data.Models.InventoryItems item in salesItemsObj)
                        {
                            if (item.ItemId == customerPricingCode.ItemId)
                            {
                                item.Price = customerPricingCode.Price;
                            }

                            var itemExists = salesItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault();

                            if (itemExists == null)
                            {
                                salesItems.Add(item);
                            }
                        }

                    }
                }
                else
                {
                    salesItems = salesItemsObj;
                }
            }
            catch (Exception ex)
            {

            }
            return salesItems;
        }

        public async Task<CompanyKeys> GetPaymentToken(ApiToken token)
        {
            CompanyKeys key = new CompanyKeys();
            try
            {
                key = await _DBContext.CompanyKeys.Where(x => x.CompanyId == token.CompanyId &&
                                                                     x.DivisionId == token.DivisionId &&
                                                                     x.DepartmentId == token.DepartmentId &&
                                                                     x.ProfileType == "Paystack" &&
                                                                     x.Active == true).FirstOrDefaultAsync();


            }
            catch (Exception ex)
            {

            }
            return key;

        }

        public async Task<double> getTotalTaxPercent(string TaxGroupID, ApiToken token)
        {
            double dTaxPercent = 0;

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sTaxGroupID = new SqlParameter("@TaxGroupID", TaxGroupID);
                var TotalPercent = new SqlParameter("@TotalPercent", SqlDbType.Float);
                TotalPercent.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.TaxGroup_GetTotalPercent @CompanyID, @DivisionID, @DepartmentID, @TaxGroupID, @TotalPercent Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sTaxGroupID, TotalPercent });

                dTaxPercent = Convert.ToDouble(TotalPercent.Value);
            }
            catch (Exception ex)
            {

            }
            return dTaxPercent;
        }

        public async Task<OrderSummary> SummaryOfOrder(Order order, ApiToken token)
        {
            OrderSummary summary = new OrderSummary();
            try
            {
                var Total = 0.0;
                var FinalTotal = 0.0;
                var Subtotal = 0.0;
                var RebateTotal = 0.0;
                var TaxAmount = 0.0;


                var inventoryItems = await GetItemsByCustomerId(order.CustomerId, token);
                var cus = await GetCustomerById(order.CustomerId, token);

                //var Final;
                foreach (OrderDetail orderDetailObj in order.orderDetail)
                {

                    //double price = (double)inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault().Price;
                    var item = inventoryItems.Where(x => x.ItemId == orderDetailObj.ItemId).FirstOrDefault();

                    double price = (double)item.Price;
                    var itemTotal = (double)orderDetailObj.OrderQty * price;

                    Subtotal += itemTotal;

                    if (cus.ApplyRebate == true)
                    {
                        var Rebate = (double)(cus.RebateAmount * 0.01) * itemTotal;
                        itemTotal = (double)(itemTotal - Rebate);
                        RebateTotal += Rebate;
                    }

                    bool isTaxable = item.Taxable == null ? false : Convert.ToBoolean(item.Taxable);

                    if (isTaxable)
                    {
                        //call stored procedure to get tax percent;
                        double taxPercent = await getTotalTaxPercent(item.TaxGroupId, token);

                        double itemTaxAmount = (itemTotal * taxPercent * 0.01);
                        //apply tax on item total
                        TaxAmount += itemTaxAmount;
                        itemTotal = itemTotal + itemTaxAmount;

                    }


                    double ItemTotal = (double)itemTotal;
                    Total += ItemTotal;
                }

                double availCred = ((double)(cus.customerFinancials.AvailibleCredit ?? 0));
                if (availCred >= Total)
                {
                    //set payable amount to zero - the customer do not need to pay
                    FinalTotal = 0;
                }
                else //compute paystack commission
                {

                    FinalTotal = Total - availCred;
                }

                summary.CompanyId = cus.CompanyId;
                summary.DepartmentId = cus.DepartmentId;
                summary.DivisionId = cus.DivisionId;
                summary.CustomerId = cus.CustomerId;
                summary.CustomerName = cus.CustomerName;
                summary.Subtotal = Subtotal;
                summary.AmountToPay = FinalTotal;
                summary.Total = Total;
                summary.TaxAmount = TaxAmount;
                summary.AvailableCredit = availCred;
                summary.CustomerEmail = cus.CustomerEmail;
                // summary.TotalOrderCount = ;
                summary.DiscountAmount = RebateTotal;
            }
            catch
            {

            }
            return summary;
        }



    }
}
