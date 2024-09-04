using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IPos : IAccount
    {
        Task<IEnumerable<PosshiftCartTable>> GetPosShiftCartSync(DateTime PeriodFrom, DateTime PeriodTo, ApiToken tokenObj);
        Task<StatusMessage> AddPosShiftCart(PosshiftCartTable posshiftCart, ApiToken tokenObj);
        Task<StatusMessage> AddPosShiftCartBatch(List<PosshiftCartTable> posshiftCartBatch, ApiToken tokenObj);
        // Task<IEnumerable<PosshiftCartTable>> GetPosshiftCart(ApiToken tokenObj);
    }
}
