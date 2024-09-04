using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IRequisition : IAccount
    {
        Task<IEnumerable<ViewModels.Requisitions>> GetAll(ApiToken token);
        Task<IEnumerable<ViewModels.Requisitions>> GetByEmployee(string Id, string Mode, ApiToken token);
        Task<ViewModels.Requisitions> GetById(string Id, ApiToken token);
        Task<IEnumerable<WarehouseBins>> GetWarehouseBins(ApiToken token);
        Task<IEnumerable<RequisitionsType>> GetRequisitionType(ApiToken token);
        Task<IEnumerable<Warehouses>> GetWarehouse(ApiToken token);
        Task<IEnumerable<WarehouseBins>> GetBinsByWarehouse(string Id, ApiToken token);
        Task<IEnumerable<Data.Models.InventoryItems>> GetInventoryItems(string Mode, ApiToken token); 
        Task<Paging> GetInventoryCatalog(int itemsPerPage, int PageNumber, string Mode, ApiToken token);
        Task<StatusMessage> Add(ViewModels.Requisitions requisition, string Mode, ApiToken token);
        Task<StatusMessage> Delete(ViewModels.Requisitions requisition, ApiToken token);
        Task<StatusMessage> Update(ViewModels.Requisitions requisition, ApiToken token);
        Task<StatusMessage> Approve(RequisitionAppModel requisitionAppModel, ApiToken token);
        Task<StatusMessage> ConvertToPurchase(RequisitionAppModel requisitionAppModel, ApiToken token);
        Task<StatusMessage> ConvertToIssue(RequisitionAppModel requisitionAppModel, ApiToken token);
        
    }
}
