using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface ISales : IAccount
    {
        Task<Paging> GetAllOrders(PaginationParams Param, ApiToken tokenObj);
        Task<IEnumerable<OrderRetrieveDto>> GetOrderById(string Id, ApiToken tokenObj);
        Task<IEnumerable<OrderRetrieveDto>> GetOrderByCustomerId(string customerId, ApiToken tokenObj);
        Task<StatusMessage> AddOrder(Order salesPolicy, ApiToken tokenObj);
        Task<StatusMessage> AddReceiptPost(Receipt receipt, ApiToken tokenObj);
        Task<StatusMessage> OrderBooking(string Id, ApiToken tokenObj);
        Task<StatusMessage> ReceiptPosting(string Id, ApiToken tokenObj);
        Task<Paging> GetRMA( PaginationParams Param, ApiToken tokenObj);
        Task<IEnumerable<RMA>> GetRmaByCustomerId(string customerId, ApiToken tokenObj);
        Task<StatusMessage> AddRMA(RMA returnMA, ApiToken tokenObj);
        Task<StatusMessage> RMAPost(string Id, ApiToken tokenObj);
        Task<Paging> GetQuotes(PaginationParams Param, ApiToken tokenObj);
        Task<IEnumerable<OrderRetrieveDto>> GetQuotesById(string orderNumber, ApiToken tokenObj);
        Task<StatusMessage> AddQuotes(Order salesPolicy, ApiToken tokenObj);
        Task<IEnumerable<OrderRetrieveDto>> GetQuotesByCustomerId(string customerId, ApiToken tokenObj);
        Task<Data.Models.InventoryItems> GetInventoryItemById(string ItemId, ApiToken token);
        Task<OrderReferenceLog> AddOrderReference (OrderReferenceLog orderLog, ApiToken tokenObj);
        Task<StatusMessage> PaymentInitialization(Order order, ApiToken tokenObj);
        Task<IEnumerable<Data.Models.InventoryItems>> GetInventoryItemByName(string itemName, ApiToken tokenObj);
        Task<CustomerInform> GetCustomerById(string id, ApiToken tokenObj);
        Task<CustomerInform> GetCustomerByEmail(string Email, ApiToken tokenObj);
        Task<IEnumerable<RMA>> GetRmaByInvoiceId(string id, ApiToken tokenObj);
        Task<Warehouses> GetWarehouseById(string id, ApiToken tokenObj);
        Task<WarehouseBins> GetWarehouseByBinId(string binId, ApiToken tokenObj);
        Task<IEnumerable<Data.Models.InventoryItems>> GetItemsByCustomerId(string CustomerId, ApiToken tokenObj);
        Task<CompanyKeys> GetPaymentToken(ApiToken tokenObj);
        Task<double> CalcTotalWithAvailableCredit(Order order, ApiToken token);
        Task<OrderSummary> SummaryOfOrder(Order order, ApiToken token);
        Task<IEnumerable<Receipt>> GetReceiptByCusId(string customerId, ApiToken tokenObj);

        //Task<PayStackVerify> PaymentVerification(string reference);

        // Task CalcSubTot(Order salesPolicy, ApiToken tokenObj);


        //Task<IEnumerable<Order>> GetQuoteById(string id, ApiToken tokenObj);
    }
}
