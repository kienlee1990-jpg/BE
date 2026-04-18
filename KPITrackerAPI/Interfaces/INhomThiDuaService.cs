using KPITrackerAPI.DTOs.NhomThiDua;

namespace KPITrackerAPI.Interfaces
{
    public interface INhomThiDuaService
    {
        Task<List<NhomThiDuaDto>> GetAllAsync();
        Task<NhomThiDuaDto?> GetByIdAsync(long id);
        Task<NhomThiDuaDto> CreateAsync(CreateNhomThiDuaDto dto);
        Task<NhomThiDuaDto?> UpdateAsync(long id, UpdateNhomThiDuaDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
