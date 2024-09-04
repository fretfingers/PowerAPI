using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Service.POCO
{
    public class PayInitialize
    {
        public string email { get; set; }
        public double amount { get; set; }
        public string reference { get; set; }
    }
}
