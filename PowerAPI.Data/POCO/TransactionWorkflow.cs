using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.POCO
{
    public class TransactionWorkflow
    {
            public string TransactionId { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Status { get; set; }
            public int StepSequence { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime? DateCompleted { get; set; }
        
    }
}
