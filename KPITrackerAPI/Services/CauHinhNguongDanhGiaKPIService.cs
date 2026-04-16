using KPITrackerAPI.Constants;
using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.CauHinhNguongDanhGiaKPI;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Responses;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class CauHinhNguongDanhGiaKPIService : ICauHinhNguongDanhGiaKPIService
    {
        private readonly ApplicationDbContext _context;

        public CauHinhNguongDanhGiaKPIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CauHinhNguongDanhGiaKPIResponse>> GetAllAsync(CauHinhNguongDanhGiaKPIQueryDto query)
        {
            var q = _context.CauHinhNguongDanhGiaKPIs
                .Include(x => x.DanhMucChiTieu)
                .AsQueryable();

            if (query.DanhMucChiTieuId.HasValue)
            {
                q = q.Where(x => x.DanhMucChiTieuId == query.DanhMucChiTieuId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();

                q = q.Where(x =>
                    x.XepLoai.Contains(keyword) ||
                    x.DieuKienThoiHan.Contains(keyword) ||
                    (x.DanhMucChiTieu != null &&
                     (x.DanhMucChiTieu.MaChiTieu.Contains(keyword) ||
                      x.DanhMucChiTieu.TenChiTieu.Contains(keyword))) ||
                    (x.GhiChu != null && x.GhiChu.Contains(keyword)));
            }

            return await q
                .OrderBy(x => x.DanhMucChiTieuId.HasValue ? 0 : 1)
                .ThenBy(x => x.DieuKienThoiHan)
                .ThenBy(x => x.TuTyLe)
                .Select(x => new CauHinhNguongDanhGiaKPIResponse
                {
                    Id = x.Id,
                    DanhMucChiTieuId = x.DanhMucChiTieuId,
                    TenChiTieu = x.DanhMucChiTieu != null ? x.DanhMucChiTieu.TenChiTieu : null,
                    TuTyLe = x.TuTyLe,
                    DenTyLe = x.DenTyLe,
                    XepLoai = x.XepLoai,
                    DieuKienThoiHan = x.DieuKienThoiHan,
                    Diem = x.Diem,
                    GhiChu = x.GhiChu
                })
                .ToListAsync();
        }

        public async Task<CauHinhNguongDanhGiaKPIResponse?> GetByIdAsync(long id)
        {
            return await _context.CauHinhNguongDanhGiaKPIs
                .Include(x => x.DanhMucChiTieu)
                .Where(x => x.Id == id)
                .Select(x => new CauHinhNguongDanhGiaKPIResponse
                {
                    Id = x.Id,
                    DanhMucChiTieuId = x.DanhMucChiTieuId,
                    TenChiTieu = x.DanhMucChiTieu != null ? x.DanhMucChiTieu.TenChiTieu : null,
                    TuTyLe = x.TuTyLe,
                    DenTyLe = x.DenTyLe,
                    XepLoai = x.XepLoai,
                    DieuKienThoiHan = x.DieuKienThoiHan,
                    Diem = x.Diem,
                    GhiChu = x.GhiChu
                })
                .FirstOrDefaultAsync();
        }

        public async Task<long> CreateAsync(CreateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            var normalizedXepLoai = await ValidateAndNormalizeAsync(dto);

            var entity = new CauHinhNguongDanhGiaKPI
            {
                DanhMucChiTieuId = dto.DanhMucChiTieuId,
                TuTyLe = dto.TuTyLe,
                DenTyLe = dto.DenTyLe,
                XepLoai = normalizedXepLoai,
                DieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan),
                Diem = dto.Diem,
                GhiChu = NormalizeNullable(dto.GhiChu),
                CreatedAt = DateTime.UtcNow
            };

            _context.CauHinhNguongDanhGiaKPIs.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(long id, UpdateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            var normalizedXepLoai = await ValidateAndNormalizeAsync(dto, id);

            entity.DanhMucChiTieuId = dto.DanhMucChiTieuId;
            entity.TuTyLe = dto.TuTyLe;
            entity.DenTyLe = dto.DenTyLe;
            entity.XepLoai = normalizedXepLoai;
            entity.DieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan);
            entity.Diem = dto.Diem;
            entity.GhiChu = NormalizeNullable(dto.GhiChu);
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            _context.CauHinhNguongDanhGiaKPIs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> ValidateAndNormalizeAsync(CreateCauHinhNguongDanhGiaKPIDto dto, long? currentId = null)
        {
            if (dto == null)
            {
                throw new Exception("Dữ liệu gửi lên không hợp lệ.");
            }

            var normalizedXepLoai = DanhGiaKPIConstants.NormalizeCode(dto.XepLoai);
            var normalizedDieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan);

            if (string.IsNullOrWhiteSpace(normalizedXepLoai))
            {
                throw new Exception("Xếp loại không được để trống.");
            }

            if (!DanhGiaKPIConstants.AllowedXepLoai.Contains(normalizedXepLoai))
            {
                throw new Exception("Xếp loại không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(normalizedDieuKienThoiHan))
            {
                throw new Exception("Điều kiện thời hạn không được để trống.");
            }

            if (!DanhGiaKPIConstants.AllowedDieuKienThoiHan.Contains(normalizedDieuKienThoiHan))
            {
                throw new Exception("Điều kiện thời hạn không hợp lệ.");
            }

            if (dto.TuTyLe > dto.DenTyLe)
            {
                throw new Exception("Tỷ lệ bắt đầu không được lớn hơn tỷ lệ kết thúc.");
            }

            if (dto.DanhMucChiTieuId.HasValue)
            {
                var existsDanhMuc = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == dto.DanhMucChiTieuId.Value);
                if (!existsDanhMuc)
                {
                    throw new Exception("Danh mục chỉ tiêu không tồn tại.");
                }
            }

            ValidateBusinessRules(normalizedXepLoai, normalizedDieuKienThoiHan);

            var isDuplicate = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                (!currentId.HasValue || x.Id != currentId.Value) &&
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.TuTyLe == dto.TuTyLe &&
                x.DenTyLe == dto.DenTyLe &&
                x.XepLoai == normalizedXepLoai &&
                x.DieuKienThoiHan == normalizedDieuKienThoiHan);

            if (isDuplicate)
            {
                throw new Exception("Cấu hình ngưỡng đánh giá đã tồn tại.");
            }

            var hasOverlap = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                (!currentId.HasValue || x.Id != currentId.Value) &&
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.DieuKienThoiHan == normalizedDieuKienThoiHan &&
                x.TuTyLe <= dto.DenTyLe &&
                x.DenTyLe >= dto.TuTyLe);

            if (hasOverlap)
            {
                throw new Exception("Khoảng tỷ lệ đang bị chồng lấn với cấu hình ngưỡng khác cùng điều kiện thời hạn.");
            }

            return normalizedXepLoai;
        }

        private static void ValidateBusinessRules(string xepLoai, string dieuKienThoiHan)
        {
            if (xepLoai == DanhGiaKPIConstants.XepLoai.KhongHoanThanh &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan)
            {
                throw new Exception("Trạng thái không hoàn thành chỉ áp dụng khi đã đến hạn.");
            }

            if (xepLoai == DanhGiaKPIConstants.XepLoai.ChuaHoanThanh &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan)
            {
                throw new Exception("Trạng thái chưa hoàn thành chỉ áp dụng khi chưa đến hạn.");
            }

            if ((xepLoai == DanhGiaKPIConstants.XepLoai.HoanThanh ||
                 xepLoai == DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc) &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.MacDinh)
            {
                throw new Exception("Trạng thái hoàn thành chỉ sử dụng điều kiện thời hạn mặc định.");
            }
        }

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
