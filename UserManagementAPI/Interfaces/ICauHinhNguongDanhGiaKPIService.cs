using KPI_Tracker_API.DTOs.CauHinhNguongDanhGiaKPI;
using KPI_Tracker_API.Responses;

namespace KPI_Tracker_API.Interfaces
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