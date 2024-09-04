using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

namespace PowerAPI.Service.POCO
{
    public partial class PayStackInitialize
    {
        public bool status { get; set; }
        public string message { get; set; }
        public auth data { get; set; }
    }
    public partial class auth
    {
        public Uri authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
    public partial class PayStackInitialize
    {
        public static PayStackInitialize FromJson(string json) => JsonConvert.DeserializeObject<PayStackInitialize>(json, Converter.Settings);
    }

    public static class Serializer
    {
        public static string ToJson(this PayStackInitialize self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

 
}


