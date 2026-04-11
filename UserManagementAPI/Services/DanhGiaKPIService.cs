using UserManagementAPI.Data;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Models.DTOs.DanhGiaKPI;
using KPI_Tracker_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_Tracker_API.Services
{
    public class DanhGiaKPIService : IDanhGiaKPIService
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaKPIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DanhGiaKPIDto>> GetAllAsync()
        {
            var items = await _context.DanhGiaKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<DanhGiaKPIDto?> GetByIdAsync(long id)
        {
            var item = await _context.DanhGiaKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<DanhGiaKPIDto>> GetByChiTietGiaoChiTieuIdAsync(long chiTietGiaoChiTieuId)
        {
            var items = await _context.DanhGiaKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .OrderByDescending(x => x.KyBaoCaoKPIId)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<DanhGiaKPIDto>> GetByKyBaoCaoKPIIdAsync(long kyBaoCaoKPIId)
        {
            var items = await _context.DanhGiaKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.KyBaoCaoKPIId == kyBaoCaoKPIId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<DanhGiaKPIDto> UpsertDanhGiaKPIAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId, string? username)
        {
            var chiTiet = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .FirstOrDefaultAsync(x => x.Id == chiTietGiaoChiTieuId);

            if (chiTiet == null)
                throw new Exception("Không tìm thấy ChiTietGiaoChiTieu.");

            var kyBaoCao = await _context.KyBaoCaoKPIs
                .FirstOrDefaultAsync(x => x.Id == kyBaoCaoKPIId);

            if (kyBaoCao == null)
                throw new Exception("Không tìm thấy KyBaoCaoKPI.");

            var theoDoi = await _context.TheoDoiThucHienKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    x.KyBaoCaoKPIId == kyBaoCaoKPIId);

            if (theoDoi == null)
                throw new Exception("Không tìm thấy TheoDoiThucHienKPI để đánh giá KPI.");

            var giaTriMucTieu = chiTiet.GiaTriMucTieu;
            var giaTriDauKy = theoDoi.GiaTriDauKy;
            var giaTriCuoiKy = theoDoi.GiaTriCuoiKy;
            var giaTriCungKyNamTruoc = await LayGiaTriCungKyNamTruocAsync(chiTietGiaoChiTieuId, kyBaoCao);

            var chenhLechSoVoiDauKy = TinhChenhLech(giaTriCuoiKy, giaTriDauKy);
            var tyLeTangTruongSoVoiDauKy = TinhTyLeTangTruong(giaTriCuoiKy, giaTriDauKy);

            var chenhLechSoVoiCungKyNamTruoc = TinhChenhLech(giaTriCuoiKy, giaTriCungKyNamTruoc);
            var tyLeTangTruongSoVoiCungKyNamTruoc = TinhTyLeTangTruong(giaTriCuoiKy, giaTriCungKyNamTruoc);

            var tyLeHoanThanh = TinhTyLeHoanThanh(giaTriCuoiKy, giaTriMucTieu);

            var (xepLoai, ketQua) = await XacDinhXepLoaiVaKetQuaAsync(
                chiTiet.DanhMucChiTieuId,
                tyLeHoanThanh
            );

            var danhGia = await _context.DanhGiaKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    x.KyBaoCaoKPIId == kyBaoCaoKPIId);

            if (danhGia == null)
            {
                danhGia = new DanhGiaKPI
                {
                    ChiTietGiaoChiTieuId = chiTietGiaoChiTieuId,
                    KyBaoCaoKPIId = kyBaoCaoKPIId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DanhGiaKPIs.Add(danhGia);
            }

            danhGia.GiaTriMucTieu = giaTriMucTieu;
            danhGia.GiaTriDauKy = giaTriDauKy;
            danhGia.GiaTriCuoiKy = giaTriCuoiKy;
            danhGia.GiaTriCungKyNamTruoc = giaTriCungKyNamTruoc;

            danhGia.ChenhLechSoVoiDauKy = chenhLechSoVoiDauKy;
            danhGia.TyLeTangTruongSoVoiDauKy = tyLeTangTruongSoVoiDauKy;

            danhGia.ChenhLechSoVoiCungKyNamTruoc = chenhLechSoVoiCungKyNamTruoc;
            danhGia.TyLeTangTruongSoVoiCungKyNamTruoc = tyLeTangTruongSoVoiCungKyNamTruoc;

            danhGia.TyLeHoanThanh = tyLeHoanThanh;
            danhGia.XepLoai = xepLoai;
            danhGia.KetQua = ketQua;

            danhGia.NhanXetDanhGia = theoDoi.NhanXet;
            danhGia.NguoiDanhGia = string.IsNullOrWhiteSpace(username) ? "system" : username;
            danhGia.NgayDanhGia = DateTime.UtcNow;
            danhGia.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = await _context.DanhGiaKPIs
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieu)
                    .ThenInclude(x => x!.DonViNhan)
                .Include(x => x.KyBaoCaoKPI)
                .FirstAsync(x => x.Id == danhGia.Id);

            return MapToDto(result);
        }

        public async Task<bool> DeleteDanhGiaKPIAsync(long chiTietGiaoChiTieuId, long kyBaoCaoKPIId)
        {
            var danhGia = await _context.DanhGiaKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    x.KyBaoCaoKPIId == kyBaoCaoKPIId);

            if (danhGia == null)
                return false;

            _context.DanhGiaKPIs.Remove(danhGia);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<decimal?> LayGiaTriCungKyNamTruocAsync(long chiTietGiaoChiTieuId, KyBaoCaoKPI kyHienTai)
        {
            var kyCungKyNamTruoc = await _context.KyBaoCaoKPIs
                .FirstOrDefaultAsync(x =>
                    x.LoaiKy == kyHienTai.LoaiKy &&
                    x.Nam == kyHienTai.Nam - 1 &&
                    x.SoKy == kyHienTai.SoKy);

            if (kyCungKyNamTruoc == null)
                return null;

            var theoDoiNamTruoc = await _context.TheoDoiThucHienKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                    x.KyBaoCaoKPIId == kyCungKyNamTruoc.Id);

            return theoDoiNamTruoc?.GiaTriCuoiKy;
        }

        private static decimal? TinhChenhLech(decimal? giaTriSau, decimal? giaTriTruoc)
        {
            if (!giaTriSau.HasValue || !giaTriTruoc.HasValue)
                return null;

            return giaTriSau.Value - giaTriTruoc.Value;
        }

        private static decimal? TinhTyLeTangTruong(decimal? giaTriSau, decimal? giaTriTruoc)
        {
            if (!giaTriSau.HasValue || !giaTriTruoc.HasValue || giaTriTruoc.Value == 0)
                return null;

            return ((giaTriSau.Value - giaTriTruoc.Value) / giaTriTruoc.Value) * 100;
        }

        private static decimal? TinhTyLeHoanThanh(decimal? giaTriCuoiKy, decimal? giaTriMucTieu)
        {
            if (!giaTriCuoiKy.HasValue || !giaTriMucTieu.HasValue || giaTriMucTieu.Value == 0)
                return null;

            return (giaTriCuoiKy.Value / giaTriMucTieu.Value) * 100;
        }

        private async Task<(string? xepLoai, string? ketQua)> XacDinhXepLoaiVaKetQuaAsync(
            long? danhMucChiTieuId,
            decimal? tyLeHoanThanh)
        {
            if (!tyLeHoanThanh.HasValue)
                return ("Chưa đánh giá", "Chưa đủ dữ liệu");

            var cauHinh = await _context.CauHinhNguongDanhGiaKPIs
                .Where(x =>
                    (x.DanhMucChiTieuId == danhMucChiTieuId || x.DanhMucChiTieuId == null) &&
                    x.TuTyLe <= tyLeHoanThanh.Value &&
                    x.DenTyLe >= tyLeHoanThanh.Value)
                .OrderByDescending(x => x.DanhMucChiTieuId.HasValue)
                .ThenBy(x => x.TuTyLe)
                .FirstOrDefaultAsync();

            if (cauHinh == null)
                return ("Chưa cấu hình", "Chưa xác định");

            var ketQua = string.Equals(cauHinh.XepLoai, "Không đạt", StringComparison.OrdinalIgnoreCase)
                ? "Chưa hoàn thành"
                : "Hoàn thành";

            return (cauHinh.XepLoai, ketQua);
        }

        private static DanhGiaKPIDto MapToDto(DanhGiaKPI x)
        {
            return new DanhGiaKPIDto
            {
                Id = x.Id,
                ChiTietGiaoChiTieuId = x.ChiTietGiaoChiTieuId,
                KyBaoCaoKPIId = x.KyBaoCaoKPIId,

                MaChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.MaChiTieu,
                TenChiTieu = x.ChiTietGiaoChiTieu?.DanhMucChiTieu?.TenChiTieu,
                TenDonViNhan = x.ChiTietGiaoChiTieu?.DonViNhan?.TenDonVi,

                MaKy = x.KyBaoCaoKPI?.MaKy,
                TenKy = x.KyBaoCaoKPI?.TenKy,
                LoaiKy = x.KyBaoCaoKPI?.LoaiKy,
                Nam = x.KyBaoCaoKPI?.Nam ?? 0,
                SoKy = x.KyBaoCaoKPI?.SoKy,

                GiaTriMucTieu = x.GiaTriMucTieu,
                GiaTriDauKy = x.GiaTriDauKy,
                GiaTriCuoiKy = x.GiaTriCuoiKy,
                GiaTriCungKyNamTruoc = x.GiaTriCungKyNamTruoc,

                ChenhLechSoVoiDauKy = x.ChenhLechSoVoiDauKy,
                TyLeTangTruongSoVoiDauKy = x.TyLeTangTruongSoVoiDauKy,
                ChenhLechSoVoiCungKyNamTruoc = x.ChenhLechSoVoiCungKyNamTruoc,
                TyLeTangTruongSoVoiCungKyNamTruoc = x.TyLeTangTruongSoVoiCungKyNamTruoc,
                TyLeHoanThanh = x.TyLeHoanThanh,

                XepLoai = x.XepLoai,
                KetQua = x.KetQua,
                NhanXetDanhGia = x.NhanXetDanhGia,
                NguoiDanhGia = x.NguoiDanhGia,
                NgayDanhGia = x.NgayDanhGia,

                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }
    }
}