namespace KPITrackerAPI.Interfaces
{
    public interface IUserAccessScopeService
    {
        Task<long?> GetCurrentOwnerScopeDonViIdAsync();
    }
}
