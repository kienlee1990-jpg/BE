using KPI_Tracker_API.Models.DTOs.TheoDoiThucHienKPI;

namespace KPI_Tracker_API.Interfaces
{
    public interface ITheoDoiThucHienKPIService
    {
        Task<IEnumerable<TheoDoiThucHienKPIDto>> GetAllAsync();
        Task<TheoDoiThucHienKPIDto?> GetByIdAsync(long id);
        Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByChiTietGiaoChiTieuIdAsync(long chiTietGiaoChiTieuId);
        Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByKyBaoCaoKPIIdAsync(long kyBaoCaoKPIId);
        Task<TheoDoiThucHienKPIDto?> GetByChiTietVaKyAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId);
        Task<TheoDoiThucHienKPIDto> CreateAsync(CreateTheoDoiThucHienKPIDto dto);
        Task<TheoDoiThucHienKPIDto?> UpdateAsync(long id, UpdateTheoDoiThucHienKPIDto dto);
        Task<bool> DeleteAsync(long id);
    }
}