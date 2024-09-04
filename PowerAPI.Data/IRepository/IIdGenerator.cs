namespace PowerAPI.Data.IRepository
{
    public interface IIdGenerator
    {
        string GenerateId(string companyId, string divisionId, string departmentId, string username);
        string[] SplitId(string id);
    }
}