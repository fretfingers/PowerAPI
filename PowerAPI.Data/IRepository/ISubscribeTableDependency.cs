using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface ISubscribeTableDependency
    {
        void SubscribeTableDependency(string connectionString);
    }
}
