using DevExpress.XtraCharts.Sankey;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Helper;
using PowerAPI.Service.Clients;
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
    [Authorize]
    [ApiController]
    public class SalesController : ControllerBase
    {
        ISales _sales;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Setup Constructor
        /// </summary>
        /// <param name="sales"></param>
        /// <param name="appSettings"></param>
        public SalesController(ISales sales, IOptions<AppSettings> appSettings)
        {
            _sales = sales;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// gets a list of order records
        /// </summary>
        [Route("api/GetAllOrders/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery]PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orders = await _sales.GetAllOrders(Param, tokenObj);

                        statusMessage.Metadata = orders.PaginationMetadata;
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = orders.OrderList;
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
        /// gets Order record by Order Id
        /// </summary>
        [Route("api/GetOrderById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderById(string OrderNumber, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orderById = await _sales.GetOrderById(OrderNumber, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = orderById;
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
        /// gets Order record by Customer Id
        /// </summary>
        [Route("api/GetOrderByCustomerId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderByCustomerId(string CustomerId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orderByCustomerId = await _sales.GetOrderByCustomerId(CustomerId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = orderByCustomerId;
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
        /// creates a new Order record 
        /// </summary>
        [Route("api/AddOrders/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddOrders([FromBody] Order salesPolicy, string token, string reference)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (salesPolicy.CustomerId == null || salesPolicy.CustomerId == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Customer ID";
                    //return Ok(statusMessage);
                }
                else if (salesPolicy.OrderDate == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Order Date";
                    //return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _sales.GetAccess(token);
                   
                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {

                            //statusMessage = _sales.AddOrder(salesPolicy, token);


                            if (reference == null || reference == "")
                            {
                                var cusInfo = await _sales.GetCustomerById(salesPolicy.CustomerId, tokenObj);
                                var acc = (double)(cusInfo.customerFinancials.AvailibleCredit);

                                //re-compute order total here
                                var amountToPay = await _sales.CalcTotalWithAvailableCredit(salesPolicy, tokenObj);
                                if (amountToPay > 0)
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Insufficient balance";
                                }
                                else
                                {
                                    var AvailCred = await _sales.AddOrder(salesPolicy, tokenObj);
                                    statusMessage = AvailCred;
                                }
                            }
                            else
                            {
                                var paymentInfo = await PaymentVerification(reference, tokenObj);

                                if (paymentInfo != null)
                                {
                                    if (paymentInfo.status)
                                    {
                                        if (paymentInfo.data.Status == "success")
                                        {
                                            Receipt receipt = new Receipt();



                                            double Amount = paymentInfo.data.Amount * 0.01;
                                            var fee = paymentInfo.data.Fees;

                                            var sFee = Convert.ToString(paymentInfo.data.Fees);

                                            var dFee = Convert.ToDouble(sFee);

                                            var dAmount = (Amount - (dFee * 0.01));

                                           // var dAmount = (Amount - fees);


                                            receipt.Amount = (decimal)dAmount;
                                            receipt.ReceiptTypeId = "Master";
                                            receipt.ReceiptClassId = "Customer";
                                            receipt.UnAppliedAmount = (decimal)dAmount;
                                            receipt.CreditAmount = (decimal)dAmount;
                                            receipt.CustomerId = salesPolicy.CustomerId;
                                            

                                            ReceiptsDetail receiptsDetail = new ReceiptsDetail();

                                            receiptsDetail.WriteOffAmount = (decimal)dAmount;
                                            receiptsDetail.CustomerId =salesPolicy.CustomerId;
                                            receiptsDetail.ReceiptDetailId = 0;

                                            List<ReceiptsDetail> rd = new List<ReceiptsDetail>();
                                            receipt.receiptsDetails = rd;
                                           
                                            receipt.receiptsDetails.Add(receiptsDetail);
                                            //receiptsDetail;     

                                            //create receipt
                                            statusMessage = await _sales.AddReceiptPost(receipt, tokenObj);

                                            salesPolicy.PaymentMethodId = paymentInfo.data.Channel;
                                            var result = await _sales.AddOrder(salesPolicy, tokenObj);
                                            statusMessage = result;
                                            
                                        }
                                        else
                                        {
                                            statusMessage.Status = "Failed";
                                            statusMessage.Message = paymentInfo.data.Status;
                                        }

                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = paymentInfo.message;
                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Unable to Resolve Payment Issues, Try Again";
                                }
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";

                    }
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;

            }

            return Ok(statusMessage);
        }

        /// <summary>
        ///  Verifies Order Payment 
        /// </summary>
        /// <param name="reference"></param>      
        /// <returns></returns>
        private async Task<PayStackVerify> PaymentVerification(string reference, ApiToken token)
        {
            PayStackVerify payStackVerify = new PayStackVerify();
            //ApiToken token = new ApiToken();

            //string bearerToken = "sk_test_78ba02aeb10444af8d6927c951f5eb9c774c1bf7";
            var tok = await _sales.GetPaymentToken(token);
            string bearerToken = tok.ApiKey;

            try
            {
                var url = "https://api.paystack.co/transaction/verify/" + reference + "";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Accept = "application/json";
                httpRequest.Headers["Authorization"] = "Bearer " + bearerToken;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    //payStackVerify = JsonConvert.DeserializeObject<PayStackVerify>(result);
                    payStackVerify = PayStackVerify.FromJson(result);
                }
            }
            catch (Exception ex)
            {
                payStackVerify.status = false;
                payStackVerify.message = "Try Again." + ex.Message;
            }
            return payStackVerify;
        }

        /// <summary>
        /// gets Order record by Order Id
        /// </summary>
        [Route("api/GetReceiptByCustomerId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetReceiptByCusId(string CustomerId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var customerById = await _sales.GetReceiptByCusId(CustomerId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = customerById;
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
        ///updates Order Reference records
        /// <param name="token"></param>
        /// <param name="receipt"></param>
        ///// <returns></returns>
        [Route("api/AddReceipt/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddReceiptPost([FromBody] Receipt receipt, string Amount, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _sales.AddReceiptPost(receipt, tokenObj);

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
        ///updates Order Reference records
        /// <param name="token"></param>
        /// <param name="orderLog"></param>
        /// <returns></returns>
        [Route("api/AddOrderReferenceLog/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddOrderReference([FromBody] OrderReferenceLog orderLog, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _sales.AddOrderReference(orderLog, tokenObj);

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
        /// Initializes Payment
        /// </summary>
        [Route("api/PaymentInitialization/{token}")]
        [HttpPost]
        public async Task<IActionResult> PaymentInitialization([FromBody] Order order, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                   //bool isEmailValid = Regex.IsMatch(email,pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$", options: RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                   // if (!isEmailValid)
                   // {
                   //     statusMessage.Status = "Failed";
                   //     statusMessage.Message = "Invalid Email Format";

                   //     return BadRequest(statusMessage);
                   // }
                    
                    if (tokenObj.TotalDays >= 0)
                    {
                        var result = await _sales.PaymentInitialization( order,tokenObj);

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
        /// gets RMA record by customer Id
        /// </summary>
        [Route("api/GetCustomerById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerById(string id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var cusById = await _sales.GetCustomerById(id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = cusById;
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
        /// gets RMA record by customer Id
        /// </summary>
        [Route("api/GetRmaByCustomerEmail/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerByEmail(string Email, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var cusById = await _sales.GetCustomerByEmail(Email, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = cusById;
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
        /// gets a list of RMA records
        /// </summary>
        [Route("api/GetRMA/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRMA([FromQuery]PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var rma = await _sales.GetRMA( Param,  tokenObj);

                        statusMessage.Metadata = rma.PaginationMetadata;
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = rma.RmaList;
                        
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
        /// gets RMA record by customer Id
        /// </summary>
        [Route("api/GetRmaByCustomerId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRmaByCustomerId(string CustomerId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var rmaById = await _sales.GetRmaByCustomerId(CustomerId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = rmaById;
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
        /// gets RMA record by Invoice Id
        /// </summary>
        [Route("api/GetRmaByInvoiceId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetRmaByInvoiceId(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var rmaById = await _sales.GetRmaByInvoiceId(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = rmaById;
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
        /// creates a new RMA record 
        /// </summary>
        [Route("api/AddRMA/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddRMA([FromBody] RMA returnMA, string token)

        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {   if (returnMA.OrderNumber == null || returnMA.OrderNumber == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Order Number";
                    return Ok(statusMessage);
                }
                else if (returnMA.CustomerId == null || returnMA.CustomerId == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Customer ID";
                    return Ok(statusMessage);
                }
                else if (returnMA.InvoiceDate == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Invoice Date";
                    return Ok(statusMessage);
                }
                else if (returnMA.RmaDetail.Count <= 0)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Return Attached Items To RMA";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _sales.GetAccess(token);

                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            var result = await _sales.AddRMA(returnMA, tokenObj);
                            //statusMessage.Status = "Success";
                            //statusMessage.Message = "Success";
                            //statusMessage.data = result;
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
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets a list of quotes records
        /// </summary>
        [Route("api/GetQuotes/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetQuotes([FromQuery]PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var quotes = await _sales.GetQuotes(Param, tokenObj);

                        statusMessage.Metadata = quotes.PaginationMetadata;
                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = quotes.QuoteList;
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
        /// gets quotes record by customer Id
        /// </summary>
        [Route("api/GetQuotesByCustomerId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetQuotesByCutomerId(string CustomerId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var orderById = await _sales.GetQuotesByCustomerId(CustomerId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = orderById;
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
        /// gets Order record by Order Id
        /// </summary>
        [Route("api/GetQuotesById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetQuotesById(string OrderNumber, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var quotesById = await _sales.GetQuotesById(OrderNumber, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = quotesById;
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
        /// creates and updates a new Quotes records
        /// </summary>
        [Route("api/AddQuotes/{token}")]
        [HttpPost]
        public async Task<IActionResult> AddQuotes([FromBody] Order salesPolicy, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var quotes = await _sales.AddQuotes(salesPolicy, tokenObj);
                        statusMessage = quotes;
                        return Ok(quotes);
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Token";
                }
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";
            }
            return Ok(statusMessage);
        }

        /// <summary>
        /// gets inventory item by name
        /// </summary>
        [Route("api/GetInventoryItemByName/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryItemByName(string ItemName, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var itemByName = await _sales.GetInventoryItemByName(ItemName, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = itemByName;
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
        /// gets inventory item by id
        /// </summary>
        [Route("api/GetInventoryItemById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryItemById(string ItemID, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var itemById = await _sales.GetInventoryItemById(ItemID, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = itemById;
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
        /// gets Warehouse By Id
        /// </summary>
        [Route("api/GetWarehouseById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseById(string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var ById = await _sales.GetWarehouseById(Id, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = Id;
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
        /// gets Warehouse by bin Id
        /// </summary>
        [Route("api/GetWarehouseByBinId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetWarehouseByBinId(string BinId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var ByBinId = await _sales.GetWarehouseByBinId(BinId, tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = ByBinId;
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
        /// gets Inventory by customer Id
        /// </summary>
        [Route("api/GetSalesItemsByCustomerId/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetSalesItemsByCustomerId(string CustomerId, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var tokenObj = await _sales.GetAccess(token);
                if (CustomerId == null || CustomerId == "")
                {

                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Customer Id";
                    return Ok(statusMessage);
                }
                else
                {
                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            var ByCusId = await _sales.GetItemsByCustomerId(CustomerId, tokenObj);

                            statusMessage.Status = "Success";
                            statusMessage.Message = "Success";
                            statusMessage.data = ByCusId;
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
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// gets payment token
        /// </summary>
        [Route("api/GetPaymentToken/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentToken( string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var payToken = await _sales.GetPaymentToken(tokenObj);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = payToken;

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
        /// gets order summary
        /// </summary>
        [Route("api/ComputeOrderSummary/{token}")]
        [HttpPost]
        public async Task<IActionResult>SummaryOfOrder([FromBody] Order order, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var tokenObj = await _sales.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                        var ordSummary = await _sales.SummaryOfOrder(order, tokenObj) ;

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                        statusMessage.data = ordSummary;

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
    }
}



