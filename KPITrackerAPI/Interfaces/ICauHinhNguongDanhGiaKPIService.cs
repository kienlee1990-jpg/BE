using KPITrackerAPI.DTOs.CauHinhNguongDanhGiaKPI;
using KPITrackerAPI.Responses;

namespace KPITrackerAPI.Interfaces
{
    public interface ICauHinhNguongDanhGiaKPIService
    {
        Task<List<CauHinhNguongDanhGiaKPIResponse>> GetAllAsync(CauHinhNguongDanhGiaKPIQueryDto query);

        Task<CauHinhNguongDanhGiaKPIResponse?> GetByIdAsync(long id);

        Task<long> CreateAsync(CreateCauHinhNguongDanhGiaKPIDto dto, string? username);

        Task<bool> UpdateAsync(long id, UpdateCauHinhNguongDanhGiaKPIDto dto, string? username);

        Task<bool> DeleteAsync(long id);
    }
}
