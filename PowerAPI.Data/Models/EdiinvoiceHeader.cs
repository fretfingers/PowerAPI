﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class EdiinvoiceHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionTypeId { get; set; }
        public string EdidirectionTypeId { get; set; }
        public string EdidocumentTypeId { get; set; }
        public bool? Ediopen { get; set; }
        public bool? TransOpen { get; set; }
        public string InvoiceTypeId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? InvoiceShipDate { get; set; }
        public DateTime? InvoiceCancelDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string TaxExemptId { get; set; }
        public string TaxGroupId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerShipToId { get; set; }
        public string CustomerShipForId { get; set; }
        public string WarehouseId { get; set; }
        public bool? CustomerDropShipment { get; set; }
        public string ShippingName { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingCountry { get; set; }
        public string ShipMethodId { get; set; }
        public string EmployeeId { get; set; }
        public string TermsId { get; set; }
        public string PaymentMethodId { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? Subtotal { get; set; }
        public double? DiscountPers { get; set; }
        public decimal? DiscountAmount { get; set; }
        public double? TaxPercent { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TaxableSubTotal { get; set; }
        public decimal? Freight { get; set; }
        public bool? TaxFreight { get; set; }
        public decimal? Handling { get; set; }
        public decimal? Advertising { get; set; }
        public decimal? Total { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? BalanceDue { get; set; }
        public decimal? UndistributedAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? CommissionableSales { get; set; }
        public decimal? ComissionalbleCost { get; set; }
        public string GlsalesAccount { get; set; }
        public string CheckNumber { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CreditCardTypeId { get; set; }
        public string CreditCardName { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime? CreditCardExpDate { get; set; }
        public string CreditCardCsvnumber { get; set; }
        public string CreditCardBillToZip { get; set; }
        public string CreditCardValidationCode { get; set; }
        public string CreditCardApprovalNumber { get; set; }
        public bool? Picked { get; set; }
        public DateTime? PickedDate { get; set; }
        public bool? Billed { get; set; }
        public DateTime? BilledDate { get; set; }
        public bool? Printed { get; set; }
        public DateTime? PrintedDate { get; set; }
        public bool? Shipped { get; set; }
        public DateTime? ShipDate { get; set; }
        public string TrackingNumber { get; set; }
        public bool? Backordered { get; set; }
        public bool? Posted { get; set; }
        public DateTime? PostedDate { get; set; }
        public string HeaderMemo1 { get; set; }
        public string HeaderMemo2 { get; set; }
        public string HeaderMemo3 { get; set; }
        public string HeaderMemo4 { get; set; }
        public string HeaderMemo5 { get; set; }
        public string HeaderMemo6 { get; set; }
        public string HeaderMemo7 { get; set; }
        public string HeaderMemo8 { get; set; }
        public string HeaderMemo9 { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? AllowanceDiscountPerc { get; set; }
        public string BranchCode { get; set; }
    }
}
