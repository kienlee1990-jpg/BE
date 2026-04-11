using UserManagementAPI.Data;
using KPI_Tracker_API.DTOs.DanhMucChiTieu;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace KPI_Tracker_API.Services
{
    public class DanhMucChiTieuService : IDanhMucChiTieuService
    {
        private readonly ApplicationDbContext _context;

        public DanhMucChiTieuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DanhMucChiTieuResponseDto> CreateAsync(CreateDanhMucChiTieuDto dto)
        {
            var exists = await _context.DanhMucChiTieus
                .AnyAsync(x => x.MaChiTieu == dto.MaChiTieu);

            if (exists)
                throw new Exception("Mã chỉ tiêu đã tồn tại.");

            ValidateBusinessRules(dto.LoaiChiTieu, dto.DieuKienHoanThanh, dto.DieuKienKhongHoanThanh,
                dto.TyLePhanTramMucTieu, dto.LoaiMocSoSanh, dto.ChieuSoSanh);

            var entity = new DanhMucChiTieu
            {
                MaChiTieu = dto.MaChiTieu.Trim(),
                TenChiTieu = dto.TenChiTieu.Trim(),
                NguonChiTieu = dto.NguonChiTieu.Trim(),
                LoaiChiTieu = dto.LoaiChiTieu.Trim(),
                CapApDung = dto.CapApDung.Trim(),
                LinhVucNghiepVu = dto.LinhVucNghiepVu,
                DonViTinh = dto.DonViTinh,
                MoTa = dto.MoTa,
                HuongDanTinhToan = dto.HuongDanTinhToan,
                CoChoPhepPhanRa = dto.CoChoPhepPhanRa,
                NgayHieuLuc = dto.NgayHieuLuc,
                NgayHetHieuLuc = dto.NgayHetHieuLuc,
                DieuKienHoanThanh = dto.DieuKienHoanThanh,
                DieuKienKhongHoanThanh = dto.DieuKienKhongHoanThanh,
                TyLePhanTramMucTieu = dto.TyLePhanTramMucTieu,
                LoaiMocSoSanh = dto.LoaiMocSoSanh,
                ChieuSoSanh = dto.ChieuSoSanh,
                TrangThaiSuDung = "DANG_AP_DUNG",
                CreatedAt = DateTime.UtcNow
            };

            _context.DanhMucChiTieus.Add(entity);
            await _context.SaveChangesAsync();

            return MapToResponse(entity);
        }

        public async Task<List<DanhMucChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            string? nguonChiTieu,
            string? loaiChiTieu,
            string? capApDung,
            string? trangThaiSuDung)
        {
            var query = _context.DanhMucChiTieus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.MaChiTieu.ToLower().Contains(k) ||
                    x.TenChiTieu.ToLower().Contains(k));
            }

            if (!string.IsNullOrWhiteSpace(nguonChiTieu))
                query = query.Where(x => x.NguonChiTieu == nguonChiTieu);

            if (!string.IsNullOrWhiteSpace(loaiChiTieu))
                query = query.Where(x => x.LoaiChiTieu == loaiChiTieu);

            if (!string.IsNullOrWhiteSpace(capApDung))
                query = query.Where(x => x.CapApDung == capApDung);

            if (!string.IsNullOrWhiteSpace(trangThaiSuDung))
                query = query.Where(x => x.TrangThaiSuDung == trangThaiSuDung);

            var data = await query
                .OrderBy(x => x.MaChiTieu)
                .ToListAsync();

            return data.Select(MapToResponse).ToList();
        }

        public async Task<DanhMucChiTieuResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _context.DanhMucChiTieus.FindAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<DanhMucChiTieuResponseDto?> UpdateAsync(long id, UpdateDanhMucChiTieuDto dto)
        {
            var entity = await _context.DanhMucChiTieus.FindAsync(id);
            if (entity == null) return null;

            ValidateBusinessRules(dto.LoaiChiTieu, dto.DieuKienHoanThanh, dto.DieuKienKhongHoanThanh,
                dto.TyLePhanTramMucTieu, dto.LoaiMocSoSanh, dto.ChieuSoSanh);

            entity.TenChiTieu = dto.TenChiTieu.Trim();
            entity.NguonChiTieu = dto.NguonChiTieu.Trim();
            entity.LoaiChiTieu = dto.LoaiChiTieu.Trim();
            entity.CapApDung = dto.CapApDung.Trim();
            entity.LinhVucNghiepVu = dto.LinhVucNghiepVu;
            entity.DonViTinh = dto.DonViTinh;
            entity.MoTa = dto.MoTa;
            entity.HuongDanTinhToan = dto.HuongDanTinhToan;
            entity.CoChoPhepPhanRa = dto.CoChoPhepPhanRa;
            entity.TrangThaiSuDung = dto.TrangThaiSuDung;
            entity.NgayHieuLuc = dto.NgayHieuLuc;
            entity.NgayHetHieuLuc = dto.NgayHetHieuLuc;
            entity.DieuKienHoanThanh = dto.DieuKienHoanThanh;
            entity.DieuKienKhongHoanThanh = dto.DieuKienKhongHoanThanh;
            entity.TyLePhanTramMucTieu = dto.TyLePhanTramMucTieu;
            entity.LoaiMocSoSanh = dto.LoaiMocSoSanh;
            entity.ChieuSoSanh = dto.ChieuSoSanh;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DanhMucChiTieus.FindAsync(id);
            if (entity == null) return false;

            _context.DanhMucChiTieus.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DanhMucChiTieuResponseDto MapToResponse(DanhMucChiTieu entity)
        {
            return new DanhMucChiTieuResponseDto
            {
                Id = entity.Id,
                MaChiTieu = entity.MaChiTieu,
                TenChiTieu = entity.TenChiTieu,
                NguonChiTieu = entity.NguonChiTieu,
                LoaiChiTieu = entity.LoaiChiTieu,
                CapApDung = entity.CapApDung,
                LinhVucNghiepVu = entity.LinhVucNghiepVu,
                DonViTinh = entity.DonViTinh,
                MoTa = entity.MoTa,
                HuongDanTinhToan = entity.HuongDanTinhToan,
                CoChoPhepPhanRa = entity.CoChoPhepPhanRa,
                TrangThaiSuDung = entity.TrangThaiSuDung,
                NgayHieuLuc = entity.NgayHieuLuc,
                NgayHetHieuLuc = entity.NgayHetHieuLuc,
                DieuKienHoanThanh = entity.DieuKienHoanThanh,
                DieuKienKhongHoanThanh = entity.DieuKienKhongHoanThanh,
                TyLePhanTramMucTieu = entity.TyLePhanTramMucTieu,
                LoaiMocSoSanh = entity.LoaiMocSoSanh,
                ChieuSoSanh = entity.ChieuSoSanh,
                CreatedAt = entity.CreatedAt
            };
        }

        private static void ValidateBusinessRules(
            string loaiChiTieu,
            string? dieuKienHoanThanh,
            string? dieuKienKhongHoanThanh,
            decimal? tyLePhanTramMucTieu,
            string? loaiMocSoSanh,
            string? chieuSoSanh)
        {
            if (loaiChiTieu == "DINH_TINH")
            {
                if (string.IsNullOrWhiteSpace(dieuKienHoanThanh) ||
                    string.IsNullOrWhiteSpace(dieuKienKhongHoanThanh))
                {
                    throw new Exception("Chỉ tiêu định tính phải nhập điều kiện hoàn thành và điều kiện không hoàn thành.");
                }
            }

            if (loaiChiTieu == "DINH_LUONG_SO_SANH")
            {
                if (tyLePhanTramMucTieu == null || tyLePhanTramMucTieu <= 0)
                    throw new Exception("Chỉ tiêu định lượng so sánh phải có tỷ lệ phần trăm mục tiêu > 0.");

                if (string.IsNullOrWhiteSpace(loaiMocSoSanh))
                    throw new Exception("Chỉ tiêu định lượng so sánh phải có loại mốc so sánh.");

                if (string.IsNullOrWhiteSpace(chieuSoSanh))
                    throw new Exception("Chỉ tiêu định lượng so sánh phải có chiều so sánh.");
            }
        }
    }
}