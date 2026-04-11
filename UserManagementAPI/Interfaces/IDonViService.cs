using KPI_Tracker_API.Models.DTOs.DonVi;

namespace KPI_Tracker_API.Interfaces
{
    public interface IDonViService
    {
        Task<IEnumerable<DonViDto>> GetAllAsync();
        Task<DonViDto?> GetByIdAsync(long id);
        Task<IEnumerable<DonViDto>> GetChildrenAsync(long donViChaId);
        Task<DonViDto> CreateAsync(CreateDonViDto dto);
        Task<DonViDto?> UpdateAsync(long id, UpdateDonViDto dto);
        Task<bool> DeleteAsync(long id);
    }
}