using KPI_Tracker_API.DTOs;
using KPI_Tracker_API.DTOs.DotGiaoChiTieu;

namespace KPI_Tracker_API.Interfaces
{
    public interface IDotGiaoChiTieuService
    {
        Task<DotGiaoChiTieuResponseDto> CreateAsync(CreateDotGiaoChiTieuDto dto);

        Task<List<DotGiaoChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            int? namApDung,
            string? nguonDotGiao,
            string? capGiao,
            string? trangThai);

        Task<DotGiaoChiTieuResponseDto?> GetByIdAsync(long id);
        Task<DotGiaoChiTieuResponseDto?> UpdateAsync(long id, UpdateDotGiaoChiTieuDto dto);
        Task<bool> DeleteAsync(long id);
    }
}