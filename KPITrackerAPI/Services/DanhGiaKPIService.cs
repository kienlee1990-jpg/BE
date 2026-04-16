using KPITrackerAPI.Constants;
using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.DanhGiaKPI;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace KPITrackerAPI.Services
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
            var chiTiet = await LoadChiTietAsync(chiTietGiaoChiTieuId);
            if (chiTiet == null)
            {
                throw new Exception("Khong tim thay ChiTietGiaoChiTieu.");
            }

            var danhGia = await UpsertDanhGiaCoreAsync(chiTiet, kyBaoCaoKPIId, username, true);
            if (danhGia == null)
            {
                throw new Exception("Khong du du lieu de danh gia KPI.");
            }

            await _context.SaveChangesAsync();

            if (chiTiet.ChiTietGiaoChaId.HasValue)
            {
                await SynchronizeDanhGiaForChiTietAsync(chiTiet.ChiTietGiaoChaId.Value, username);
            }

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
            {
                return false;
            }

            var chiTiet = await _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == chiTietGiaoChiTieuId);

            _context.DanhGiaKPIs.Remove(danhGia);
            await _context.SaveChangesAsync();

            if (chiTiet?.ChiTietGiaoChaId.HasValue == true)
            {
                await SynchronizeDanhGiaForChiTietAsync(chiTiet.ChiTietGiaoChaId.Value, "system");
            }

            return true;
        }

        public async Task SynchronizeDanhGiaForChiTietAsync(long chiTietGiaoChiTieuId, string? username)
        {
            var visited = new HashSet<long>();
            await SynchronizeDanhGiaHierarchyAsync(chiTietGiaoChiTieuId, username, visited);
        }

        private async Task SynchronizeDanhGiaHierarchyAsync(long chiTietGiaoChiTieuId, string? username, HashSet<long> visited)
        {
            if (!visited.Add(chiTietGiaoChiTieuId))
            {
                return;
            }

            var chiTiet = await LoadChiTietAsync(chiTietGiaoChiTieuId);
            if (chiTiet == null)
            {
                return;
            }

            var kyIds = await GetRelevantKyIdsAsync(chiTiet);
            var keepKyIds = kyIds.ToHashSet();

            var existingRows = await _context.DanhGiaKPIs
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .ToListAsync();

            foreach (var row in existingRows.Where(x => !keepKyIds.Contains(x.KyBaoCaoKPIId)))
            {
                _context.DanhGiaKPIs.Remove(row);
            }

            foreach (var kyId in kyIds)
            {
                await UpsertDanhGiaCoreAsync(chiTiet, kyId, username, false);
            }

            await _context.SaveChangesAsync();

            if (chiTiet.ChiTietGiaoChaId.HasValue)
            {
                await SynchronizeDanhGiaHierarchyAsync(chiTiet.ChiTietGiaoChaId.Value, username, visited);
            }
        }

        private async Task<ChiTietGiaoChiTieu?> LoadChiTietAsync(long chiTietGiaoChiTieuId)
        {
            return await _context.ChiTietGiaoChiTieus
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.ChiTietGiaoChiTieuCons)
                    .ThenInclude(x => x.DanhMucChiTieu)
                .FirstOrDefaultAsync(x => x.Id == chiTietGiaoChiTieuId);
        }

        private async Task<List<long>> GetRelevantKyIdsAsync(ChiTietGiaoChiTieu chiTiet)
        {
            if (chiTiet.ChiTietGiaoChiTieuCons.Any())
            {
                var childIds = chiTiet.ChiTietGiaoChiTieuCons.Select(x => x.Id).ToList();

                return await _context.DanhGiaKPIs
                    .Where(x => childIds.Contains(x.ChiTietGiaoChiTieuId))
                    .Select(x => x.KyBaoCaoKPIId)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();
            }

            return await _context.TheoDoiThucHienKPIs
                .Where(x => x.ChiTietGiaoChiTieuId == chiTiet.Id)
                .Select(x => x.KyBaoCaoKPIId)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        private async Task<DanhGiaKPI?> UpsertDanhGiaCoreAsync(
            ChiTietGiaoChiTieu chiTiet,
            long kyBaoCaoKPIId,
            string? username,
            bool throwWhenNoData)
        {
            var kyBaoCao = await _context.KyBaoCaoKPIs
                .FirstOrDefaultAsync(x => x.Id == kyBaoCaoKPIId);

            if (kyBaoCao == null)
            {
                throw new Exception("Khong tim thay KyBaoCaoKPI.");
            }

            if (chiTiet.ChiTietGiaoChiTieuCons.Any())
            {
                return await UpsertDanhGiaTongHopAsync(chiTiet, kyBaoCao, username, throwWhenNoData);
            }

            return await UpsertDanhGiaLaAsync(chiTiet, kyBaoCao, username, throwWhenNoData);
        }

        private async Task<DanhGiaKPI?> UpsertDanhGiaLaAsync(
            ChiTietGiaoChiTieu chiTiet,
            KyBaoCaoKPI kyBaoCao,
            string? username,
            bool throwWhenNoData)
        {
            var theoDoi = await _context.TheoDoiThucHienKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTiet.Id &&
                    x.KyBaoCaoKPIId == kyBaoCao.Id);

            if (theoDoi == null)
            {
                if (throwWhenNoData)
                {
                    throw new Exception("Khong tim thay TheoDoiThucHienKPI de danh gia.");
                }

                return null;
            }

            var comparisonContext = await BuildComparisonContextAsync(chiTiet.Id, kyBaoCao);
            var tyLeHoanThanh = TinhTyLeHoanThanhTheoLoaiChiTieu(chiTiet, theoDoi, comparisonContext);
            var (xepLoai, ketQua) = await XacDinhXepLoaiVaKetQuaAsync(
                chiTiet,
                kyBaoCao,
                tyLeHoanThanh);
            ketQua = GetDanhGiaLabel(xepLoai);

            var danhGia = await _context.DanhGiaKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTiet.Id &&
                    x.KyBaoCaoKPIId == kyBaoCao.Id);

            if (danhGia == null)
            {
                danhGia = new DanhGiaKPI
                {
                    ChiTietGiaoChiTieuId = chiTiet.Id,
                    KyBaoCaoKPIId = kyBaoCao.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DanhGiaKPIs.Add(danhGia);
            }

            danhGia.GiaTriMucTieu = chiTiet.GiaTriMucTieu;
            danhGia.GiaTriDauKy = theoDoi.GiaTriDauKy;
            danhGia.GiaTriCuoiKy = theoDoi.GiaTriCuoiKy;
            danhGia.GiaTriCungKyNamTruoc = comparisonContext.CungKyNamTruocGiaTriCuoiKy;
            danhGia.ChenhLechSoVoiDauKy = TinhChenhLech(theoDoi.GiaTriCuoiKy, theoDoi.GiaTriDauKy);
            danhGia.TyLeTangTruongSoVoiDauKy = TinhTyLeTangTruong(theoDoi.GiaTriCuoiKy, theoDoi.GiaTriDauKy);
            danhGia.ChenhLechSoVoiCungKyNamTruoc = TinhChenhLech(theoDoi.GiaTriCuoiKy, comparisonContext.CungKyNamTruocGiaTriCuoiKy);
            danhGia.TyLeTangTruongSoVoiCungKyNamTruoc = TinhTyLeTangTruong(theoDoi.GiaTriCuoiKy, comparisonContext.CungKyNamTruocGiaTriCuoiKy);
            danhGia.TyLeHoanThanh = tyLeHoanThanh;
            danhGia.XepLoai = xepLoai;
            danhGia.KetQua = ketQua;
            danhGia.NhanXetDanhGia = theoDoi.NhanXet;
            danhGia.NguoiDanhGia = string.IsNullOrWhiteSpace(username) ? "system" : username;
            danhGia.NgayDanhGia = DateTime.UtcNow;
            danhGia.UpdatedAt = DateTime.UtcNow;

            return danhGia;
        }

        private async Task<DanhGiaKPI?> UpsertDanhGiaTongHopAsync(
            ChiTietGiaoChiTieu chiTiet,
            KyBaoCaoKPI kyBaoCao,
            string? username,
            bool throwWhenNoData)
        {
            var childIds = chiTiet.ChiTietGiaoChiTieuCons.Select(x => x.Id).ToList();
            var childEvaluations = await _context.DanhGiaKPIs
                .Where(x =>
                    childIds.Contains(x.ChiTietGiaoChiTieuId) &&
                    x.KyBaoCaoKPIId == kyBaoCao.Id)
                .OrderBy(x => x.ChiTietGiaoChiTieuId)
                .ToListAsync();

            if (childEvaluations.Count == 0)
            {
                if (throwWhenNoData)
                {
                    throw new Exception("Khong co du lieu danh gia cua tieu chi con de tong hop.");
                }

                return null;
            }

            var batBuocDatTatCa = chiTiet.DanhMucChiTieu?.BatBuocDatTatCaTieuChiCon ?? true;
            var tyLeHoanThanh = TinhTyLeTongHop(childEvaluations, batBuocDatTatCa);
            var (xepLoai, ketQua) = await XacDinhXepLoaiVaKetQuaAsync(
                chiTiet,
                kyBaoCao,
                tyLeHoanThanh);
            ketQua = GetDanhGiaLabel(xepLoai);

            var coConKhongDat = childEvaluations.Any(x => !LaTrangThaiHoanThanh(x.XepLoai));
            var coConDat = childEvaluations.Any(x => LaTrangThaiHoanThanh(x.XepLoai));

            if ((batBuocDatTatCa && coConKhongDat) || (!batBuocDatTatCa && !coConDat))
            {
                xepLoai = XacDinhTrangThaiThatBai(chiTiet, kyBaoCao);
                ketQua = GetDanhGiaLabel(xepLoai);
            }

            var danhGia = await _context.DanhGiaKPIs
                .FirstOrDefaultAsync(x =>
                    x.ChiTietGiaoChiTieuId == chiTiet.Id &&
                    x.KyBaoCaoKPIId == kyBaoCao.Id);

            if (danhGia == null)
            {
                danhGia = new DanhGiaKPI
                {
                    ChiTietGiaoChiTieuId = chiTiet.Id,
                    KyBaoCaoKPIId = kyBaoCao.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DanhGiaKPIs.Add(danhGia);
            }

            var giaTriMucTieu = childEvaluations.Sum(x => x.GiaTriMucTieu ?? 0m);
            var giaTriDauKy = childEvaluations.Sum(x => x.GiaTriDauKy ?? 0m);
            var giaTriCuoiKy = childEvaluations.Sum(x => x.GiaTriCuoiKy ?? 0m);
            var giaTriCungKyNamTruoc = childEvaluations.Sum(x => x.GiaTriCungKyNamTruoc ?? 0m);

            danhGia.GiaTriMucTieu = giaTriMucTieu == 0 ? null : giaTriMucTieu;
            danhGia.GiaTriDauKy = giaTriDauKy == 0 ? null : giaTriDauKy;
            danhGia.GiaTriCuoiKy = giaTriCuoiKy == 0 ? null : giaTriCuoiKy;
            danhGia.GiaTriCungKyNamTruoc = giaTriCungKyNamTruoc == 0 ? null : giaTriCungKyNamTruoc;
            danhGia.ChenhLechSoVoiDauKy = TinhChenhLech(danhGia.GiaTriCuoiKy, danhGia.GiaTriDauKy);
            danhGia.TyLeTangTruongSoVoiDauKy = TinhTyLeTangTruong(danhGia.GiaTriCuoiKy, danhGia.GiaTriDauKy);
            danhGia.ChenhLechSoVoiCungKyNamTruoc = TinhChenhLech(danhGia.GiaTriCuoiKy, danhGia.GiaTriCungKyNamTruoc);
            danhGia.TyLeTangTruongSoVoiCungKyNamTruoc = TinhTyLeTangTruong(danhGia.GiaTriCuoiKy, danhGia.GiaTriCungKyNamTruoc);
            danhGia.TyLeHoanThanh = tyLeHoanThanh;
            danhGia.XepLoai = xepLoai;
            danhGia.KetQua = ketQua;
            danhGia.NhanXetDanhGia = GopNhanXet(childEvaluations);
            danhGia.NguoiDanhGia = string.IsNullOrWhiteSpace(username) ? "system" : username;
            danhGia.NgayDanhGia = DateTime.UtcNow;
            danhGia.UpdatedAt = DateTime.UtcNow;

            return danhGia;
        }

        private async Task<ComparisonContext> BuildComparisonContextAsync(long chiTietGiaoChiTieuId, KyBaoCaoKPI kyBaoCao)
        {
            var cungKyNamTruoc = await _context.KyBaoCaoKPIs
                .FirstOrDefaultAsync(x =>
                    x.LoaiKy == kyBaoCao.LoaiKy &&
                    x.Nam == kyBaoCao.Nam - 1 &&
                    x.SoKy == kyBaoCao.SoKy);

            TheoDoiThucHienKPI? theoDoiCungKyNamTruoc = null;
            if (cungKyNamTruoc != null)
            {
                theoDoiCungKyNamTruoc = await _context.TheoDoiThucHienKPIs
                    .FirstOrDefaultAsync(x =>
                        x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId &&
                        x.KyBaoCaoKPIId == cungKyNamTruoc.Id);
            }

            var danhSachKyTruoc = await _context.TheoDoiThucHienKPIs
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .ToListAsync();

            var theoDoiKyTruoc = danhSachKyTruoc
                .Where(x => x.KyBaoCaoKPI != null && LaKyTruoc(x.KyBaoCaoKPI, kyBaoCao))
                .OrderByDescending(x => x.KyBaoCaoKPI!.Nam)
                .ThenByDescending(x => ThuTuLoaiKy(x.KyBaoCaoKPI!.LoaiKy))
                .ThenByDescending(x => x.KyBaoCaoKPI!.SoKy ?? 0)
                .FirstOrDefault();

            var theoDoiTongNamTruoc = danhSachKyTruoc
                .Where(x => x.KyBaoCaoKPI != null && x.KyBaoCaoKPI.Nam == kyBaoCao.Nam - 1)
                .OrderByDescending(x => x.KyBaoCaoKPI!.NgayCuoiKy)
                .FirstOrDefault();

            return new ComparisonContext
            {
                CungKyNamTruocGiaTriCuoiKy = theoDoiCungKyNamTruoc?.GiaTriCuoiKy,
                CungKyNamTruocGiaTriLuyKe = theoDoiCungKyNamTruoc?.GiaTriLuyKe,
                KyTruocGiaTriCuoiKy = theoDoiKyTruoc?.GiaTriCuoiKy,
                KyTruocGiaTriLuyKe = theoDoiKyTruoc?.GiaTriLuyKe,
                TongNamTruocGiaTriLuyKe = theoDoiTongNamTruoc?.GiaTriLuyKe
            };
        }

        private static decimal? TinhTyLeHoanThanhTheoLoaiChiTieu(
            ChiTietGiaoChiTieu chiTiet,
            TheoDoiThucHienKPI theoDoi,
            ComparisonContext comparisonContext)
        {
            var loaiChiTieu = NormalizeKey(chiTiet.DanhMucChiTieu?.LoaiChiTieu);

            return loaiChiTieu switch
            {
                "DINH_LUONG_TICH_LUY" => TinhTyLeHoanThanh(theoDoi.GiaTriLuyKe, chiTiet.GiaTriMucTieu),
                "DINH_LUONG_SO_SANH" => TinhTyLeHoanThanhSoSanh(chiTiet.DanhMucChiTieu, theoDoi, comparisonContext),
                "DINH_TINH" => TinhTyLeHoanThanhDinhTinh(chiTiet, theoDoi),
                _ => TinhTyLeHoanThanh(theoDoi.GiaTriCuoiKy, chiTiet.GiaTriMucTieu)
            };
        }

        private static decimal? TinhTyLeHoanThanhDinhTinh(ChiTietGiaoChiTieu chiTiet, TheoDoiThucHienKPI theoDoi)
        {
            if (chiTiet.GiaTriMucTieu.HasValue && chiTiet.GiaTriMucTieu.Value > 0)
            {
                return TinhTyLeHoanThanh(theoDoi.GiaTriLuyKe, chiTiet.GiaTriMucTieu);
            }

            if (string.IsNullOrWhiteSpace(theoDoi.NhanXet))
            {
                return null;
            }

            var normalizedOption = NormalizeKey(theoDoi.NhanXet);
            if (DinhTinhHoanThanhOptions.Contains(normalizedOption))
            {
                return 100m;
            }

            if (DinhTinhKhongHoanThanhOptions.Contains(normalizedOption))
            {
                return 0m;
            }

            return null;
        }

        private static decimal? TinhTyLeHoanThanhSoSanh(
            DanhMucChiTieu? danhMucChiTieu,
            TheoDoiThucHienKPI theoDoi,
            ComparisonContext comparisonContext)
        {
            if (danhMucChiTieu?.TyLePhanTramMucTieu == null || danhMucChiTieu.TyLePhanTramMucTieu <= 0)
            {
                return null;
            }

            var loaiMocSoSanh = NormalizeKey(danhMucChiTieu.LoaiMocSoSanh);
            var giaTriHienTai = loaiMocSoSanh switch
            {
                "CUNG_KY" or "KY_TRUOC" or "TONG_NAM_TRUOC" => theoDoi.GiaTriLuyKe,
                _ => theoDoi.GiaTriCuoiKy
            };
            var giaTriMoc = NormalizeKey(danhMucChiTieu.LoaiMocSoSanh) switch
            {
                "DAU_KY" => theoDoi.GiaTriDauKy,
                "CUNG_KY" => comparisonContext.CungKyNamTruocGiaTriLuyKe ?? comparisonContext.CungKyNamTruocGiaTriCuoiKy,
                "KY_TRUOC" => comparisonContext.KyTruocGiaTriLuyKe ?? comparisonContext.KyTruocGiaTriCuoiKy,
                "TONG_NAM_TRUOC" => comparisonContext.TongNamTruocGiaTriLuyKe,
                _ => null
            };

            var tyLeSoSanhThucTe = TinhTyLeSoSanhThucTe(
                giaTriHienTai,
                giaTriMoc,
                NormalizeKey(danhMucChiTieu.ChieuSoSanh));

            if (!tyLeSoSanhThucTe.HasValue)
            {
                return null;
            }

            return Math.Max((tyLeSoSanhThucTe.Value / danhMucChiTieu.TyLePhanTramMucTieu.Value) * 100m, 0m);
        }

        private static decimal? TinhTyLeSoSanhThucTe(decimal? giaTriHienTai, decimal? giaTriMoc, string chieuSoSanh)
        {
            if (!giaTriHienTai.HasValue || !giaTriMoc.HasValue || giaTriMoc.Value == 0)
            {
                return null;
            }

            return chieuSoSanh switch
            {
                "GIAM" => ((giaTriMoc.Value - giaTriHienTai.Value) / giaTriMoc.Value) * 100m,
                _ => ((giaTriHienTai.Value - giaTriMoc.Value) / giaTriMoc.Value) * 100m
            };
        }

        private static decimal? TinhTyLeTongHop(IEnumerable<DanhGiaKPI> childEvaluations, bool batBuocDatTatCa)
        {
            var tyLeList = childEvaluations
                .Where(x => x.TyLeHoanThanh.HasValue)
                .Select(x => x.TyLeHoanThanh!.Value)
                .ToList();

            if (tyLeList.Count == 0)
            {
                return null;
            }

            return batBuocDatTatCa ? tyLeList.Min() : tyLeList.Average();
        }

        private static bool LaTrangThaiHoanThanh(string? xepLoai)
        {
            return DanhGiaKPIConstants.LaTrangThaiHoanThanh(xepLoai);
        }

        private static string GetDanhGiaLabel(string? xepLoai)
        {
            return DanhGiaKPIConstants.NormalizeCode(xepLoai) switch
            {
                DanhGiaKPIConstants.XepLoai.KhongHoanThanh => "Không hoàn thành",
                DanhGiaKPIConstants.XepLoai.ChuaHoanThanh => "Chưa hoàn thành",
                DanhGiaKPIConstants.XepLoai.HoanThanh => "Hoàn thành",
                DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc => "Hoàn thành vượt mức",
                DanhGiaKPIConstants.XepLoai.ChuaDanhGia => "Chưa đánh giá",
                DanhGiaKPIConstants.XepLoai.ChuaCauHinh => "Chưa cấu hình",
                _ => string.IsNullOrWhiteSpace(xepLoai) ? "Chưa xác định" : xepLoai.Trim()
            };
        }

        private static string? GopNhanXet(IEnumerable<DanhGiaKPI> childEvaluations)
        {
            var parts = childEvaluations
                .Select(x => x.NhanXetDanhGia?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            return parts.Count == 0 ? null : string.Join(" | ", parts);
        }

        private static decimal? TinhChenhLech(decimal? giaTriSau, decimal? giaTriTruoc)
        {
            if (!giaTriSau.HasValue || !giaTriTruoc.HasValue)
            {
                return null;
            }

            return giaTriSau.Value - giaTriTruoc.Value;
        }

        private static decimal? TinhTyLeTangTruong(decimal? giaTriSau, decimal? giaTriTruoc)
        {
            if (!giaTriSau.HasValue || !giaTriTruoc.HasValue || giaTriTruoc.Value == 0)
            {
                return null;
            }

            return ((giaTriSau.Value - giaTriTruoc.Value) / giaTriTruoc.Value) * 100m;
        }

        private static decimal? TinhTyLeHoanThanh(decimal? giaTriThucHien, decimal? giaTriMucTieu)
        {
            if (!giaTriThucHien.HasValue || !giaTriMucTieu.HasValue || giaTriMucTieu.Value == 0)
            {
                return null;
            }

            return (giaTriThucHien.Value / giaTriMucTieu.Value) * 100m;
        }

        private async Task<(string? xepLoai, string? ketQua)> XacDinhXepLoaiVaKetQuaAsync(
            ChiTietGiaoChiTieu chiTiet,
            KyBaoCaoKPI kyBaoCao,
            decimal? tyLeHoanThanh)
        {
            if (!tyLeHoanThanh.HasValue)
            {
                return (DanhGiaKPIConstants.XepLoai.ChuaDanhGia, "Chưa đủ dữ liệu");
            }

            var dieuKienThoiHan = XacDinhDieuKienThoiHan(chiTiet, kyBaoCao, tyLeHoanThanh.Value);
            var cauHinh = await LoadNguongDanhGiaAsync(
                chiTiet.DanhMucChiTieuId,
                tyLeHoanThanh.Value,
                dieuKienThoiHan);

            if (cauHinh == null)
            {
                return (DanhGiaKPIConstants.XepLoai.ChuaCauHinh, "Chưa xác định");
            }

            return (cauHinh.XepLoai, DanhGiaKPIConstants.GetDisplayLabel(cauHinh.XepLoai));
        }

        private static bool LaKyTruoc(KyBaoCaoKPI kyCanSoSanh, KyBaoCaoKPI kyHienTai)
        {
            if (kyCanSoSanh.Nam < kyHienTai.Nam)
            {
                return true;
            }

            if (kyCanSoSanh.Nam > kyHienTai.Nam)
            {
                return false;
            }

            var thuTuKyCanSoSanh = ThuTuLoaiKy(kyCanSoSanh.LoaiKy);
            var thuTuKyHienTai = ThuTuLoaiKy(kyHienTai.LoaiKy);

            if (thuTuKyCanSoSanh < thuTuKyHienTai)
            {
                return true;
            }

            if (thuTuKyCanSoSanh > thuTuKyHienTai)
            {
                return false;
            }

            return (kyCanSoSanh.SoKy ?? 0) < (kyHienTai.SoKy ?? 0);
        }

        private static int ThuTuLoaiKy(string? loaiKy)
        {
            return NormalizeKey(loaiKy) switch
            {
                "THANG" => 1,
                "QUY" => 2,
                "6THANG" => 3,
                "NAM" => 4,
                _ => 99
            };
        }

        private static string NormalizeKey(string? value)
        {
            var normalized = (value ?? string.Empty)
                .Trim()
                .Normalize(NormalizationForm.FormD);

            var chars = normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars)
                .Replace('đ', 'd')
                .Replace('Đ', 'D')
                .Trim()
                .ToUpper()
                .Replace(" ", "_");
        }

        private static readonly HashSet<string> DinhTinhHoanThanhOptions = new()
        {
            "KHONG_XAY_RA",
            "DAM_BAO",
            "DAT_100"
        };

        private static readonly HashSet<string> DinhTinhKhongHoanThanhOptions = new()
        {
            "XAY_RA",
            "KHONG_DAM_BAO",
            "KHONG_DAT"
        };

        private async Task<CauHinhNguongDanhGiaKPI?> LoadNguongDanhGiaAsync(
            long? danhMucChiTieuId,
            decimal tyLeHoanThanh,
            string dieuKienThoiHan)
        {
            var baseQuery = _context.CauHinhNguongDanhGiaKPIs
                .Where(x =>
                    (x.DanhMucChiTieuId == danhMucChiTieuId || x.DanhMucChiTieuId == null) &&
                    x.TuTyLe <= tyLeHoanThanh &&
                    x.DenTyLe >= tyLeHoanThanh);

            var exactMatch = await baseQuery
                .Where(x => x.DieuKienThoiHan == dieuKienThoiHan)
                .OrderByDescending(x => x.DanhMucChiTieuId.HasValue)
                .ThenBy(x => x.TuTyLe)
                .FirstOrDefaultAsync();

            if (exactMatch != null || dieuKienThoiHan == DanhGiaKPIConstants.DieuKienThoiHan.MacDinh)
            {
                return exactMatch;
            }

            return await baseQuery
                .Where(x => x.DieuKienThoiHan == DanhGiaKPIConstants.DieuKienThoiHan.MacDinh)
                .OrderByDescending(x => x.DanhMucChiTieuId.HasValue)
                .ThenBy(x => x.TuTyLe)
                .FirstOrDefaultAsync();
        }

        private static string XacDinhDieuKienThoiHan(
            ChiTietGiaoChiTieu chiTiet,
            KyBaoCaoKPI kyBaoCao,
            decimal tyLeHoanThanh)
        {
            if (tyLeHoanThanh >= 100m)
            {
                return DanhGiaKPIConstants.DieuKienThoiHan.MacDinh;
            }

            var ngayKetThuc = chiTiet.DotGiaoChiTieu?.NgayKetThuc;
            if (!ngayKetThuc.HasValue)
            {
                return DanhGiaKPIConstants.DieuKienThoiHan.MacDinh;
            }

            return kyBaoCao.NgayCuoiKy < ngayKetThuc.Value
                ? DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan
                : DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan;
        }

        private static string XacDinhTrangThaiThatBai(ChiTietGiaoChiTieu chiTiet, KyBaoCaoKPI kyBaoCao)
        {
            return XacDinhDieuKienThoiHan(chiTiet, kyBaoCao, 0m) switch
            {
                var value when value == DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan => DanhGiaKPIConstants.XepLoai.KhongHoanThanh,
                var value when value == DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan => DanhGiaKPIConstants.XepLoai.ChuaHoanThanh,
                _ => DanhGiaKPIConstants.XepLoai.ChuaHoanThanh
            };
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

        private sealed class ComparisonContext
        {
            public decimal? CungKyNamTruocGiaTriCuoiKy { get; set; }
            public decimal? CungKyNamTruocGiaTriLuyKe { get; set; }
            public decimal? KyTruocGiaTriCuoiKy { get; set; }
            public decimal? KyTruocGiaTriLuyKe { get; set; }
            public decimal? TongNamTruocGiaTriLuyKe { get; set; }
        }
    }
}
