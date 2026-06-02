using KPITrackerAPI.Constants;
using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.TheoDoiThucHienKPI;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class TheoDoiThucHienKPIService : ITheoDoiThucHienKPIService
    {
        private const string TrangThaiChoXetDuyet = "CHO_XET_DUYET";
        private const string TrangThaiChoGui = "CHO_GUI";
        private const string TrangThaiTraLaiNhapLai = "TRA_LAI_NHAP_LAI";
        private const string TrangThaiDaGhiNhan = "MOI_TAO";

        private readonly ApplicationDbContext _context;
        private readonly IDanhGiaKPIService _danhGiaKPIService;
        private readonly IUserAccessScopeService _accessScope;

        public TheoDoiThucHienKPIService(
            ApplicationDbContext context,
            IDanhGiaKPIService danhGiaKPIService,
            IUserAccessScopeService accessScope)
        {
            _context = context;
            _danhGiaKPIService = danhGiaKPIService;
            _accessScope = accessScope;
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetAllAsync()
        {
            var query = await ApplyOwnerScopeAsync(_context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI));

            var items = await query
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => x.KyBaoCaoKPI!.LoaiKy)
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<TheoDoiThucHienKPIDto?> GetByIdAsync(long id)
        {
            var query = await ApplyOwnerScopeAsync(_context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI));

            var item = await query
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByChiTietGiaoChiTieuIdAsync(long chiTietGiaoChiTieuId)
        {
            var query = await ApplyOwnerScopeAsync(_context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI));

            var items = await query
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => x.KyBaoCaoKPI!.LoaiKy)
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<TheoDoiThucHienKPIDto>> GetByKyBaoCaoKPIIdAsync(long kyBaoCaoKPIId)
        {
            var query = await ApplyOwnerScopeAsync(_context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI));

            var items = await query
                .Where(x => x.KyBaoCaoKPIId == kyBaoCaoKPIId)
                .OrderBy(x => x.ChiTietGiaoChiTieuId)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<TheoDoiThucHienKPIDto?> GetByChiTietVaKyAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId)
        {
            var query = await ApplyOwnerScopeAsync(_context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI));

            var item = await query
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
                dto.GiaTriPhatSinhTrongKy,
                dto.GiaTriThucHienTrongKy,
                dto.NhanXet,
                null);

            var entity = new TheoDoiThucHienKPI
            {
                ChiTietGiaoChiTieuId = dto.ChiTietGiaoChiTieuId,
                KyBaoCaoKPIId = dto.KyBaoCaoKPIId,
                GiaTriDauKy = await GetGiaTriDauKyCoDinhAsync(dto.ChiTietGiaoChiTieuId),
                GiaTriPhatSinhTrongKy = dto.GiaTriPhatSinhTrongKy,
                GiaTriThucHienTrongKy = dto.GiaTriThucHienTrongKy ?? 0,
                GiaTriCuoiKy = 0,
                GiaTriLuyKe = 0,
                GiaTriPhatSinhLuyKe = 0,
                NhanXet = dto.NhanXet?.Trim(),
                TrangThai = TrangThaiChoGui,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system"
            };

            _context.TheoDoiThucHienKPIs.Add(entity);
            await _context.SaveChangesAsync();

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
            if (entity == null)
            {
                return null;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return null;
            }

            await ValidateBeforeSave(
                dto.ChiTietGiaoChiTieuId,
                dto.KyBaoCaoKPIId,
                dto.GiaTriDauKy,
                dto.GiaTriPhatSinhTrongKy,
                dto.GiaTriThucHienTrongKy,
                dto.NhanXet,
                id);

            var oldChiTietId = entity.ChiTietGiaoChiTieuId;

            entity.ChiTietGiaoChiTieuId = dto.ChiTietGiaoChiTieuId;
            entity.KyBaoCaoKPIId = dto.KyBaoCaoKPIId;
            entity.GiaTriDauKy = await GetGiaTriDauKyCoDinhAsync(dto.ChiTietGiaoChiTieuId);
            entity.GiaTriPhatSinhTrongKy = dto.GiaTriPhatSinhTrongKy;
            entity.GiaTriThucHienTrongKy = dto.GiaTriThucHienTrongKy ?? 0;
            entity.NhanXet = dto.NhanXet?.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();

            var affectedChiTietIds = new HashSet<long> { oldChiTietId, entity.ChiTietGiaoChiTieuId };
            foreach (var chiTietId in affectedChiTietIds)
            {
                await RecalculateTheoDoiChainAsync(chiTietId);
                await _danhGiaKPIService.SynchronizeDanhGiaForChiTietAsync(chiTietId, entity.UpdatedBy);
            }

            var updated = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(updated);
        }

        public async Task<TheoDoiThucHienKPIDto?> SubmitAsync(long id)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return null;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return null;
            }

            if (!string.Equals(entity.TrangThai, TrangThaiChoGui, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chi co the gui bao cao dang o trang thai cho gui.");
            }

            entity.TrangThai = TrangThaiChoXetDuyet;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();

            var submitted = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(submitted);
        }

        public async Task<TheoDoiThucHienKPIDto?> ApproveAsync(long id)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return null;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return null;
            }

            if (!string.Equals(entity.TrangThai, TrangThaiChoXetDuyet, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Bao cao nay khong o trang thai cho xet duyet.");
            }

            entity.TrangThai = TrangThaiDaGhiNhan;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();
            await RecalculateTheoDoiChainAsync(entity.ChiTietGiaoChiTieuId);
            await _danhGiaKPIService.SynchronizeDanhGiaForChiTietAsync(
                entity.ChiTietGiaoChiTieuId,
                entity.UpdatedBy);

            var approved = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(approved);
        }

        public async Task<bool> ReturnForReEntryAsync(long id, string? lyDo)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return false;
            }

            if (!string.Equals(entity.TrangThai, TrangThaiChoXetDuyet, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chi co the gui tra bao cao dang cho xet duyet.");
            }

            var normalizedReason = lyDo?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedReason))
            {
                throw new Exception("Vui long nhap ly do gui tra bao cao.");
            }

            entity.TrangThai = TrangThaiTraLaiNhapLai;
            entity.NhanXet = normalizedReason;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<TheoDoiThucHienKPIDto?> ResubmitReturnedAsync(long id)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return null;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return null;
            }

            if (!string.Equals(entity.TrangThai, TrangThaiTraLaiNhapLai, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chi co the gui lai bao cao dang bi tra ve.");
            }

            entity.TrangThai = TrangThaiChoXetDuyet;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _context.SaveChangesAsync();

            var resubmitted = await _context.TheoDoiThucHienKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(resubmitted);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.TheoDoiThucHienKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            if (!await CanAccessTheoDoiAsync(entity.Id))
            {
                return false;
            }

            var chiTietId = entity.ChiTietGiaoChiTieuId;
            _context.TheoDoiThucHienKPIs.Remove(entity);
            await _context.SaveChangesAsync();

            await RecalculateTheoDoiChainAsync(chiTietId);
            await _danhGiaKPIService.SynchronizeDanhGiaForChiTietAsync(chiTietId, "system");

            return true;
        }

        private async Task ValidateBeforeSave(
            long chiTietGiaoChiTieuId,
            long kyBaoCaoKPIId,
            decimal? giaTriDauKy,
            decimal? giaTriPhatSinhTrongKy,
            decimal? giaTriThucHienTrongKy,
            string? nhanXet,
            long? currentId)
        {
            var chiTiet = await _context.ChiTietGiaoChiTieus
                .Include(x => x.ChiTietGiaoChiTieuCons)
                .Include(x => x.DanhMucChiTieu)
                .FirstOrDefaultAsync(x => x.Id == chiTietGiaoChiTieuId);

            if (chiTiet == null)
            {
                throw new Exception("ChiTietGiaoChiTieuId khong ton tai.");
            }

            if (!await CanAccessChiTietAsync(chiTiet))
            {
                throw new Exception("Tai khoan chi duoc quan ly bao cao cua danh muc chi tieu do don vi cua minh chu tri.");
            }

            if (chiTiet.ChiTietGiaoChiTieuCons.Any())
            {
                throw new Exception("Chi tieu phan ra phai nhap ket qua o tung tieu chi con, khong nhap o chi tieu cha.");
            }

            var kyBaoCaoExists = await _context.KyBaoCaoKPIs
                .AnyAsync(x => x.Id == kyBaoCaoKPIId);

            if (!kyBaoCaoExists)
            {
                throw new Exception("KyBaoCaoKPIId khong ton tai.");
            }

            var duplicated = await _context.TheoDoiThucHienKPIs.AnyAsync(x =>
                x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                x.KyBaoCaoKPIId == kyBaoCaoKPIId &&
                (!currentId.HasValue || x.Id != currentId.Value));

            if (duplicated)
            {
                throw new Exception("Da ton tai theo doi thuc hien cho chi tieu va ky bao cao nay.");
            }

            if (giaTriThucHienTrongKy.HasValue && giaTriThucHienTrongKy.Value < 0)
            {
                throw new Exception("GiaTriThucHienTrongKy khong duoc nho hon 0.");
            }

            var tieuChiDanhGia = DanhGiaKPIConstants.NormalizeCode(
                chiTiet.TieuChiDanhGia ?? chiTiet.DanhMucChiTieu?.LoaiChiTieu);
            var kieuSoSanh = DanhGiaKPIConstants.NormalizeCode(chiTiet.KieuSoSanh)
                ?? DanhGiaKPIConstants.KieuSoSanh.ChenhLech;
            if (tieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh &&
                string.IsNullOrWhiteSpace(nhanXet))
            {
                throw new Exception("Chi tieu dinh tinh bat buoc chon ket qua danh gia.");
            }

            if (tieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh)
            {
                var normalizedOption = DanhGiaKPIConstants.NormalizeCode(nhanXet);
                if (!DanhGiaKPIConstants.AllowedDinhTinhOptions.Contains(normalizedOption))
                {
                    throw new Exception("Ket qua danh gia dinh tinh khong hop le.");
                }
            }

            if (tieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhLuongSoSanh &&
                kieuSoSanh == DanhGiaKPIConstants.KieuSoSanh.TyLe)
            {
                if (!giaTriPhatSinhTrongKy.HasValue)
                {
                    throw new Exception("Chi tieu so sanh theo ty le bat buoc nhap GiaTriPhatSinhTrongKy.");
                }

                if (giaTriPhatSinhTrongKy.Value < 0)
                {
                    throw new Exception("GiaTriPhatSinhTrongKy khong duoc nho hon 0.");
                }
            }
        }

        private async Task RecalculateTheoDoiChainAsync(long chiTietGiaoChiTieuId)
        {
            var giaTriDauKyCoDinh = await GetGiaTriDauKyCoDinhAsync(chiTietGiaoChiTieuId);
            var records = await _context.TheoDoiThucHienKPIs
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .ToListAsync();

            var ghiNhanRecords = records
                .Where(x =>
                    !string.Equals(x.TrangThai, TrangThaiChoGui, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(x.TrangThai, TrangThaiChoXetDuyet, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(x.TrangThai, TrangThaiTraLaiNhapLai, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (ghiNhanRecords.Count == 0)
            {
                return;
            }

            var ordered = ghiNhanRecords
                .OrderBy(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => ThuTuLoaiKy(x.KyBaoCaoKPI!.LoaiKy))
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy ?? 0)
                .ThenBy(x => x.Id)
                .ToList();

            decimal luyKe = 0;
            decimal phatSinhLuyKe = 0;

            foreach (var record in ordered)
            {
                var giaTriThucHienTrongKy = record.GiaTriThucHienTrongKy ?? 0;
                var giaTriPhatSinhTrongKy = record.GiaTriPhatSinhTrongKy ?? 0;
                luyKe += giaTriThucHienTrongKy;
                phatSinhLuyKe += giaTriPhatSinhTrongKy;

                record.GiaTriDauKy = giaTriDauKyCoDinh;
                record.GiaTriCuoiKy = CalculateGiaTriCuoiKy(giaTriDauKyCoDinh, luyKe);
                record.GiaTriLuyKe = luyKe;
                record.GiaTriPhatSinhLuyKe = phatSinhLuyKe;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<decimal> GetGiaTriDauKyCoDinhAsync(long chiTietGiaoChiTieuId)
        {
            var giaTri = await _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .Where(x => x.Id == chiTietGiaoChiTieuId)
                .Select(x => x.GiaTriDauKyCoDinh)
                .FirstOrDefaultAsync();

            return giaTri ?? 0;
        }

        private static decimal CalculateGiaTriCuoiKy(
            decimal giaTriDauKyCoDinh,
            decimal giaTriLuyKe)
        {
            return giaTriDauKyCoDinh + giaTriLuyKe;
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

        private async Task<IQueryable<TheoDoiThucHienKPI>> ApplyOwnerScopeAsync(IQueryable<TheoDoiThucHienKPI> query)
        {
            var ownerScopeDonViId = await _accessScope.GetCurrentOwnerScopeDonViIdAsync();
            if (!ownerScopeDonViId.HasValue)
            {
                return query;
            }

            return query.Where(x =>
                x.ChiTietGiaoChiTieu != null &&
                x.ChiTietGiaoChiTieu.DanhMucChiTieu != null &&
                x.ChiTietGiaoChiTieu.DanhMucChiTieu.DonViChuTriId == ownerScopeDonViId.Value);
        }

        private async Task<bool> CanAccessTheoDoiAsync(long id)
        {
            var ownerScopeDonViId = await _accessScope.GetCurrentOwnerScopeDonViIdAsync();
            if (!ownerScopeDonViId.HasValue)
            {
                return true;
            }

            return await _context.TheoDoiThucHienKPIs
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == id &&
                    x.ChiTietGiaoChiTieu != null &&
                    x.ChiTietGiaoChiTieu.DanhMucChiTieu != null &&
                    x.ChiTietGiaoChiTieu.DanhMucChiTieu.DonViChuTriId == ownerScopeDonViId.Value);
        }

        private async Task<bool> CanAccessChiTietAsync(ChiTietGiaoChiTieu chiTiet)
        {
            var ownerScopeDonViId = await _accessScope.GetCurrentOwnerScopeDonViIdAsync();
            return !ownerScopeDonViId.HasValue ||
                   chiTiet.DanhMucChiTieu?.DonViChuTriId == ownerScopeDonViId.Value;
        }

        private static TheoDoiThucHienKPIDto MapToDto(TheoDoiThucHienKPI x)
        {
            return new TheoDoiThucHienKPIDto
            {
                Id = x.Id,
                ChiTietGiaoChiTieuId = x.ChiTietGiaoChiTieuId,
                MaChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.MaChiTieu,
                TenChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.TenChiTieu,
                TieuChiDanhGia = x.ChiTietGiaoChiTieu?.TieuChiDanhGia ?? x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.LoaiChiTieu,
                DonViNhanId = x.ChiTietGiaoChiTieu?.DonViNhanId,
                TenDonViNhan = x.ChiTietGiaoChiTieu?.DonViNhan?.TenDonVi,

                KyBaoCaoKPIId = x.KyBaoCaoKPIId,
                MaKy = x.KyBaoCaoKPI?.MaKy,
                TenKy = x.KyBaoCaoKPI?.TenKy,
                LoaiKy = x.KyBaoCaoKPI?.LoaiKy,
                Nam = x.KyBaoCaoKPI?.Nam ?? 0,
                SoKy = x.KyBaoCaoKPI?.SoKy,

                GiaTriDauKy = x.GiaTriDauKy,
                GiaTriPhatSinhTrongKy = x.GiaTriPhatSinhTrongKy,
                GiaTriThucHienTrongKy = x.GiaTriThucHienTrongKy,
                GiaTriCuoiKy = x.GiaTriCuoiKy,
                GiaTriLuyKe = x.GiaTriLuyKe,
                GiaTriPhatSinhLuyKe = x.GiaTriPhatSinhLuyKe,

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
