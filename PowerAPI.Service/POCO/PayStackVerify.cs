using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PowerAPI.Service.POCO
{
    public partial class PayStackVerify
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaymentInfo data { get; set; }
    }

    public partial class PaymentInfo
    {
        public long Id { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public long Amount { get; set; }
        public object Message { get; set; }
        public string GatewayResponse { get; set; }
        public object DataPaidAt { get; set; }
        public DateTimeOffset DataCreatedAt { get; set; }
        public string Channel { get; set; }
        public string Currency { get; set; }
        public string IpAddress { get; set; }
        public string Metadata { get; set; }
        public object Log { get; set; }
        public object Fees { get; set; }
        public object FeesSplit { get; set; }
        public Authorization Authorization { get; set; }
        public Customer Customer { get; set; }
        public object Plan { get; set; }
        public Authorization Split { get; set; }
        public object OrderId { get; set; }
        public object PaidAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long RequestedAmount { get; set; }
        public object PosTransactionData { get; set; }
        public object Source { get; set; }
        public object FeesBreakdown { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public Authorization PlanObject { get; set; }
        public Authorization Subaccount { get; set; }
    }

    public partial class Authorization
    {
    }

    public partial class Customer
    {
        public long Id { get; set; }
        public object FirstName { get; set; }
        public object LastName { get; set; }
        public string Email { get; set; }
        public string CustomerCode { get; set; }
        public object Phone { get; set; }
        public object Metadata { get; set; }
        public string RiskAction { get; set; }
        public object InternationalFormatPhone { get; set; }
    }

    public partial class PayStackVerify
    {
        public static PayStackVerify FromJson(string json) => JsonConvert.DeserializeObject<PayStackVerify>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PayStackVerify self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
        };
    }
    }


