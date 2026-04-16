using KPITrackerAPI.DTOs.DanhMucChiTieu;

namespace KPITrackerAPI.Interfaces
{
    public interface IDanhMucChiTieuService
    {
        Task<DanhMucChiTieuResponseDto> CreateAsync(CreateDanhMucChiTieuDto dto);
        Task<List<DanhMucChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            string? nguonChiTieu,
            string? loaiChiTieu,
            string? capApDung,
            string? trangThaiSuDung,
            bool? coChoPhepPhanRa);

        Task<DanhMucChiTieuResponseDto?> GetByIdAsync(long id);
        Task<DanhMucChiTieuResponseDto?> UpdateAsync(long id, UpdateDanhMucChiTieuDto dto);
        Task<bool> DeleteAsync(long id);
    }
}

