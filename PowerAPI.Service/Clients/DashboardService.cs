using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.POCO;
using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerAPI.Data.Models;
using DevExpress.XtraRichEdit.Layout.Engine;

namespace PowerAPI.Service.Clients
{
    public class DashboardService : IDashboardService
    {
        private readonly EnterpriseContext _dbContext;
        public DashboardService(EnterpriseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICollection<OrderSummDashboard>> GetDashboardOrderSummary(string companyId, string divisionId, string departmentId)
        {
            var currentDate = DateTime.Now;
            // Fetch the order headers and necessary data
            var orders = await _dbContext.OrderHeader
                .Where(x => x.CompanyId == companyId &&
                            x.DivisionId == divisionId &&
                            x.DepartmentId == departmentId &&
                            x.TransactionTypeId == "Order")
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderNumber)
                .Take(400)
                .Select(o => new
                {
                    o.OrderNumber,
                    o.OrderDate,
                    o.CustomerId,
                    o.ShippingName,
                    o.CurrencyId,
                    o.Total,
                    o.Posted,
                    o.PostedDate,
                    o.OrderTypeId,
                    o.Backordered,
                    o.Picked,
                    o.PickedDate,
                    o.Invoiced,
                    o.InvoiceDate
                })
                .ToListAsync();

            // Transform the fetched data in-memory
            var salesItemsObj = orders.Select(i =>
            {
                var workflows = new List<OrderWorkflow>
                    {
                                new OrderWorkflow
                                {
                                    OrderNumber = i.OrderNumber,
                                    OrderDate = i.OrderDate ?? currentDate,
                                    StepSequence = 1,
                                    Status = "Booked",
                                    IsCompleted = i.Posted ?? false,
                                    DateCompleted = i.PostedDate
                                },
                                new OrderWorkflow
                                {
                                    OrderNumber = i.OrderNumber,
                                    OrderDate = i.OrderDate ?? currentDate,
                                    StepSequence = 2,
                                    Status = "On Hold",
                                    IsCompleted = i.OrderTypeId == "hold",
                                    DateCompleted = i.OrderTypeId == "hold"? i.PostedDate: null
                                },
                                new OrderWorkflow
                                {
                                    OrderNumber = i.OrderNumber,
                                    OrderDate = i.OrderDate ?? currentDate,
                                    StepSequence = 3,
                                    Status = "Back Ordered",
                                    IsCompleted = i.Backordered ?? false,
                                    DateCompleted = i.Backordered == true ? i.PostedDate : null
                                },
                                new OrderWorkflow
                                {
                                    OrderNumber = i.OrderNumber,
                                    OrderDate = i.OrderDate ?? currentDate,
                                    StepSequence = 4,
                                    Status = "Picked",
                                    IsCompleted = i.Picked ?? false,
                                    DateCompleted = i.Picked == true ? i.PickedDate : null
                                },
                                new OrderWorkflow
                                {
                                    OrderNumber = i.OrderNumber,
                                    OrderDate = i.OrderDate ?? currentDate,
                                    StepSequence = 5,
                                    Status = "Invoiced",
                                    IsCompleted = i.Invoiced ?? false,
                                    DateCompleted = i.Invoiced == true ? i.InvoiceDate : null
                                }
                    };

                var lastCompletedWorkflow = workflows
                    .Where(ow => ow.IsCompleted)
                    .OrderBy(workflow => workflow.StepSequence)
                    .LastOrDefault();

                return new OrderSummDashboard
                {
                    OrderNumber = i.OrderNumber,
                    OrderDate = i.OrderDate ?? currentDate,
                    CustomerId = i.CustomerId,
                    CustomerName = i.ShippingName,
                    CurrencyId = i.CurrencyId,
                    Status = lastCompletedWorkflow?.Status ?? "Draft",
                    Total = i.Total ?? 0
                };
            }).ToList();



            return salesItemsObj;

            //        // Step 1: Create the orderWorkflows query
            //        var orderWorkflows = _dbContext.OrderHeader
            //            .Where(x => x.CompanyId == companyId &&
            //            x.DivisionId == divisionId &&
            //            x.DepartmentId == departmentId &&
            //            x.TransactionTypeId == "Order")
            //            .OrderByDescending(o => o.OrderDate)
            //            .ThenByDescending(o => o.OrderNumber)
            //            .Take(500)
            //                .Select(o => new
            //                {
            //                    o.OrderNumber,
            //                    o.OrderDate,
            //                    o.Posted,
            //                    o.PostedDate,
            //                    o.OrderTypeId,
            //                    o.Backordered,
            //                    o.Picked,
            //                    o.PickedDate,
            //                    o.Invoiced,
            //                    o.InvoiceDate
            //                })
            //.AsEnumerable()
            //            .SelectMany(o => new[]
            //            {
            //            new OrderWorkflow
            //            {
            //                OrderNumber = o.OrderNumber,
            //                OrderDate = o.OrderDate ?? currentDate,
            //                StepSequence = 1,
            //                Status = "Booked",
            //                IsCompleted = o.Posted ?? false,
            //                DateCompleted = o.PostedDate,
            //            },
            //            new OrderWorkflow
            //            {
            //                OrderNumber = o.OrderNumber,
            //                OrderDate = o.OrderDate ?? currentDate,
            //                StepSequence = 2,
            //                Status = "On Hold",
            //                IsCompleted = o.OrderTypeId == "hold",
            //                DateCompleted = o.PostedDate
            //            },
            //            new OrderWorkflow
            //            {
            //                OrderNumber = o.OrderNumber,
            //                OrderDate = o.OrderDate ?? currentDate,
            //                StepSequence = 3,
            //                Status = "Back Ordered",
            //                IsCompleted = o.Backordered ?? false,
            //                DateCompleted = o.PostedDate
            //            },
            //            new OrderWorkflow
            //            {
            //                OrderNumber = o.OrderNumber,
            //                OrderDate = o.OrderDate ?? currentDate,
            //                StepSequence = 4,
            //                Status = "Picked",
            //                IsCompleted = o.Picked ?? false,
            //                DateCompleted = o.PickedDate
            //            },
            //            new OrderWorkflow
            //            {
            //                OrderNumber = o.OrderNumber,
            //                OrderDate = o.OrderDate ?? currentDate,
            //                StepSequence = 5,
            //                Status = "Invoiced",
            //                IsCompleted = o.Invoiced ?? false,
            //                DateCompleted = o.InvoiceDate
            //            }
            //            });

            //        // Step 2: Create Order Summary
            //        var salesItemsObj = await _dbContext.OrderHeader
            //            .Where(x => x.CompanyId == companyId &&
            //                            x.DivisionId == divisionId &&
            //                            x.DepartmentId == departmentId &&
            //                            x.TransactionTypeId == "Order")
            //            .OrderByDescending(o => o.OrderDate)
            //            .ThenByDescending(o => o.OrderNumber)
            //            .Take(500)
            //            .Select (i => new OrderSummDashboard { 
            //                OrderNumber = i.OrderNumber,
            //                OrderDate = i.OrderDate ?? currentDate,
            //                CustomerId = i.CustomerId,
            //                CustomerName = i.ShippingName,
            //                CurrencyId = i.CurrencyId,
            //                Status = orderWorkflows.Where(ow => ow.OrderNumber == i.OrderNumber && ow.IsCompleted)
            //                .OrderBy(workflow => workflow.StepSequence)
            //                .Select(workItem => workItem.Status).LastOrDefault() ?? "No completed Work Item",
            //                Total = i.Total ?? 0,
            //                WorkFlowTrail = orderWorkflows.Where(ow => ow.OrderNumber == i.OrderNumber).ToList()
            //            })
            //            .ToListAsync();




        }
    }
}
