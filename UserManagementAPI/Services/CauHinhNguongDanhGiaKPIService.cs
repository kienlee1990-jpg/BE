using KPI_Tracker_API.DTOs.CauHinhNguongDanhGiaKPI;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Interfaces;
using KPI_Tracker_API.Responses;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;

namespace KPI_Tracker_API.Services
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
                q = q.Where(x => x.DanhMucChiTieuId == query.DanhMucChiTieuId.Value);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();

                q = q.Where(x =>
                    x.XepLoai.Contains(keyword)
                    ||
                    (x.DanhMucChiTieu != null &&
                     (x.DanhMucChiTieu.MaChiTieu.Contains(keyword) ||
                      x.DanhMucChiTieu.TenChiTieu.Contains(keyword)))
                    ||
                    (x.GhiChu != null && x.GhiChu.Contains(keyword))
                );
            }

            return await q
                .OrderBy(x => x.TuTyLe)
                .Select(x => new CauHinhNguongDanhGiaKPIResponse
                {
                    Id = x.Id,
                    DanhMucChiTieuId = x.DanhMucChiTieuId,
                    TenChiTieu = x.DanhMucChiTieu != null ? x.DanhMucChiTieu.TenChiTieu : null,
                    TuTyLe = x.TuTyLe,
                    DenTyLe = x.DenTyLe,
                    XepLoai = x.XepLoai,
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
                    Diem = x.Diem,
                    GhiChu = x.GhiChu
                })
                .FirstOrDefaultAsync();
        }

        public async Task<long> CreateAsync(CreateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            if (dto == null)
                throw new Exception("Dữ liệu gửi lên không hợp lệ.");

            if (string.IsNullOrWhiteSpace(dto.XepLoai))
                throw new Exception("Xếp loại không được để trống.");

            if (dto.TuTyLe > dto.DenTyLe)
                throw new Exception("Tỷ lệ bắt đầu không được lớn hơn tỷ lệ kết thúc.");

            if (dto.DanhMucChiTieuId.HasValue)
            {
                var existsDanhMuc = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == dto.DanhMucChiTieuId.Value);
                if (!existsDanhMuc)
                    throw new Exception("Danh mục chỉ tiêu không tồn tại.");
            }

            var isDuplicate = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.TuTyLe == dto.TuTyLe &&
                x.DenTyLe == dto.DenTyLe &&
                x.XepLoai == dto.XepLoai.Trim());

            if (isDuplicate)
                throw new Exception("Cấu hình ngưỡng đánh giá đã tồn tại.");

            var entity = new CauHinhNguongDanhGiaKPI
            {
                DanhMucChiTieuId = dto.DanhMucChiTieuId,
                TuTyLe = dto.TuTyLe,
                DenTyLe = dto.DenTyLe,
                XepLoai = dto.XepLoai.Trim(),
                Diem = dto.Diem,
                GhiChu = dto.GhiChu,
                CreatedAt = DateTime.UtcNow
            };

            _context.CauHinhNguongDanhGiaKPIs.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(long id, UpdateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            if (dto == null)
                throw new Exception("Dữ liệu gửi lên không hợp lệ.");

            if (string.IsNullOrWhiteSpace(dto.XepLoai))
                throw new Exception("Xếp loại không được để trống.");

            if (dto.TuTyLe > dto.DenTyLe)
                throw new Exception("Tỷ lệ bắt đầu không được lớn hơn tỷ lệ kết thúc.");

            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;

            if (dto.DanhMucChiTieuId.HasValue)
            {
                var existsDanhMuc = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == dto.DanhMucChiTieuId.Value);
                if (!existsDanhMuc)
                    throw new Exception("Danh mục chỉ tiêu không tồn tại.");
            }

            var isDuplicate = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                x.Id != id &&
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.TuTyLe == dto.TuTyLe &&
                x.DenTyLe == dto.DenTyLe &&
                x.XepLoai == dto.XepLoai.Trim());

            if (isDuplicate)
                throw new Exception("Cấu hình ngưỡng đánh giá đã tồn tại.");

            entity.DanhMucChiTieuId = dto.DanhMucChiTieuId;
            entity.TuTyLe = dto.TuTyLe;
            entity.DenTyLe = dto.DenTyLe;
            entity.XepLoai = dto.XepLoai.Trim();
            entity.Diem = dto.Diem;
            entity.GhiChu = dto.GhiChu;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;

            _context.CauHinhNguongDanhGiaKPIs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}