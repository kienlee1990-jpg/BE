using KPI_Tracker_API.DTOs.DanhMucChiTieu;

namespace KPI_Tracker_API.Interfaces
{
    public interface IDanhMucChiTieuService
    {
        Task<DanhMucChiTieuResponseDto> CreateAsync(CreateDanhMucChiTieuDto dto);
        Task<List<DanhMucChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            string? nguonChiTieu,
            string? loaiChiTieu,
            string? capApDung,
            string? trangThaiSuDung);

        Task<DanhMucChiTieuResponseDto?> GetByIdAsync(long id);
        Task<DanhMucChiTieuResponseDto?> UpdateAsync(long id, UpdateDanhMucChiTieuDto dto);
        Task<bool> DeleteAsync(long id);
    }
}