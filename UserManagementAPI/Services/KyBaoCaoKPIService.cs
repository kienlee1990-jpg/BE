using UserManagementAPI.Data;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Interfaces;
using KPI_Tracker_API.Models.DTOs.KyBaoCaoKPI;
using Microsoft.EntityFrameworkCore;
using System;

namespace KPI_Tracker_API.Services
{
    public class KyBaoCaoKPIService : IKyBaoCaoKPIService
    {
        private readonly ApplicationDbContext _context;

        public KyBaoCaoKPIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KyBaoCaoKPIDto>> GetAllAsync()
        {
            var items = await _context.KyBaoCaoKPIs
                .OrderByDescending(x => x.Nam)
                .ThenBy(x => x.LoaiKy)
                .ThenBy(x => x.SoKy)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<KyBaoCaoKPIDto?> GetByIdAsync(long id)
        {
            var item = await _context.KyBaoCaoKPIs.FirstOrDefaultAsync(x => x.Id == id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<KyBaoCaoKPIDto>> GetByNamAsync(int nam)
        {
            var items = await _context.KyBaoCaoKPIs
                .Where(x => x.Nam == nam)
                .OrderBy(x => x.LoaiKy)
                .ThenBy(x => x.SoKy)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<KyBaoCaoKPIDto>> GetByLoaiKyAsync(string loaiKy)
        {
            loaiKy = loaiKy.Trim().ToUpper();

            var items = await _context.KyBaoCaoKPIs
                .Where(x => x.LoaiKy.ToUpper() == loaiKy)
                .OrderByDescending(x => x.Nam)
                .ThenBy(x => x.SoKy)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<KyBaoCaoKPIDto> CreateAsync(CreateKyBaoCaoKPIDto dto)
        {
            await ValidateBeforeSave(
                dto.MaKy,
                dto.LoaiKy,
                dto.Nam,
                dto.SoKy,
                dto.TuNgay,
                dto.DenNgay,
                dto.NgayDauKy,
                dto.NgayCuoiKy,
                null);

            var entity = new KyBaoCaoKPI
            {
                MaKy = dto.MaKy.Trim(),
                TenKy = dto.TenKy.Trim(),
                LoaiKy = dto.LoaiKy.Trim().ToUpper(),
                Nam = dto.Nam,
                SoKy = dto.SoKy,
                TuNgay = dto.TuNgay,
                DenNgay = dto.DenNgay,
                NgayDauKy = dto.NgayDauKy,
                NgayCuoiKy = dto.NgayCuoiKy,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "MOI_TAO" : dto.TrangThai.Trim().ToUpper(),
                GhiChu = dto.GhiChu,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy
            };

            _context.KyBaoCaoKPIs.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<KyBaoCaoKPIDto?> UpdateAsync(long id, UpdateKyBaoCaoKPIDto dto)
        {
            var entity = await _context.KyBaoCaoKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            await ValidateBeforeSave(
                dto.MaKy,
                dto.LoaiKy,
                dto.Nam,
                dto.SoKy,
                dto.TuNgay,
                dto.DenNgay,
                dto.NgayDauKy,
                dto.NgayCuoiKy,
                id);

            entity.MaKy = dto.MaKy.Trim();
            entity.TenKy = dto.TenKy.Trim();
            entity.LoaiKy = dto.LoaiKy.Trim().ToUpper();
            entity.Nam = dto.Nam;
            entity.SoKy = dto.SoKy;
            entity.TuNgay = dto.TuNgay;
            entity.DenNgay = dto.DenNgay;
            entity.NgayDauKy = dto.NgayDauKy;
            entity.NgayCuoiKy = dto.NgayCuoiKy;
            entity.TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? entity.TrangThai : dto.TrangThai.Trim().ToUpper();
            entity.GhiChu = dto.GhiChu;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = dto.UpdatedBy;

            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.KyBaoCaoKPIs
                .Include(x => x.TheoDoiThucHienKPIs)
                .Include(x => x.DanhGiaKPIs)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            if (entity.TheoDoiThucHienKPIs.Any())
                throw new Exception("Không thể xóa vì kỳ báo cáo này đang được sử dụng trong theo dõi thực hiện KPI.");

            if (entity.DanhGiaKPIs.Any())
                throw new Exception("Không thể xóa vì kỳ báo cáo này đang được sử dụng trong đánh giá KPI.");

            _context.KyBaoCaoKPIs.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task ValidateBeforeSave(
            string maKy,
            string loaiKy,
            int nam,
            int? soKy,
            DateTime tuNgay,
            DateTime denNgay,
            DateTime ngayDauKy,
            DateTime ngayCuoiKy,
            long? currentId)
        {
            maKy = maKy.Trim();
            loaiKy = loaiKy.Trim().ToUpper();

            var allowedLoaiKy = new[] { "THANG", "QUY", "NAM", "6THANG" };
            if (!allowedLoaiKy.Contains(loaiKy))
                throw new Exception("LoaiKy chỉ được phép là THANG, QUY, NAM hoặc 6THANG.");

            if (tuNgay > denNgay)
                throw new Exception("TuNgay không được lớn hơn DenNgay.");

            if (ngayDauKy > ngayCuoiKy)
                throw new Exception("NgayDauKy không được lớn hơn NgayCuoiKy.");

            var existedMaKy = await _context.KyBaoCaoKPIs.AnyAsync(x =>
                x.MaKy == maKy && (!currentId.HasValue || x.Id != currentId.Value));

            if (existedMaKy)
                throw new Exception("Mã kỳ đã tồn tại.");

            var existedLoaiKyNamSoKy = await _context.KyBaoCaoKPIs.AnyAsync(x =>
                x.LoaiKy == loaiKy &&
                x.Nam == nam &&
                x.SoKy == soKy &&
                (!currentId.HasValue || x.Id != currentId.Value));

            if (existedLoaiKyNamSoKy)
                throw new Exception("Kỳ báo cáo cùng LoaiKy, Nam, SoKy đã tồn tại.");

            if (loaiKy == "NAM" && soKy.HasValue)
                throw new Exception("Kỳ năm không cần SoKy.");

            if ((loaiKy == "THANG" || loaiKy == "QUY" || loaiKy == "6THANG") && !soKy.HasValue)
                throw new Exception("LoaiKy này bắt buộc phải có SoKy.");

            if (loaiKy == "THANG" && (soKy < 1 || soKy > 12))
                throw new Exception("SoKy của THANG phải từ 1 đến 12.");

            if (loaiKy == "QUY" && (soKy < 1 || soKy > 4))
                throw new Exception("SoKy của QUY phải từ 1 đến 4.");

            if (loaiKy == "6THANG" && (soKy < 1 || soKy > 2))
                throw new Exception("SoKy của 6THANG phải từ 1 đến 2.");
        }

        private static KyBaoCaoKPIDto MapToDto(KyBaoCaoKPI x)
        {
            return new KyBaoCaoKPIDto
            {
                Id = x.Id,
                MaKy = x.MaKy,
                TenKy = x.TenKy,
                LoaiKy = x.LoaiKy,
                Nam = x.Nam,
                SoKy = x.SoKy,
                TuNgay = x.TuNgay,
                DenNgay = x.DenNgay,
                NgayDauKy = x.NgayDauKy,
                NgayCuoiKy = x.NgayCuoiKy,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy
            };
        }
    }
}