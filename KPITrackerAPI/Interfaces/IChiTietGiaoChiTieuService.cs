using KPITrackerAPI.DTOs.ChiTietGiaoChiTieu;

namespace KPITrackerAPI.Interfaces
{
    public interface IChiTietGiaoChiTieuService
    {
        Task<List<ChiTietGiaoChiTieuDto>> GetAllAsync();
        Task<ChiTietGiaoChiTieuDto?> GetByIdAsync(long id);
        Task<List<ChiTietGiaoChiTieuDto>> GetByDotGiaoChiTieuIdAsync(long dotGiaoChiTieuId);
        Task<List<ChiTietGiaoChiTieuDto>> GetByDonViNhanIdAsync(long donViNhanId);
        Task<List<ChiTietGiaoChiTieuDto>> GetChildrenAsync(long chiTietGiaoChaId);

        Task<ChiTietGiaoChiTieuDto> CreateAsync(CreateChiTietGiaoChiTieuDto dto);
        Task<ChiTietGiaoChiTieuDto?> UpdateAsync(long id, UpdateChiTietGiaoChiTieuDto dto);
        Task<bool> DeleteAsync(long id);
    }
}

