using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Helper;
using PowerAPI.Service.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;

namespace PowerAPI.Service.Clients
{
    public class Customer : ICustomer
    {
        EnterpriseContext _DBContext;

        public Customer(EnterpriseContext DBContext)
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
            catch (Exception ex)
            {
                throw;
            }

            return apiToken;
        }

        public async Task<bool> CustomerLogin(string Username, string Password, ApiToken token)
        {
            bool status = false;

            try
            {
                string pwd = EnterpriseExtras.doConvertPwd(Password);

                var employee = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == Username &&
                                                         x.CustomerPassword == pwd).FirstOrDefaultAsync();
                if (employee != null)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        public async Task<StatusMessage> ChangePwd(PasswordModel changePassword, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            string newPwd = "";

            try
            {
                //check old password
                string oldPwd = EnterpriseExtras.doConvertPwd(changePassword.OldPassword);

                var customer = await _DBContext.CustomerInformation.Where(x =>
                                                         x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerId == changePassword.Username &&
                                                         x.CustomerPassword == oldPwd).FirstOrDefaultAsync();
                if (customer != null)
                {
                    //encrypt new password
                    newPwd = EnterpriseExtras.doConvertPwd(changePassword.NewPassword);

                    customer.CustomerPassword = newPwd;
                    customer.CustomerPasswordOld = oldPwd;
                    customer.CustomerPasswordDate = DateTime.Now;

                    _DBContext.Entry(customer).State = EntityState.Modified;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Incorrect Username and/or Password";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }
        
        public async Task<CustomerInform> GetCustomerById(string Id, ApiToken token)
        {
            CustomerInform customerInfo = new CustomerInform();

            try
            {
                var customers = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.CustomerId == Id &&
                                                            x.CustomerTypeId != "User").FirstOrDefaultAsync();
                customerInfo = await cusInfo(customers, token);

            }
            catch (Exception)
            {
                throw;
            }
            return customerInfo;
        }

        
        public async Task<CustomerInform> GetCustomerByEmail(string Email, ApiToken token)
        {
            CustomerInform customersInfo = new CustomerInform();
            try
            {
                var customerByEmail = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerEmail == Email).FirstOrDefaultAsync();
                customersInfo = await cusInfo(customerByEmail, token);
            }
            catch (Exception)
            {
                throw;
            }
            return customersInfo;
        }

        public async Task<CustomerInform> CustomerLoginEmail(string Username, string Password, ApiToken token)
        {
            //bool status = false;
            //StatusMessage statusMessage = new StatusMessage();
            CustomerInform customersInfo = new CustomerInform();
            try
            {
                string pwd = EnterpriseExtras.doConvertPwd(Password);

                var customerUsername = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.CustomerPassword == pwd &&
                                                         (x.CustomerEmail == Username ||
                                                         x.CustomerPhone == Username  ||
                                                         x.CustomerId == Username)).FirstOrDefaultAsync();
                //if (employee != null)
                //{
                //    status = true;
                //}

                if (customerUsername != null) {
                    customersInfo = await cusInfo(customerUsername, token);
                }
                else
                {
                    customersInfo = null;
                    //throw new InvalidOperationException("Failed, Incorrect Username and/or Password.");
                }
                
            }
            catch (Exception ex)
            {
                throw;//status = false;
            }
            return customersInfo;

            //return employee;
        }

        public async Task<Paging> GetCustomer(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.CustomerInform> customers = new List<Data.ViewModels.CustomerInform>();
            try
            {
                var totalCount = await _DBContext.CustomerInformation
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.CustomerInformation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.CustomerId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                customers = await customer(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    CustomerList = customers
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Paging> GetSalesRepresentatives(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.SalesRepresentatives> salesRepresentatives = new List<Data.ViewModels.SalesRepresentatives>();
            try
            {
                var totalCount = await _DBContext.SalesRepresentatives
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.SalesRepresentatives.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.SalesRepId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                salesRepresentatives = await salesRepresentative(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    SalesRepresentativesList = salesRepresentatives
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }






        #region View Model Data

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

        private async Task<List<Data.ViewModels.CustomerInform>> customer(List<Data.Models.CustomerInformation> obj, ApiToken token)
        {
            List<Data.ViewModels.CustomerInform> customers = new List<Data.ViewModels.CustomerInform>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.CustomerInformation customerInformation in obj)
                    {
                        Data.ViewModels.CustomerInform customerObj = new Data.ViewModels.CustomerInform();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        customerObj.CompanyId = customerInformation.CompanyId;
                        customerObj.DivisionId = customerInformation.DivisionId;
                        customerObj.DepartmentId = customerInformation.DepartmentId;
                        customerObj.CustomerId = customerInformation.CustomerId;
                        customerObj.AccountStatus = customerInformation.AccountStatus;
                        customerObj.CustomerName = customerInformation.CustomerName;
                        customerObj.CustomerAddress1 = customerInformation.CustomerAddress1;
                        customerObj.CustomerAddress2 = customerInformation.CustomerAddress2;
                        customerObj.CustomerAddress3 = customerInformation.CustomerAddress3;
                        customerObj.CustomerCity = customerInformation.CustomerCity;
                        customerObj.CustomerState = customerInformation.CustomerState;
                        customerObj.CustomerZip = customerInformation.CustomerZip;
                        customerObj.CustomerCountry = customerInformation.CustomerCountry;
                        customerObj.CustomerPhone = customerInformation.CustomerPhone;
                        customerObj.CustomerFax = customerInformation.CustomerFax;
                        customerObj.CustomerEmail = customerInformation.CustomerEmail;
                        customerObj.CustomerWebPage = customerInformation.CustomerWebPage;
                        customerObj.CustomerLogin = customerInformation.CustomerLogin;
                        customerObj.CustomerPasswordDate = customerInformation.CustomerPasswordDate;
                        customerObj.CustomerPasswordExpires = customerInformation.CustomerPasswordExpires;
                        customerObj.CustomerPasswordExpiresDate = customerInformation.CustomerPasswordExpiresDate;
                        customerObj.CustomerFirstName = customerInformation.CustomerFirstName;
                        customerObj.CustomerSalutation = customerInformation.CustomerSalutation;
                        customerObj.Attention = customerInformation.Attention;
                        customerObj.CustomerTypeId = customerInformation.CustomerTypeId;
                        customerObj.TaxIdno = customerInformation.TaxIdno;
                        customerObj.VattaxIdnumber = customerInformation.VattaxIdnumber;
                        customerObj.VatTaxOtherNumber = customerInformation.VatTaxOtherNumber;
                        customerObj.GlsalesAccount = customerInformation.GlsalesAccount;
                        customerObj.TermsId = customerInformation.TermsId;
                        customerObj.TermsStart = customerInformation.TermsStart;
                        customerObj.EmployeeId = customerInformation.EmployeeId;
                        customerObj.TaxGroupId = customerInformation.TaxGroupId;
                        customerObj.PriceMatrix = customerInformation.PriceMatrix;
                        customerObj.PriceMatrixCurrent = customerInformation.PriceMatrixCurrent;
                        customerObj.CreditRating = customerInformation.CreditRating;
                        customerObj.CreditLimit = customerInformation.CreditLimit;
                        customerObj.CreditComments = customerInformation.CreditComments;
                        customerObj.PaymentDay = customerInformation.PaymentDay;
                        customerObj.ApprovalDate = customerInformation.ApprovalDate;
                        customerObj.CustomerSince = customerInformation.CustomerSince;
                        customerObj.SendCreditMemos = customerInformation.SendCreditMemos;
                        customerObj.SendDebitMemos = customerInformation.SendDebitMemos;
                        customerObj.Statements = customerInformation.Statements;
                        customerObj.StatementCycleCode = customerInformation.StatementCycleCode;
                        customerObj.CustomerSpecialInstructions = customerInformation.CustomerSpecialInstructions;
                        customerObj.CustomerShipToId = customerInformation.CustomerShipToId;
                        customerObj.ShipMethodId = customerInformation.ShipMethodId;
                        customerObj.WarehouseId = customerInformation.WarehouseId;
                        customerObj.RoutingInfo1 = customerInformation.RoutingInfo1;
                        customerObj.RoutingInfo2 = customerInformation.RoutingInfo2;
                        customerObj.RoutingInfo3 = customerInformation.RoutingInfo3;
                        customerObj.RoutingInfoCurrent = customerInformation.RoutingInfoCurrent;
                        customerObj.FreightPayment = customerInformation.FreightPayment;
                        customerObj.PickTicketsNeeded = customerInformation.PickTicketsNeeded;
                        customerObj.PackingListNeeded = customerInformation.PackingListNeeded;
                        customerObj.SpecialLabelsNeeded = customerInformation.SpecialLabelsNeeded;
                        customerObj.CustomerItemCodes = customerInformation.CustomerItemCodes;
                        customerObj.ConfirmBeforeShipping = customerInformation.ConfirmBeforeShipping;
                        customerObj.Backorders = customerInformation.Backorders;
                        customerObj.UseStoreNumbers = customerInformation.UseStoreNumbers;
                        customerObj.UseDepartmentNumbers = customerInformation.UseDepartmentNumbers;
                        customerObj.SpecialShippingInstructions = customerInformation.SpecialShippingInstructions;
                        customerObj.RoutingNotes = customerInformation.RoutingNotes;
                        customerObj.ApplyRebate = customerInformation.ApplyRebate;
                        customerObj.RebateAmount = customerInformation.RebateAmount;
                        customerObj.RebateGlaccount = customerInformation.RebateGlaccount;
                        customerObj.RebateGlaccount = customerInformation.RebateGlaccount;
                        customerObj.ApplyNewStore = customerInformation.ApplyNewStore;
                        customerObj.NewStoreGlaccount = customerInformation.NewStoreGlaccount;
                        customerObj.NewStoreDiscount = customerInformation.NewStoreDiscount;
                        customerObj.NewStoreDiscountNotes = customerInformation.NewStoreDiscountNotes;
                        customerObj.ApplyWarehouse = customerInformation.ApplyWarehouse;
                        customerObj.WarehouseAllowance = customerInformation.WarehouseAllowance;
                        customerObj.WarehouseGlaccount = customerInformation.WarehouseGlaccount;
                        customerObj.WarehouseAllowanceNotes = customerInformation.WarehouseAllowanceNotes;
                        customerObj.ApplyAdvertising = customerInformation.ApplyAdvertising;
                        customerObj.AdvertisingDiscount = customerInformation.AdvertisingDiscount;
                        customerObj.AdvertisingGlaccount = customerInformation.AdvertisingGlaccount;
                        customerObj.ApplyManualAdvert = customerInformation.ApplyManualAdvert;
                        customerObj.ManualAdvertising = customerInformation.ManualAdvertising;
                        customerObj.RebateGlaccount = customerInformation.RebateGlaccount;
                        customerObj.ManualAdvertisingGlaccount = customerInformation.ManualAdvertisingGlaccount;
                        customerObj.ManualAdvertisingNotes = customerInformation.ManualAdvertisingNotes;
                        customerObj.ApplyTrade = customerInformation.ApplyTrade;
                        customerObj.TradeDiscount = customerInformation.TradeDiscount;
                        customerObj.TradeDiscountGlaccount = customerInformation.TradeDiscountGlaccount;
                        customerObj.TradeDiscountNotes = customerInformation.TradeDiscountNotes;
                        customerObj.SpecialTerms = customerInformation.SpecialTerms;
                        customerObj.Ediqualifier = customerInformation.Ediqualifier;
                        customerObj.Ediid = customerInformation.Ediid;
                        customerObj.EditestQualifier = customerInformation.EditestQualifier;
                        customerObj.EditestId = customerInformation.EditestId;
                        customerObj.EdicontactName = customerInformation.EdicontactName;
                        customerObj.EdicontactAgentFax = customerInformation.EdicontactAgentFax;
                        customerObj.EdicontactAgentPhone = customerInformation.EdicontactAgentPhone;
                        customerObj.EdicontactAddressLine = customerInformation.EdicontactAddressLine;
                        customerObj.EdipurchaseOrders = customerInformation.EdipurchaseOrders;
                        customerObj.Ediinvoices = customerInformation.Ediinvoices;
                        customerObj.Edipayments = customerInformation.Edipayments;
                        customerObj.EdiorderStatus = customerInformation.EdiorderStatus;
                        customerObj.EdishippingNotices = customerInformation.EdishippingNotices;
                        customerObj.Approved = customerInformation.Approved;
                        customerObj.ApprovedBy = customerInformation.ApprovedBy;
                        customerObj.ApprovedDate = customerInformation.ApprovedDate;
                        customerObj.EnteredBy = customerInformation.EnteredBy;
                        customerObj.ConvertedFromVendor = customerInformation.ConvertedFromVendor;
                        customerObj.ConvertedFromLead = customerInformation.ConvertedFromLead;
                        customerObj.CustomerRegionId = customerInformation.CustomerRegionId;
                        customerObj.CustomerSourceId = customerInformation.CustomerSourceId;
                        customerObj.CustomerIndustryId = customerInformation.CustomerIndustryId;
                        customerObj.Confirmed = customerInformation.Confirmed;
                        customerObj.FirstContacted = customerInformation.FirstContacted;
                        customerObj.LastFollowUp = customerInformation.LastFollowUp;
                        customerObj.NextFollowUp = customerInformation.NextFollowUp;
                        customerObj.ReferedByExistingCustomer = customerInformation.ReferedByExistingCustomer;
                        customerObj.ReferedBy = customerInformation.ReferedBy;
                        customerObj.ReferalUrl = customerInformation.ReferalUrl;
                        customerObj.Hot = customerInformation.Hot;
                        customerObj.PrimaryInterest = customerInformation.PrimaryInterest;
                        customerObj.LockedBy = customerInformation.LockedBy;
                        customerObj.LockTs = customerInformation.LockTs;
                        customerObj.AccountBalance = customerInformation.AccountBalance;
                        customerObj.BranchCode = customerInformation.BranchCode;
                        customerObj.KnowYourCustomer = customerInformation.KnowYourCustomer;
                        customerObj.Smsalert = customerInformation.Smsalert;
                        customerObj.EmailAlert = customerInformation.EmailAlert;

                        customerObj.customerFinancials = await _DBContext.CustomerFinancials.Where(x => x.CompanyId == token.CompanyId &&
                                   x.DivisionId == token.DivisionId &&
                                   x.DepartmentId == token.DepartmentId &&
                                   x.CustomerId == customerInformation.CustomerId).FirstOrDefaultAsync();



                        customerObj.WorkFlowTrail = workflows;
                        customers.Add(customerObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return customers;

        }
       
        private async Task<List<Data.ViewModels.SalesRepresentatives>> salesRepresentative(List<Data.Models.SalesRepresentatives> obj, ApiToken token)
        {
            List<Data.ViewModels.SalesRepresentatives> salesRepresentatives = new List<Data.ViewModels.SalesRepresentatives>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.SalesRepresentatives salesRepresentative in obj)
                    {
                        Data.ViewModels.SalesRepresentatives salesRepresentativeObj = new Data.ViewModels.SalesRepresentatives();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        salesRepresentativeObj.CompanyId = salesRepresentative.CompanyId;
                        salesRepresentativeObj.DivisionId = salesRepresentative.DivisionId;
                        salesRepresentativeObj.DepartmentId = salesRepresentative.DepartmentId;
                        salesRepresentativeObj.SalesRepId = salesRepresentative.SalesRepId;
                        salesRepresentativeObj.Name = salesRepresentative.Name;
                        salesRepresentativeObj.Method = salesRepresentative.Method;
                        salesRepresentativeObj.Target1 = salesRepresentative.Target1;
                        salesRepresentativeObj.Commission1 = salesRepresentative.Commission1;
                        salesRepresentativeObj.Target2 = salesRepresentative.Target2;
                        salesRepresentativeObj.Commission2 = salesRepresentative.Commission2;
                        salesRepresentativeObj.Target3 = salesRepresentative.Target3;
                        salesRepresentativeObj.Commission3 = salesRepresentative.Commission3;
                        salesRepresentativeObj.Target4 = salesRepresentative.Target4;
                        salesRepresentativeObj.Commission4 = salesRepresentative.Commission4;
                        salesRepresentativeObj.Commission5 = salesRepresentative.Commission5;
                        salesRepresentativeObj.Address1 = salesRepresentative.Address1;
                        salesRepresentativeObj.Address2 = salesRepresentative.Address2;
                        salesRepresentativeObj.Address3 = salesRepresentative.Address3;
                        salesRepresentativeObj.Active = salesRepresentative.Active;
                        salesRepresentativeObj.BankAccount = salesRepresentative.BankAccount;
                        salesRepresentativeObj.Comment1 = salesRepresentative.Comment1;
                        salesRepresentativeObj.Comment2 = salesRepresentative.Comment2;
                        salesRepresentativeObj.Comment3 = salesRepresentative.Comment3;
                        salesRepresentativeObj.EstimatedValue = salesRepresentative.EstimatedValue;
                        salesRepresentativeObj.ActualValue = salesRepresentative.ActualValue;
                        salesRepresentativeObj.LockedBy = salesRepresentative.LockedBy;
                        salesRepresentativeObj.LockTs = salesRepresentative.LockTs;
                        salesRepresentativeObj.SalesGroupId = salesRepresentative.SalesGroupId;
                        salesRepresentativeObj.BranchCode = salesRepresentative.BranchCode;
                        salesRepresentativeObj.CommissionPercent = salesRepresentative.CommissionPercent;
                        salesRepresentativeObj.LastModifiedBy = salesRepresentative.LastModifiedBy;
                        salesRepresentativeObj.LastModifiedDate = salesRepresentative.LastModifiedDate;
                    
                        salesRepresentativeObj.WorkFlowTrail = workflows;
                        salesRepresentatives.Add(salesRepresentativeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return salesRepresentatives;

        }

        #endregion View Model Data












        #region Customer Profile Utility
        private string generateOTP(string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            OTPGeneratorCode oTPGenerator = new OTPGeneratorCode(); 
            var randomCode = "";
            try
            {
                Random random = new Random();
                randomCode = (random.Next(999999).ToString());
                return randomCode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<StatusMessage> ResetPasswordOTP(string Username, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            OTPGeneratorCode otpGenerator = new OTPGeneratorCode(); 
            CustomerInform customersInfo = new CustomerInform();
            MessageResponse messageRes = new MessageResponse();
            MessageRequest messageReq = new MessageRequest();
            MailSend _mailSend = new MailSend();


            var randomCode = "";

            //DateTime Date = new DateTime(2023, 6, 14, 4, 43, 60);

            try
            {
                var keys = await _DBContext.CompanyKeys.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         x.ProfileType == "SMS" &&
                                                         x.Active == true).FirstOrDefaultAsync();

                var customerUsername = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                         x.DivisionId == token.DivisionId &&
                                                         x.DepartmentId == token.DepartmentId &&
                                                         //x.CustomerPassword == pwd &&
                                                         (x.CustomerEmail == Username ||
                                                         x.CustomerPhone == Username ||
                                                         x.CustomerId == Username)).FirstOrDefaultAsync();
             

                if (customerUsername != null)
                {
                    Random random = new Random();
                    randomCode = (random.Next(999999).ToString());

                    otpGenerator.CompanyId = customerUsername.CompanyId;
                    otpGenerator.DivisionId = customerUsername.DivisionId;
                    otpGenerator.DepartmentId = customerUsername.DepartmentId;
                    otpGenerator.CustomerId = customerUsername.CustomerId;
                    otpGenerator.OTPCode = randomCode;
                    otpGenerator.OTPUsed = false;
                    otpGenerator.TimeSent = DateTime.Now;
                    otpGenerator.TimeExpired = DateTime.Now.AddSeconds(60);

                    _DBContext.Entry(otpGenerator).State = EntityState.Added;
                    var rowCount = _DBContext.SaveChanges();

                    var bearerToken = keys.ApiKey;
                    //var bearerToken = "MA-f2ca0e17-a994-4382-93fd-645a5962b04e";

                    if (rowCount > 0)
                    {
                        //messageReq.senderID = "DANTATA_PL";
                        messageReq.senderID = keys.SenderId;
                        messageReq.mobileNumber = customerUsername.CustomerPhone;
                        messageReq.messageText = "Please use the OTP code: " + randomCode + " to complete your password reset";
                        messageReq.route = "eeee";

                        var url = keys.BaseUrl;
                        //var url = "https://api.smslive247.com/api/v4/sms";

                        var uri = new Uri(url);
                        //string initializeEndPoint = "initialize";

                        var integrationHelper = new IntegrationHelper(uri);
                        var requestUrl = integrationHelper.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, url));


                        messageRes = await integrationHelper.PostAsync<MessageResponse, MessageRequest>(requestUrl, messageReq, bearerToken);

                        statusMessage.Status = "Success";
                        statusMessage.Message = randomCode;
                        statusMessage.data = messageRes;

                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Unsuccessful";
                    }
                   
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Incorrect Username";

                }

            }
            catch(Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }

        public async Task<StatusMessage> ValidateOTP(string username, string otp, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var customerUsername = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                                           x.DivisionId == token.DivisionId &&
                                                                           x.DepartmentId == token.DepartmentId &&
                                                                           //x.CustomerPassword == pwd &&
                                                                           (x.CustomerEmail == username ||
                                                                           x.CustomerPhone == username ||
                                                                           x.CustomerId == username)).FirstOrDefaultAsync();

                var otpStat = await _DBContext.OTPGeneratorCode.Where(x => x.CompanyId == token.CompanyId &&
                                                                           x.DivisionId == token.DivisionId &&
                                                                           x.DepartmentId == token.DepartmentId &&
                                                                           x.CustomerId == customerUsername.CustomerId &&
                                                                           x.OTPCode == otp).FirstOrDefaultAsync(); 
                if (otpStat != null)
                {
                    if(otpStat.OTPUsed == false && otpStat.TimeExpired > DateTime.Now)
                    {
                        statusMessage.Status = "Success";
                        statusMessage.Message = "OTP Validated";
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "OTP not Valid or expired";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Username or OTP";
                }

            }
            catch(Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }

        public async Task<StatusMessage> ResetPassword (string username, string otp, string password, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            string newPwd = "";
            try
            {
                var customerUsername = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
                                                                           x.DivisionId == token.DivisionId &&
                                                                           x.DepartmentId == token.DepartmentId &&
                                                                           //x.CustomerPassword == pwd &&
                                                                           (x.CustomerEmail == username ||
                                                                           x.CustomerPhone == username ||
                                                                           x.CustomerId == username)).FirstOrDefaultAsync();

                var otpStat = await _DBContext.OTPGeneratorCode.Where(x => x.CompanyId == token.CompanyId &&
                                                                           x.DivisionId == token.DivisionId &&
                                                                           x.DepartmentId == token.DepartmentId &&
                                                                           x.CustomerId == customerUsername.CustomerId &&
                                                                           x.OTPCode == otp).FirstOrDefaultAsync();
                if (otpStat != null)
                {
                    if(otpStat.OTPUsed == false)
                    {
                        newPwd = EnterpriseExtras.doConvertPwd(password);

                        customerUsername.CustomerPassword = newPwd;
                        customerUsername.CustomerPasswordDate = DateTime.Now;

                        _DBContext.Entry(customerUsername).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        otpStat.OTPUsed = true;
                        otpStat.TransactionCompleted = true;
                        _DBContext.Entry(otpStat).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Password Reset Successful";

                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "OTP has already been used";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Username or OTP";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }

        
        #endregion Customer Profile Utility


        //private async Task<StatusMessage> SendMail(string Username, ApiToken token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();
        //    MailSend _mailSend = new MailSend();

        //    try
        //    {
        //        var customerUsername = await _DBContext.CustomerInformation.Where(x => x.CompanyId == token.CompanyId &&
        //                                                 x.DivisionId == token.DivisionId &&
        //                                                 x.DepartmentId == token.DepartmentId &&
        //                                                 //x.CustomerPassword == pwd &&
        //                                                 (x.CustomerEmail == Username ||
        //                                                 x.CustomerPhone == Username ||
        //                                                 x.CustomerId == Username)).FirstOrDefaultAsync();
        //        if (customerUsername == null)
        //        {
        //            statusMessage.Status = "Failed";
        //            statusMessage.Message = "Incorrect Username and/or Password";
        //        }
        //        else if (customerUsername.CustomerEmail == null || customerUsername.CustomerEmail == "")
        //        {
        //            statusMessage.Status = "Failed";
        //            statusMessage.Message = "No Email Address. Contact System Administrator";
        //        }
        //        else
        //        {

        //            //if (generatedPwd != null && generatedPwd != "")
        //            //{
        //            //    //encrypt new password
        //            //    newPwd = EnterpriseExtras.doConvertPwd(generatedPwd);

        //            //    employee.EmployeePasswordOld = employee.EmployeePassword;
        //            //    employee.EmployeePassword = newPwd;
        //            //    employee.EmployeePasswordDate = DateTime.Now;

        //            //    _DBContext.Entry(employee).State = EntityState.Modified;
        //            //    var rowCount = _DBContext.SaveChanges();

        //            //if (rowCount > 0)
        //            //{
        //            var Body = "<html><body style='font-family:Calibri;'><p>" +
        //                        "Dear " + customerUsername.CustomerName +
        //                        "Dear " + customerUsername.CustomerName +
        //                    ",<br></p><p>Your one-time password has been sent successfully</p><br>" +
        //                    "<p><strong> New Password: </strong>" + randomCode +
        //                    "</p>" +
        //                    "<br><br><p> Sent from: <strong>Power Employee Manager</strong></p><p>Date: <strong> " + DateTime.Now.Day.ToString() + '/' + DateTime.Now.Month.ToString() + '/' + DateTime.Now.Year.ToString() +
        //                    "</strong></p></body></html>" +
        //                    "";

        //            var counter = await _DBContext.MailSendCounter.Where(x => x.CompanyId == token.CompanyId &&
        //                                         x.DivisionId == token.DivisionId &&
        //                                         x.DepartmentId == token.DepartmentId
        //                                         ).FirstOrDefaultAsync();

        //            if (counter != null)
        //            {
        //                _mailSend.Counter = counter.Counter;
        //                _mailSend.CompanyId = token.CompanyId;
        //                _mailSend.DivisionId = token.DivisionId;
        //                _mailSend.DepartmentId = token.DepartmentId;
        //                _mailSend.Recipient = customerUsername.CustomerEmail;
        //                _mailSend.Body = Body;
        //                _mailSend.Subject = "Password Reset";
        //                _mailSend.SenderId = "Power Employee Manager";

        //                await _DBContext.AddAsync(_mailSend);
        //                var mailInsert = _DBContext.SaveChanges();

        //                if (mailInsert > 0)
        //                {
        //                    statusMessage.Status = "Success";
        //                    statusMessage.Message = "A new password has been sent to your email.";

        //                    //update counter
        //                    counter.Counter = counter.Counter + 1;

        //                    _DBContext.Entry(counter).State = EntityState.Modified;
        //                    _DBContext.SaveChanges();

        //                }
        //                else
        //                {
        //                    statusMessage.Status = "Failed";
        //                    statusMessage.Message = "Failed To Send Mail. Try Again";
        //                }
        //            }
        //            else
        //            {
        //                statusMessage.Status = "Failed";
        //                statusMessage.Message = "Mail Not Sent";
        //            }


        //            //    else
        //            //    {
        //            //        statusMessage.Status = "Failed";
        //            //        statusMessage.Message = "Failed To Send Mail. Try Again";
        //        }   //    }
        //            //}
        //        //    else
        //        //    {
        //        //        statusMessage.Status = "Failed";
        //        //        statusMessage.Message = "Password Generation Failed";
        //        //    }
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        statusMessage.Status = "Failed";
        //        statusMessage.Message = ex.Message;
        //    }
        //    return statusMessage;
        //}
    }

}
