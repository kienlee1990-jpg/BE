using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Interfaces;
using KPI_Tracker_API.Models.DTOs.TheoDoiThucHienKPI;
using KPI_Tracker_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;

namespace KPI_Tracker_API.Services
{
    public class TheoDoiThucHienKPIService : ITheoDoiThucHienKPIService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDanhGiaKPIService _danhGiaKPIService;

        public TheoDoiThucHienKPIService(
            ApplicationDbContext context,
            IDanhGiaKPIService danhGiaKPIService)
        {
            _context = context;
            _danhGiaKPIService = danhGiaKPIService;
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetAllAsync()
        {
            var items = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => x.KyBaoCaoKPI!.LoaiKy)
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<TheoDoiThucHienKPIDto?> GetByIdAsync(long id)
        {
            var item = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByChiTietGiaoChiTieuIdAsync(long chiTietGiaoChiTieuId)
        {
            var items = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => x.KyBaoCaoKPI!.LoaiKy)
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByKyBaoCaoKPIIdAsync(long kyBaoCaoKPIId)
        {
            var items = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.KyBaoCaoKPIId == kyBaoCaoKPIId)
                .OrderBy(x => x.ChiTietGiaoChiTieuId)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<TheoDoiThucHienKPIDto?> GetByChiTietVaKyAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId)
        {
            var item = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    x.KyBaoCaoKPIId == kyBaoCaoKPIId);

            return item == null ? null : MapToDto(item);
        }

        public async Task<TheoDoiThucHienKPIDto> CreateAsync(CreateTheoDoiThucHienKPIDto dto)
        {
            await ValidateBeforeSave(
                dto.ChiTietGiaoChiTieuId,
                dto.KyBaoCaoKPIId,
                dto.GiaTriDauKy,
                dto.GiaTriThucHienTrongKy,
                null);

            var giaTriDauKy = dto.GiaTriDauKy ?? 0;
            var giaTriThucHienTrongKy = dto.GiaTriThucHienTrongKy ?? 0;
            var giaTriCuoiKy = giaTriDauKy + giaTriThucHienTrongKy;
            var giaTriLuyKe = await TinhGiaTriLuyKeAsync(
                dto.ChiTietGiaoChiTieuId,
                dto.KyBaoCaoKPIId,
                giaTriCuoiKy,
                null);

            var entity = new TheoDoiThucHienKPI
            {
                ChiTietGiaoChiTieuId = dto.ChiTietGiaoChiTieuId,
                KyBaoCaoKPIId = dto.KyBaoCaoKPIId,
                GiaTriDauKy = giaTriDauKy,
                GiaTriThucHienTrongKy = giaTriThucHienTrongKy,
                GiaTriCuoiKy = giaTriCuoiKy,
                GiaTriLuyKe = giaTriLuyKe,
                NhanXet = dto.NhanXet?.Trim(),
                TrangThai = "MOI_TAO",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            _context.TheoDoiThucHienKPIs.Add(entity);
            await _context.SaveChangesAsync();

            await _danhGiaKPIService.UpsertDanhGiaKPIAsync(
                entity.ChiTietGiaoChiTieuId,
                entity.KyBaoCaoKPIId,
                entity.CreatedBy
            );

            var created = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(created);
        }

        public async Task<TheoDoiThucHienKPIDto?> UpdateAsync(long id, UpdateTheoDoiThucHienKPIDto dto)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            await ValidateBeforeSave(
                dto.ChiTietGiaoChiTieuId,
                dto.KyBaoCaoKPIId,
                dto.GiaTriDauKy,
                dto.GiaTriThucHienTrongKy,
                id);

            var giaTriDauKy = dto.GiaTriDauKy ?? 0;
            var giaTriThucHienTrongKy = dto.GiaTriThucHienTrongKy ?? 0;
            var giaTriCuoiKy = giaTriDauKy + giaTriThucHienTrongKy;
            var giaTriLuyKe = await TinhGiaTriLuyKeAsync(
                dto.ChiTietGiaoChiTieuId,
                dto.KyBaoCaoKPIId,
                giaTriCuoiKy,
                id);

            entity.ChiTietGiaoChiTieuId = dto.ChiTietGiaoChiTieuId;
            entity.KyBaoCaoKPIId = dto.KyBaoCaoKPIId;
            entity.GiaTriDauKy = giaTriDauKy;
            entity.GiaTriThucHienTrongKy = giaTriThucHienTrongKy;
            entity.GiaTriCuoiKy = giaTriCuoiKy;
            entity.GiaTriLuyKe = giaTriLuyKe;
            entity.NhanXet = dto.NhanXet?.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();

            await _danhGiaKPIService.UpsertDanhGiaKPIAsync(
                entity.ChiTietGiaoChiTieuId,
                entity.KyBaoCaoKPIId,
                entity.UpdatedBy
            );

            var updated = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            await _danhGiaKPIService.DeleteDanhGiaKPIAsync(
                entity.ChiTietGiaoChiTieuId,
                entity.KyBaoCaoKPIId
            );

            _context.TheoDoiThucHienKPIs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task ValidateBeforeSave(
            long chiTietGiaoChiTieuId,
            long kyBaoCaoKPIId,
            decimal? giaTriDauKy,
            decimal? giaTriThucHienTrongKy,
            long? currentId)
        {
            var chiTietExists = await _context.ChiTietGiaoChiTieus
                .AnyAsync(x => x.Id == chiTietGiaoChiTieuId);
            if (!chiTietExists)
                throw new Exception("ChiTietGiaoChiTieuId không tồn tại.");

            var kyBaoCaoExists = await _context.KyBaoCaoKPIs
                .AnyAsync(x => x.Id == kyBaoCaoKPIId);
            if (!kyBaoCaoExists)
                throw new Exception("KyBaoCaoKPIId không tồn tại.");

            var duplicated = await _context.TheoDoiThucHienKPIs.AnyAsync(x =>
                x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                x.KyBaoCaoKPIId == kyBaoCaoKPIId &&
                (!currentId.HasValue || x.Id != currentId.Value));

            if (duplicated)
                throw new Exception("Đã tồn tại theo dõi thực hiện cho chỉ tiêu và kỳ báo cáo này.");

            if (giaTriDauKy.HasValue && giaTriDauKy.Value < 0)
                throw new Exception("GiaTriDauKy không được nhỏ hơn 0.");

            if (giaTriThucHienTrongKy.HasValue && giaTriThucHienTrongKy.Value < 0)
                throw new Exception("GiaTriThucHienTrongKy không được nhỏ hơn 0.");
        }

        private async Task<decimal> TinhGiaTriLuyKeAsync(
            long chiTietGiaoChiTieuId,
            long kyBaoCaoKPIId,
            decimal giaTriCuoiKyHienTai,
            long? currentId)
        {
            var kyHienTai = await _context.KyBaoCaoKPIs
                .AsNoTracking()
                .FirstAsync(x => x.Id == kyBaoCaoKPIId);

            var cacBanGhiTruoc = await _context.TheoDoiThucHienKPIs
                .Include(x => x.KyBaoCaoKPI)
                .Where(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    (!currentId.HasValue || x.Id != currentId.Value))
                .ToListAsync();

            decimal luyKeTruoc = cacBanGhiTruoc
                .Where(x => x.KyBaoCaoKPI != null && LaKyTruoc(x.KyBaoCaoKPI, kyHienTai))
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenByDescending(x => ThuTuLoaiKy(x.KyBaoCaoKPI!.LoaiKy))
                .ThenByDescending(x => x.KyBaoCaoKPI!.SoKy ?? 0)
                .Select(x => x.GiaTriLuyKe ?? x.GiaTriCuoiKy ?? 0)
                .FirstOrDefault();

            return luyKeTruoc + giaTriCuoiKyHienTai;
        }

        private static bool LaKyTruoc(KyBaoCaoKPI kyCanSoSanh, KyBaoCaoKPI kyHienTai)
        {
            if (kyCanSoSanh.Nam < kyHienTai.Nam) return true;
            if (kyCanSoSanh.Nam > kyHienTai.Nam) return false;

            var thuTuKyCanSoSanh = ThuTuLoaiKy(kyCanSoSanh.LoaiKy);
            var thuTuKyHienTai = ThuTuLoaiKy(kyHienTai.LoaiKy);

            if (thuTuKyCanSoSanh < thuTuKyHienTai) return true;
            if (thuTuKyCanSoSanh > thuTuKyHienTai) return false;

            return (kyCanSoSanh.SoKy ?? 0) < (kyHienTai.SoKy ?? 0);
        }

        private static int ThuTuLoaiKy(string? loaiKy)
        {
            return (loaiKy ?? string.Empty).Trim().ToUpper() switch
            {
                "THANG" => 1,
                "QUY" => 2,
                "6THANG" => 3,
                "NAM" => 4,
                _ => 99
            };
        }

        private static TheoDoiThucHienKPIDto MapToDto(TheoDoiThucHienKPI x)
        {
            return new TheoDoiThucHienKPIDto
            {
                Id = x.Id,
                ChiTietGiaoChiTieuId = x.ChiTietGiaoChiTieuId,
                MaChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.MaChiTieu,
                TenChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.TenChiTieu,
                TenDonViNhan = x.ChiTietGiaoChiTieu?.DonViNhan?.TenDonVi,

                KyBaoCaoKPIId = x.KyBaoCaoKPIId,
                MaKy = x.KyBaoCaoKPI?.MaKy,
                TenKy = x.KyBaoCaoKPI?.TenKy,
                LoaiKy = x.KyBaoCaoKPI?.LoaiKy,
                Nam = x.KyBaoCaoKPI?.Nam ?? 0,
                SoKy = x.KyBaoCaoKPI?.SoKy,

                GiaTriDauKy = x.GiaTriDauKy,
                GiaTriThucHienTrongKy = x.GiaTriThucHienTrongKy,
                GiaTriCuoiKy = x.GiaTriCuoiKy,
                GiaTriLuyKe = x.GiaTriLuyKe,

                NhanXet = x.NhanXet,
                TrangThai = x.TrangThai,

                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy
            };
        }
    }
}