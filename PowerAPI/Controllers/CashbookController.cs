using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Helper;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Clients;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Cashbook Transaction API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class CashbookController : ControllerBase
    {
        IBanks _banks;

        /// <summary>
        /// Setup Constructor
        /// </summary>
        /// <param name="cashbook"></param>
        public CashbookController(IBanks banks)
        {
            _banks = banks;
        }

        
    }
}
