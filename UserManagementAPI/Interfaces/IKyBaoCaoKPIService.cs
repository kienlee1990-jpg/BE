using KPI_Tracker_API.Models.DTOs.KyBaoCaoKPI;

namespace KPI_Tracker_API.Interfaces
{
    public interface IKyBaoCaoKPIService
    {
        Task<IEnumerable<KyBaoCaoKPIDto>> GetAllAsync();
        Task<KyBaoCaoKPIDto?> GetByIdAsync(long id);
        Task<IEnumerable<KyBaoCaoKPIDto>> GetByNamAsync(int nam);
        Task<IEnumerable<KyBaoCaoKPIDto>> GetByLoaiKyAsync(string loaiKy);
        Task<KyBaoCaoKPIDto> CreateAsync(CreateKyBaoCaoKPIDto dto);
        Task<KyBaoCaoKPIDto?> UpdateAsync(long id, UpdateKyBaoCaoKPIDto dto);
        Task<bool> DeleteAsync(long id);
    }
}