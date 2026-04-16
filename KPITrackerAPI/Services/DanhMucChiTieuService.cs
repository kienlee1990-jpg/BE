using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.DanhMucChiTieu;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class DanhMucChiTieuService : IDanhMucChiTieuService
    {
        private const string LoaiChiTieuPhanRa = "PHAN_RA";

        private readonly ApplicationDbContext _context;

        public DanhMucChiTieuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DanhMucChiTieuResponseDto> CreateAsync(CreateDanhMucChiTieuDto dto)
        {
            await EnsureCodeNotExistsAsync(dto.MaChiTieu);

            var coTieuChiCon = dto.TieuChiDanhGias.Count > 0;
            await ValidateCatalogRequestAsync(
                dto.MaChiTieu,
                dto.LoaiChiTieu,
                dto.DieuKienHoanThanh,
                dto.DieuKienKhongHoanThanh,
                dto.TyLePhanTramMucTieu,
                dto.LoaiMocSoSanh,
                dto.ChieuSoSanh,
                dto.TieuChiDanhGias);

            var entity = new DanhMucChiTieu
            {
                MaChiTieu = dto.MaChiTieu.Trim(),
                TenChiTieu = dto.TenChiTieu.Trim(),
                NguonChiTieu = dto.NguonChiTieu.Trim().ToUpper(),
                LoaiChiTieu = coTieuChiCon ? LoaiChiTieuPhanRa : dto.LoaiChiTieu.Trim().ToUpper(),
                CapApDung = dto.CapApDung.Trim().ToUpper(),
                LinhVucNghiepVu = NormalizeNullable(dto.LinhVucNghiepVu),
                DonViTinh = coTieuChiCon ? null : NormalizeNullable(dto.DonViTinh),
                MoTa = NormalizeNullable(dto.MoTa),
                HuongDanTinhToan = NormalizeNullable(dto.HuongDanTinhToan),
                CoChoPhepPhanRa = dto.CoChoPhepPhanRa || coTieuChiCon,
                TrangThaiSuDung = "DANG_AP_DUNG",
                NgayHieuLuc = dto.NgayHieuLuc,
                NgayHetHieuLuc = dto.NgayHetHieuLuc,
                DieuKienHoanThanh = coTieuChiCon ? null : NormalizeNullable(dto.DieuKienHoanThanh),
                DieuKienKhongHoanThanh = coTieuChiCon ? null : NormalizeNullable(dto.DieuKienKhongHoanThanh),
                TyLePhanTramMucTieu = coTieuChiCon ? null : dto.TyLePhanTramMucTieu,
                LoaiMocSoSanh = coTieuChiCon ? null : NormalizeNullable(dto.LoaiMocSoSanh)?.ToUpper(),
                ChieuSoSanh = coTieuChiCon ? null : NormalizeNullable(dto.ChieuSoSanh)?.ToUpper(),
                BatBuocDatTatCaTieuChiCon = dto.BatBuocDatTatCaTieuChiCon,
                CreatedAt = DateTime.UtcNow
            };

            _context.DanhMucChiTieus.Add(entity);
            await _context.SaveChangesAsync();

            if (coTieuChiCon)
            {
                var children = dto.TieuChiDanhGias
                    .Select((x, index) => MapChildEntity(entity, dto, x, index))
                    .ToList();

                _context.DanhMucChiTieus.AddRange(children);
                await _context.SaveChangesAsync();
            }

            var created = await LoadTopLevelCatalogAsync(entity.Id);
            return MapToResponse(created!);
        }

        public async Task<List<DanhMucChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            string? nguonChiTieu,
            string? loaiChiTieu,
            string? capApDung,
            string? trangThaiSuDung,
            bool? coChoPhepPhanRa)
        {
            var query = _context.DanhMucChiTieus
                .AsNoTracking()
                .Where(x => x.ChiTieuChaId == null)
                .Include(x => x.TieuChiCons)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.MaChiTieu.ToLower().Contains(normalizedKeyword) ||
                    x.TenChiTieu.ToLower().Contains(normalizedKeyword));
            }

            if (!string.IsNullOrWhiteSpace(nguonChiTieu))
            {
                var normalizedNguon = nguonChiTieu.Trim().ToUpper();
                query = query.Where(x => x.NguonChiTieu == normalizedNguon);
            }

            if (!string.IsNullOrWhiteSpace(loaiChiTieu))
            {
                var normalizedLoai = loaiChiTieu.Trim().ToUpper();
                query = query.Where(x => x.LoaiChiTieu == normalizedLoai);
            }

            if (!string.IsNullOrWhiteSpace(capApDung))
            {
                var normalizedCap = capApDung.Trim().ToUpper();
                query = query.Where(x => x.CapApDung == normalizedCap);
            }

            if (!string.IsNullOrWhiteSpace(trangThaiSuDung))
            {
                var normalizedTrangThai = trangThaiSuDung.Trim().ToUpper();
                query = query.Where(x => x.TrangThaiSuDung == normalizedTrangThai);
            }

            if (coChoPhepPhanRa.HasValue)
            {
                query = query.Where(x => x.CoChoPhepPhanRa == coChoPhepPhanRa.Value);
            }

            var data = await query
                .OrderBy(x => x.MaChiTieu)
                .ToListAsync();

            return data.Select(MapToResponse).ToList();
        }

        public async Task<DanhMucChiTieuResponseDto?> GetByIdAsync(long id)
        {
            var entity = await LoadTopLevelCatalogAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<DanhMucChiTieuResponseDto?> UpdateAsync(long id, UpdateDanhMucChiTieuDto dto)
        {
            var entity = await _context.DanhMucChiTieus
                .Include(x => x.TieuChiCons)
                .FirstOrDefaultAsync(x => x.Id == id && x.ChiTieuChaId == null);

            if (entity == null)
            {
                return null;
            }

            var coTieuChiCon = dto.TieuChiDanhGias.Count > 0;
            await ValidateCatalogRequestAsync(
                entity.MaChiTieu,
                dto.LoaiChiTieu,
                dto.DieuKienHoanThanh,
                dto.DieuKienKhongHoanThanh,
                dto.TyLePhanTramMucTieu,
                dto.LoaiMocSoSanh,
                dto.ChieuSoSanh,
                dto.TieuChiDanhGias,
                entity.Id,
                entity.TieuChiCons.Select(x => x.Id).ToHashSet());

            entity.TenChiTieu = dto.TenChiTieu.Trim();
            entity.NguonChiTieu = dto.NguonChiTieu.Trim().ToUpper();
            entity.LoaiChiTieu = coTieuChiCon ? LoaiChiTieuPhanRa : dto.LoaiChiTieu.Trim().ToUpper();
            entity.CapApDung = dto.CapApDung.Trim().ToUpper();
            entity.LinhVucNghiepVu = NormalizeNullable(dto.LinhVucNghiepVu);
            entity.DonViTinh = coTieuChiCon ? null : NormalizeNullable(dto.DonViTinh);
            entity.MoTa = NormalizeNullable(dto.MoTa);
            entity.HuongDanTinhToan = NormalizeNullable(dto.HuongDanTinhToan);
            entity.CoChoPhepPhanRa = dto.CoChoPhepPhanRa || coTieuChiCon;
            entity.TrangThaiSuDung = string.IsNullOrWhiteSpace(dto.TrangThaiSuDung)
                ? entity.TrangThaiSuDung
                : dto.TrangThaiSuDung.Trim().ToUpper();
            entity.NgayHieuLuc = dto.NgayHieuLuc;
            entity.NgayHetHieuLuc = dto.NgayHetHieuLuc;
            entity.DieuKienHoanThanh = coTieuChiCon ? null : NormalizeNullable(dto.DieuKienHoanThanh);
            entity.DieuKienKhongHoanThanh = coTieuChiCon ? null : NormalizeNullable(dto.DieuKienKhongHoanThanh);
            entity.TyLePhanTramMucTieu = coTieuChiCon ? null : dto.TyLePhanTramMucTieu;
            entity.LoaiMocSoSanh = coTieuChiCon ? null : NormalizeNullable(dto.LoaiMocSoSanh)?.ToUpper();
            entity.ChieuSoSanh = coTieuChiCon ? null : NormalizeNullable(dto.ChieuSoSanh)?.ToUpper();
            entity.BatBuocDatTatCaTieuChiCon = dto.BatBuocDatTatCaTieuChiCon;
            entity.UpdatedAt = DateTime.UtcNow;

            await SyncChildCriteriaAsync(entity, dto.TieuChiDanhGias, dto);
            await _context.SaveChangesAsync();

            var updated = await LoadTopLevelCatalogAsync(id);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DanhMucChiTieus
                .Include(x => x.TieuChiCons)
                .Include(x => x.ChiTietGiaoChiTieux)
                .FirstOrDefaultAsync(x => x.Id == id && x.ChiTieuChaId == null);

            if (entity == null)
            {
                return false;
            }

            var childIds = entity.TieuChiCons.Select(x => x.Id).ToList();
            var hasAssignments = entity.ChiTietGiaoChiTieux.Any() ||
                                 await _context.ChiTietGiaoChiTieus.AnyAsync(x => childIds.Contains(x.DanhMucChiTieuId));

            if (hasAssignments)
            {
                throw new Exception("Khong the xoa danh muc chi tieu da duoc giao cho don vi.");
            }

            if (entity.TieuChiCons.Count > 0)
            {
                _context.DanhMucChiTieus.RemoveRange(entity.TieuChiCons);
            }

            _context.DanhMucChiTieus.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<DanhMucChiTieu?> LoadTopLevelCatalogAsync(long id)
        {
            return await _context.DanhMucChiTieus
                .AsNoTracking()
                .Include(x => x.TieuChiCons)
                .FirstOrDefaultAsync(x => x.Id == id && x.ChiTieuChaId == null);
        }

        private async Task ValidateCatalogRequestAsync(
            string maChiTieu,
            string loaiChiTieu,
            string? dieuKienHoanThanh,
            string? dieuKienKhongHoanThanh,
            decimal? tyLePhanTramMucTieu,
            string? loaiMocSoSanh,
            string? chieuSoSanh,
            List<TieuChiDanhGiaDto> tieuChiDanhGias,
            long? currentParentId = null,
            HashSet<long>? currentChildIds = null)
        {
            if (tieuChiDanhGias.Count == 0)
            {
                ValidateCriterionRules(
                    loaiChiTieu,
                    dieuKienHoanThanh,
                    dieuKienKhongHoanThanh,
                    tyLePhanTramMucTieu,
                    loaiMocSoSanh,
                    chieuSoSanh);

                return;
            }

            if (tieuChiDanhGias.Count < 2)
            {
                throw new Exception("Chi tieu phan ra phai co it nhat 2 tieu chi danh gia.");
            }

            var normalizedParentCode = maChiTieu.Trim().ToUpper();
            var usedCodes = new HashSet<string> { normalizedParentCode };

            foreach (var child in tieuChiDanhGias)
            {
                if (string.IsNullOrWhiteSpace(child.MaChiTieu))
                {
                    throw new Exception("Moi tieu chi danh gia phai co ma chi tieu.");
                }

                if (string.IsNullOrWhiteSpace(child.TenChiTieu))
                {
                    throw new Exception("Moi tieu chi danh gia phai co ten chi tieu.");
                }

                var normalizedCode = child.MaChiTieu.Trim().ToUpper();
                if (!usedCodes.Add(normalizedCode))
                {
                    throw new Exception($"Ma chi tieu '{child.MaChiTieu}' bi trung trong cau hinh phan ra.");
                }

                await EnsureCodeNotExistsAsync(child.MaChiTieu, currentParentId, currentChildIds, child.Id);
                ValidateCriterionRules(
                    child.LoaiChiTieu,
                    child.DieuKienHoanThanh,
                    child.DieuKienKhongHoanThanh,
                    child.TyLePhanTramMucTieu,
                    child.LoaiMocSoSanh,
                    child.ChieuSoSanh);
            }
        }

        private async Task EnsureCodeNotExistsAsync(
            string maChiTieu,
            long? currentParentId = null,
            HashSet<long>? currentChildIds = null,
            long? currentChildId = null)
        {
            var normalizedCode = maChiTieu.Trim();

            var existing = await _context.DanhMucChiTieus
                .Where(x => x.MaChiTieu == normalizedCode)
                .Select(x => new { x.Id })
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                return;
            }

            if (currentParentId.HasValue && existing.Id == currentParentId.Value)
            {
                return;
            }

            if (currentChildId.HasValue && existing.Id == currentChildId.Value)
            {
                return;
            }

            if (currentChildIds != null && currentChildIds.Contains(existing.Id))
            {
                return;
            }

            throw new Exception($"Ma chi tieu '{maChiTieu}' da ton tai.");
        }

        private async Task SyncChildCriteriaAsync(
            DanhMucChiTieu parent,
            List<TieuChiDanhGiaDto> requestChildren,
            UpdateDanhMucChiTieuDto dto)
        {
            var currentChildren = parent.TieuChiCons.ToDictionary(x => x.Id);
            var requestChildIds = requestChildren.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
            var removedChildren = parent.TieuChiCons.Where(x => !requestChildIds.Contains(x.Id)).ToList();

            if (removedChildren.Count > 0)
            {
                var removedIds = removedChildren.Select(x => x.Id).ToList();
                var hasAssignments = await _context.ChiTietGiaoChiTieus.AnyAsync(x => removedIds.Contains(x.DanhMucChiTieuId));
                if (hasAssignments)
                {
                    throw new Exception("Khong the xoa tieu chi con da duoc giao cho don vi.");
                }

                _context.DanhMucChiTieus.RemoveRange(removedChildren);
            }

            if (requestChildren.Count == 0)
            {
                return;
            }

            foreach (var childDto in requestChildren.Select((value, index) => new { value, index }))
            {
                if (childDto.value.Id.HasValue && currentChildren.TryGetValue(childDto.value.Id.Value, out var existingChild))
                {
                    existingChild.MaChiTieu = childDto.value.MaChiTieu.Trim();
                    existingChild.TenChiTieu = childDto.value.TenChiTieu.Trim();
                    existingChild.LoaiChiTieu = childDto.value.LoaiChiTieu.Trim().ToUpper();
                    existingChild.DonViTinh = NormalizeNullable(childDto.value.DonViTinh);
                    existingChild.MoTa = NormalizeNullable(childDto.value.MoTa);
                    existingChild.HuongDanTinhToan = NormalizeNullable(childDto.value.HuongDanTinhToan);
                    existingChild.DieuKienHoanThanh = NormalizeNullable(childDto.value.DieuKienHoanThanh);
                    existingChild.DieuKienKhongHoanThanh = NormalizeNullable(childDto.value.DieuKienKhongHoanThanh);
                    existingChild.TyLePhanTramMucTieu = childDto.value.TyLePhanTramMucTieu;
                    existingChild.LoaiMocSoSanh = NormalizeNullable(childDto.value.LoaiMocSoSanh)?.ToUpper();
                    existingChild.ChieuSoSanh = NormalizeNullable(childDto.value.ChieuSoSanh)?.ToUpper();
                    existingChild.ThuTuHienThi = childDto.value.ThuTuHienThi ?? childDto.index + 1;
                    existingChild.TrangThaiSuDung = parent.TrangThaiSuDung;
                    existingChild.NgayHieuLuc = dto.NgayHieuLuc;
                    existingChild.NgayHetHieuLuc = dto.NgayHetHieuLuc;
                    existingChild.UpdatedAt = DateTime.UtcNow;
                    continue;
                }

                _context.DanhMucChiTieus.Add(MapChildEntity(parent, dto, childDto.value, childDto.index));
            }
        }

        private static DanhMucChiTieu MapChildEntity(
            DanhMucChiTieu parent,
            CreateDanhMucChiTieuDto parentDto,
            TieuChiDanhGiaDto childDto,
            int index)
        {
            return new DanhMucChiTieu
            {
                MaChiTieu = childDto.MaChiTieu.Trim(),
                TenChiTieu = childDto.TenChiTieu.Trim(),
                NguonChiTieu = parentDto.NguonChiTieu.Trim().ToUpper(),
                LoaiChiTieu = childDto.LoaiChiTieu.Trim().ToUpper(),
                CapApDung = parentDto.CapApDung.Trim().ToUpper(),
                LinhVucNghiepVu = NormalizeNullable(parentDto.LinhVucNghiepVu),
                DonViTinh = NormalizeNullable(childDto.DonViTinh),
                MoTa = NormalizeNullable(childDto.MoTa),
                HuongDanTinhToan = NormalizeNullable(childDto.HuongDanTinhToan),
                CoChoPhepPhanRa = false,
                TrangThaiSuDung = "DANG_AP_DUNG",
                NgayHieuLuc = parentDto.NgayHieuLuc,
                NgayHetHieuLuc = parentDto.NgayHetHieuLuc,
                DieuKienHoanThanh = NormalizeNullable(childDto.DieuKienHoanThanh),
                DieuKienKhongHoanThanh = NormalizeNullable(childDto.DieuKienKhongHoanThanh),
                TyLePhanTramMucTieu = childDto.TyLePhanTramMucTieu,
                LoaiMocSoSanh = NormalizeNullable(childDto.LoaiMocSoSanh)?.ToUpper(),
                ChieuSoSanh = NormalizeNullable(childDto.ChieuSoSanh)?.ToUpper(),
                ChiTieuChaId = parent.Id,
                ThuTuHienThi = childDto.ThuTuHienThi ?? index + 1,
                BatBuocDatTatCaTieuChiCon = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static DanhMucChiTieu MapChildEntity(
            DanhMucChiTieu parent,
            UpdateDanhMucChiTieuDto parentDto,
            TieuChiDanhGiaDto childDto,
            int index)
        {
            return new DanhMucChiTieu
            {
                MaChiTieu = childDto.MaChiTieu.Trim(),
                TenChiTieu = childDto.TenChiTieu.Trim(),
                NguonChiTieu = parentDto.NguonChiTieu.Trim().ToUpper(),
                LoaiChiTieu = childDto.LoaiChiTieu.Trim().ToUpper(),
                CapApDung = parentDto.CapApDung.Trim().ToUpper(),
                LinhVucNghiepVu = NormalizeNullable(parentDto.LinhVucNghiepVu),
                DonViTinh = NormalizeNullable(childDto.DonViTinh),
                MoTa = NormalizeNullable(childDto.MoTa),
                HuongDanTinhToan = NormalizeNullable(childDto.HuongDanTinhToan),
                CoChoPhepPhanRa = false,
                TrangThaiSuDung = "DANG_AP_DUNG",
                NgayHieuLuc = parentDto.NgayHieuLuc,
                NgayHetHieuLuc = parentDto.NgayHetHieuLuc,
                DieuKienHoanThanh = NormalizeNullable(childDto.DieuKienHoanThanh),
                DieuKienKhongHoanThanh = NormalizeNullable(childDto.DieuKienKhongHoanThanh),
                TyLePhanTramMucTieu = childDto.TyLePhanTramMucTieu,
                LoaiMocSoSanh = NormalizeNullable(childDto.LoaiMocSoSanh)?.ToUpper(),
                ChieuSoSanh = NormalizeNullable(childDto.ChieuSoSanh)?.ToUpper(),
                ChiTieuChaId = parent.Id,
                ThuTuHienThi = childDto.ThuTuHienThi ?? index + 1,
                BatBuocDatTatCaTieuChiCon = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static DanhMucChiTieuResponseDto MapToResponse(DanhMucChiTieu entity)
        {
            var children = (entity.TieuChiCons ?? Array.Empty<DanhMucChiTieu>())
                .OrderBy(x => x.ThuTuHienThi ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(MapToResponse)
                .ToList();

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
                ChiTieuChaId = entity.ChiTieuChaId,
                ThuTuHienThi = entity.ThuTuHienThi,
                BatBuocDatTatCaTieuChiCon = entity.BatBuocDatTatCaTieuChiCon,
                TieuChiDanhGias = children,
                CreatedAt = entity.CreatedAt
            };
        }

        private static void ValidateCriterionRules(
            string loaiChiTieu,
            string? dieuKienHoanThanh,
            string? dieuKienKhongHoanThanh,
            decimal? tyLePhanTramMucTieu,
            string? loaiMocSoSanh,
            string? chieuSoSanh)
        {
            var normalizedLoai = loaiChiTieu.Trim().ToUpper();
            var allowed = new[] { "DINH_TINH", "DINH_LUONG_TICH_LUY", "DINH_LUONG_SO_SANH" };

            if (!allowed.Contains(normalizedLoai))
            {
                throw new Exception("Loai chi tieu khong hop le.");
            }

            if (normalizedLoai == "DINH_TINH")
            {
                if (string.IsNullOrWhiteSpace(dieuKienHoanThanh) || string.IsNullOrWhiteSpace(dieuKienKhongHoanThanh))
                {
                    throw new Exception("Chi tieu dinh tinh phai co dieu kien hoan thanh va khong hoan thanh.");
                }
            }

            if (normalizedLoai == "DINH_LUONG_SO_SANH")
            {
                if (!tyLePhanTramMucTieu.HasValue || tyLePhanTramMucTieu.Value <= 0)
                {
                    throw new Exception("Chi tieu dinh luong so sanh phai co ty le phan tram muc tieu > 0.");
                }

                if (string.IsNullOrWhiteSpace(loaiMocSoSanh))
                {
                    throw new Exception("Chi tieu dinh luong so sanh phai co loai moc so sanh.");
                }

                if (string.IsNullOrWhiteSpace(chieuSoSanh))
                {
                    throw new Exception("Chi tieu dinh luong so sanh phai co chieu so sanh.");
                }
            }
        }

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
