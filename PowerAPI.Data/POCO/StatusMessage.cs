﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerAPI.Data.POCO
{
    public class StatusMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic data { get; set; }

        public string auth_token { get; set; }
    }

    //public class ResultData
    //{
    //    public dynamic data { get; set; }
    //    public string auth_token { get; set; }
    //}
}
