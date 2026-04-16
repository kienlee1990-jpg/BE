using KPITrackerAPI.DTOs.DanhGiaKPI;

namespace KPITrackerAPI.Interfaces
{
    public interface IDanhGiaKPIService
    {
        Task<IEnumerable<DanhGiaKPIDto>> GetAllAsync();
        Task<DanhGiaKPIDto?> GetByIdAsync(long id);
        Task<IEnumerable<DanhGiaKPIDto>> GetByChiTietGiaoChiTieuIdAsync(long chiTietGiaoChiTieuId);
        Task<IEnumerable<DanhGiaKPIDto>> GetByKyBaoCaoKPIIdAsync(long kyBaoCaoKPIId);

        Task<DanhGiaKPIDto> UpsertDanhGiaKPIAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId, string? username);
        Task<bool> DeleteDanhGiaKPIAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId);
        Task SynchronizeDanhGiaForChiTietAsync(long chiTietGiaoChiTieuId, string? username);
    }
}
