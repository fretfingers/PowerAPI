using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class Paging
    {
        public PaginationMetadata PaginationMetadata { get; set; }
        public List<RMA> RmaList { get; set; }
        public List<OrderRetrieveDto> OrderList { get; set; }
        public List<OrderRetrieveDto> QuoteList { get; set; }
        public List<Data.Models.InventoryItems> InventoryList { get; set; }
        public List<CompanyBranch> BranchList { get; set; }
        public List<CurrencyTypes> CurrencyList { get; set; }
        public List<Taxes> TaxList { get; set; }
        public List<TaxGroupDetail> TaxGroupDetailList { get; set; }
        public List<TaxGroups> TaxGroupsList { get; set; }
        public List<MultipleDiscounts> MultipleDiscountList { get; set; }
        public List<MultipleDiscountGroups> MultipleDiscountGroupList { get; set; }
        public List<MultipleDiscountGroupDetail> MultipleDiscountGroupDetailList { get; set; }
        public List<Terms> TermsList { get; set; }
        public List<OrderTypes> OrderTypesList { get; set; }
        public List<LedgerAnalysis> LedgerAnalysisList { get; set; }
        public List<LedgerAnalysis2> LedgerAnalysis2List { get; set; }
        public List<PaymentTypes> PaymentTypesList { get; set; }
        public List<ArtransactionTypes> ARTransactionTypesList { get; set; }
        public List<CashbookPaymentTypes> CashbookPaymentTypesList { get; set; }
        public List<BankAccounts> BankAccountsList { get; set; }
        public List<InventoryItems> InventoryItemsList { get; set; }
        public List<InventoryByWarehouse> InventoryByWarehouseList { get; set; }
        public List<FixedAssets> FixedAssetsList { get; set; }
        public List<Projects> ProjectsList { get; set; }
        public List<LedgerChartOfAccounts> LedgerChartOfAccountList { get; set; }
        public List<CreditCardTypes> CreditCardTypesList { get; set; }
        public List<InventoryUnitOfMeasure> InventoryUOMList { get; set; }
        public List<InventoryPricingCode> InventoryPricingCodeList { get; set; }
        public List<InventoryAttributes> InventoryAttributesList { get; set; }
        public List<SurCharge> InventorySurChargeList { get; set; }
        public List<AdvertType> InventoryAdvertTypeList { get; set; }
        public List<ProductType> InventoryProductTypeList { get; set; }
        public List<CustomerInform> CustomerList { get; set; }
        public List<SalesRepresentatives> SalesRepresentativesList { get; set; }
        







    }
}
