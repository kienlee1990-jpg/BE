using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.ChiTietGiaoChiTieu;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class ChiTietGiaoChiTieuService : IChiTietGiaoChiTieuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDanhGiaKPIService _danhGiaKPIService;

        private static readonly HashSet<string> TanSuatBaoCaoHopLe = new()
        {
            "THANG",
            "QUY",
            "6THANG",
            "NAM"
        };

        public ChiTietGiaoChiTieuService(
            ApplicationDbContext context,
            IDanhGiaKPIService danhGiaKPIService)
        {
            _context = context;
            _danhGiaKPIService = danhGiaKPIService;
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetAllAsync()
        {
            var items = await BuildTopLevelQuery()
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(x => MapToDto(x)).ToList();
        }

        public async Task<ChiTietGiaoChiTieuDto?> GetByIdAsync(long id)
        {
            var item = await BuildTopLevelQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetByDotGiaoChiTieuIdAsync(long dotGiaoChiTieuId)
        {
            var items = await BuildTopLevelQuery()
                .Where(x => x.DotGiaoChiTieuId == dotGiaoChiTieuId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(x => MapToDto(x)).ToList();
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetByDonViNhanIdAsync(long donViNhanId)
        {
            var items = await BuildTopLevelQuery()
                .Where(x => x.DonViNhanId == donViNhanId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(x => MapToDto(x)).ToList();
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetChildrenAsync(long chiTietGiaoChaId)
        {
            var items = await _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .Where(x => x.ChiTietGiaoChaId == chiTietGiaoChaId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(x => MapToDto(x, false)).ToList();
        }

        public async Task<ChiTietGiaoChiTieuDto> CreateAsync(CreateChiTietGiaoChiTieuDto dto)
        {
            var danhMuc = await LoadDanhMucWithChildrenAsync(dto.DanhMucChiTieuId);
            if (danhMuc == null)
            {
                throw new Exception("DanhMucChiTieuId khong ton tai.");
            }

            await ValidateForeignKeys(
                dto.DotGiaoChiTieuId,
                dto.DanhMucChiTieuId,
                dto.DonViNhanId,
                dto.DonViThucHienChinhId,
                dto.ChiTietGiaoChaId);

            var tanSuatBaoCao = NormalizeTanSuatBaoCao(dto.TanSuatBaoCao);
            var childCriteria = danhMuc.TieuChiCons.OrderBy(x => x.ThuTuHienThi ?? int.MaxValue).ThenBy(x => x.Id).ToList();
            var assignmentIds = childCriteria.Select(x => x.Id).Append(danhMuc.Id).ToList();
            await EnsureAssignmentNotExistsAsync(dto.DotGiaoChiTieuId, dto.DonViNhanId, assignmentIds);

            ValidateGiaTriDauKyCoDinh(danhMuc.LoaiChiTieu, dto.GiaTriDauKyCoDinh, "chi tieu");
            ValidateChildAssignmentPayload(childCriteria, dto.TieuChiCon);

            var parent = BuildEntity(dto, danhMuc, tanSuatBaoCao);
            _context.ChiTietGiaoChiTieus.Add(parent);
            await _context.SaveChangesAsync();

            if (childCriteria.Count > 0)
            {
                var childEntities = BuildChildAssignments(parent, dto, childCriteria, tanSuatBaoCao);
                _context.ChiTietGiaoChiTieus.AddRange(childEntities);
                await _context.SaveChangesAsync();
            }

            var created = await BuildTopLevelQuery().FirstAsync(x => x.Id == parent.Id);
            return MapToDto(created);
        }

        public async Task<ChiTietGiaoChiTieuDto?> UpdateAsync(long id, UpdateChiTietGiaoChiTieuDto dto)
        {
            var entity = await _context.ChiTietGiaoChiTieus
                .Include(x => x.ChiTietGiaoChiTieuCons)
                .FirstOrDefaultAsync(x => x.Id == id && x.ChiTietGiaoChaId == null);

            if (entity == null)
            {
                return null;
            }

            var danhMuc = await LoadDanhMucWithChildrenAsync(dto.DanhMucChiTieuId);
            if (danhMuc == null)
            {
                throw new Exception("DanhMucChiTieuId khong ton tai.");
            }

            await ValidateForeignKeys(
                dto.DotGiaoChiTieuId,
                dto.DanhMucChiTieuId,
                dto.DonViNhanId,
                dto.DonViThucHienChinhId,
                dto.ChiTietGiaoChaId,
                id);

            var tanSuatBaoCao = NormalizeTanSuatBaoCao(dto.TanSuatBaoCao);
            var childCriteria = danhMuc.TieuChiCons.OrderBy(x => x.ThuTuHienThi ?? int.MaxValue).ThenBy(x => x.Id).ToList();
            var excludeIds = entity.ChiTietGiaoChiTieuCons.Select(x => x.Id).Append(entity.Id).ToList();
            var assignmentIds = childCriteria.Select(x => x.Id).Append(danhMuc.Id).ToList();
            await EnsureAssignmentNotExistsAsync(dto.DotGiaoChiTieuId, dto.DonViNhanId, assignmentIds, excludeIds);

            ValidateGiaTriDauKyCoDinh(danhMuc.LoaiChiTieu, dto.GiaTriDauKyCoDinh, "chi tieu");
            ValidateChildAssignmentPayload(childCriteria, dto.TieuChiCon);

            entity.DotGiaoChiTieuId = dto.DotGiaoChiTieuId;
            entity.DanhMucChiTieuId = dto.DanhMucChiTieuId;
            entity.DonViNhanId = dto.DonViNhanId;
            entity.DonViThucHienChinhId = dto.DonViThucHienChinhId;
            entity.GiaTriMucTieu = dto.GiaTriMucTieu;
            entity.GiaTriMucTieuText = NormalizeNullable(dto.GiaTriMucTieuText);
            entity.GiaTriDauKyCoDinh = ResolveGiaTriDauKyCoDinh(danhMuc.LoaiChiTieu, dto.GiaTriDauKyCoDinh);
            entity.ChiTietGiaoChaId = dto.ChiTietGiaoChaId;
            entity.GhiChu = NormalizeNullable(dto.GhiChu);
            entity.ThuTuHienThi = dto.ThuTuHienThi;
            entity.TanSuatBaoCao = tanSuatBaoCao;
            entity.TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? entity.TrangThai : dto.TrangThai.Trim().ToUpper();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = dto.UpdatedBy;

            await SyncChildAssignmentsAsync(entity, dto, childCriteria, tanSuatBaoCao);
            await _context.SaveChangesAsync();

            var affectedChiTietIds = await _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .Where(x => x.Id == entity.Id || x.ChiTietGiaoChaId == entity.Id)
                .Select(x => x.Id)
                .ToListAsync();

            await RefreshMonitoringAndDanhGiaAsync(affectedChiTietIds, dto.UpdatedBy);

            var updated = await BuildTopLevelQuery().FirstAsync(x => x.Id == entity.Id);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.ChiTietGiaoChiTieus
                .Include(x => x.ChiTietGiaoChiTieuCons)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return false;
            }

            if (entity.ChiTietGiaoChiTieuCons.Any())
            {
                var childIds = entity.ChiTietGiaoChiTieuCons.Select(x => x.Id).ToList();
                var hasActivity = await HasRelatedMonitoringAsync(childIds);
                if (hasActivity)
                {
                    throw new Exception("Khong the xoa chi tiet giao da co du lieu theo doi hoac danh gia.");
                }

                _context.ChiTietGiaoChiTieus.RemoveRange(entity.ChiTietGiaoChiTieuCons);
            }

            var selfHasActivity = await HasRelatedMonitoringAsync(new List<long> { entity.Id });
            if (selfHasActivity)
            {
                throw new Exception("Khong the xoa chi tiet giao da co du lieu theo doi hoac danh gia.");
            }

            _context.ChiTietGiaoChiTieus.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<ChiTietGiaoChiTieu> BuildTopLevelQuery()
        {
            return _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .Where(x => x.ChiTietGiaoChaId == null)
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .Include(x => x.ChiTietGiaoChiTieuCons)
                    .ThenInclude(x => x.DanhMucChiTieu)
                .Include(x => x.ChiTietGiaoChiTieuCons)
                    .ThenInclude(x => x.DonViNhan)
                .Include(x => x.ChiTietGiaoChiTieuCons)
                    .ThenInclude(x => x.DonViThucHienChinh)
                .Include(x => x.ChiTietGiaoChiTieuCons)
                    .ThenInclude(x => x.DotGiaoChiTieu);
        }

        private async Task<DanhMucChiTieu?> LoadDanhMucWithChildrenAsync(long danhMucChiTieuId)
        {
            return await _context.DanhMucChiTieus
                .Include(x => x.TieuChiCons)
                .FirstOrDefaultAsync(x => x.Id == danhMucChiTieuId && x.ChiTieuChaId == null);
        }

        private async Task ValidateForeignKeys(
            long dotGiaoChiTieuId,
            long danhMucChiTieuId,
            long donViNhanId,
            long? donViThucHienChinhId,
            long? chiTietGiaoChaId,
            long? currentId = null)
        {
            var dotExists = await _context.DotGiaoChiTieus.AnyAsync(x => x.Id == dotGiaoChiTieuId);
            if (!dotExists)
            {
                throw new Exception("DotGiaoChiTieuId khong ton tai.");
            }

            var danhMucExists = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == danhMucChiTieuId);
            if (!danhMucExists)
            {
                throw new Exception("DanhMucChiTieuId khong ton tai.");
            }

            var donViNhanExists = await _context.DonVis.AnyAsync(x => x.Id == donViNhanId);
            if (!donViNhanExists)
            {
                throw new Exception("DonViNhanId khong ton tai.");
            }

            if (donViThucHienChinhId.HasValue)
            {
                var donViThucHienExists = await _context.DonVis.AnyAsync(x => x.Id == donViThucHienChinhId.Value);
                if (!donViThucHienExists)
                {
                    throw new Exception("DonViThucHienChinhId khong ton tai.");
                }
            }

            if (chiTietGiaoChaId.HasValue)
            {
                if (currentId.HasValue && chiTietGiaoChaId.Value == currentId.Value)
                {
                    throw new Exception("ChiTietGiaoChaId khong duoc tro toi chinh no.");
                }

                var chaExists = await _context.ChiTietGiaoChiTieus.AnyAsync(x => x.Id == chiTietGiaoChaId.Value);
                if (!chaExists)
                {
                    throw new Exception("ChiTietGiaoChaId khong ton tai.");
                }
            }
        }

        private async Task EnsureAssignmentNotExistsAsync(
            long dotGiaoChiTieuId,
            long donViNhanId,
            List<long> danhMucChiTieuIds,
            List<long>? excludeAssignmentIds = null)
        {
            var query = _context.ChiTietGiaoChiTieus.Where(x =>
                x.DotGiaoChiTieuId == dotGiaoChiTieuId &&
                x.DonViNhanId == donViNhanId &&
                danhMucChiTieuIds.Contains(x.DanhMucChiTieuId));

            if (excludeAssignmentIds != null && excludeAssignmentIds.Count > 0)
            {
                query = query.Where(x => !excludeAssignmentIds.Contains(x.Id));
            }

            var exists = await query.AnyAsync();
            if (exists)
            {
                throw new Exception("Da ton tai giao chi tieu cho dot giao, don vi va danh muc da chon.");
            }
        }

        private static void ValidateChildAssignmentPayload(
            List<DanhMucChiTieu> childCriteria,
            List<ChiTietTieuChiConDto> requestChildren)
        {
            if (childCriteria.Count == 0)
            {
                return;
            }

            if (requestChildren.Count != childCriteria.Count)
            {
                throw new Exception("Can nhap day du muc tieu cho tung tieu chi con.");
            }

            var expectedIds = childCriteria.Select(x => x.Id).OrderBy(x => x).ToList();
            var providedIds = requestChildren.Select(x => x.DanhMucChiTieuId).OrderBy(x => x).ToList();

            if (!expectedIds.SequenceEqual(providedIds))
            {
                throw new Exception("Danh sach tieu chi con gui len khong khop voi danh muc chi tieu da chon.");
            }

            foreach (var criterion in childCriteria)
            {
                var payload = requestChildren.First(x => x.DanhMucChiTieuId == criterion.Id);
                ValidateGiaTriDauKyCoDinh(criterion.LoaiChiTieu, payload.GiaTriDauKyCoDinh, $"tieu chi con '{criterion.TenChiTieu}'");
            }
        }

        private static ChiTietGiaoChiTieu BuildEntity(
            CreateChiTietGiaoChiTieuDto dto,
            DanhMucChiTieu danhMuc,
            string? tanSuatBaoCao)
        {
            return new ChiTietGiaoChiTieu
            {
                DotGiaoChiTieuId = dto.DotGiaoChiTieuId,
                DanhMucChiTieuId = dto.DanhMucChiTieuId,
                DonViNhanId = dto.DonViNhanId,
                DonViThucHienChinhId = dto.DonViThucHienChinhId,
                GiaTriMucTieu = dto.GiaTriMucTieu,
                GiaTriMucTieuText = NormalizeNullable(dto.GiaTriMucTieuText),
                GiaTriDauKyCoDinh = ResolveGiaTriDauKyCoDinh(danhMuc.LoaiChiTieu, dto.GiaTriDauKyCoDinh),
                ChiTietGiaoChaId = dto.ChiTietGiaoChaId,
                GhiChu = NormalizeNullable(dto.GhiChu),
                ThuTuHienThi = dto.ThuTuHienThi,
                TanSuatBaoCao = tanSuatBaoCao,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "DA_GIAO" : dto.TrangThai.Trim().ToUpper(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy
            };
        }

        private static List<ChiTietGiaoChiTieu> BuildChildAssignments(
            ChiTietGiaoChiTieu parent,
            CreateChiTietGiaoChiTieuDto dto,
            List<DanhMucChiTieu> childCriteria,
            string? tanSuatBaoCao)
        {
            var payloadByCatalogId = dto.TieuChiCon.ToDictionary(x => x.DanhMucChiTieuId);

            return childCriteria.Select((criterion, index) =>
            {
                var payload = payloadByCatalogId[criterion.Id];
                return new ChiTietGiaoChiTieu
                {
                    DotGiaoChiTieuId = dto.DotGiaoChiTieuId,
                    DanhMucChiTieuId = criterion.Id,
                    DonViNhanId = dto.DonViNhanId,
                    DonViThucHienChinhId = dto.DonViThucHienChinhId,
                    GiaTriMucTieu = payload.GiaTriMucTieu,
                    GiaTriMucTieuText = NormalizeNullable(payload.GiaTriMucTieuText),
                    GiaTriDauKyCoDinh = ResolveGiaTriDauKyCoDinh(criterion.LoaiChiTieu, payload.GiaTriDauKyCoDinh),
                    ChiTietGiaoChaId = parent.Id,
                    GhiChu = NormalizeNullable(payload.GhiChu),
                    ThuTuHienThi = payload.ThuTuHienThi ?? criterion.ThuTuHienThi ?? index + 1,
                    TanSuatBaoCao = tanSuatBaoCao,
                    TrangThai = parent.TrangThai,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy
                };
            }).ToList();
        }

        private async Task SyncChildAssignmentsAsync(
            ChiTietGiaoChiTieu parent,
            UpdateChiTietGiaoChiTieuDto dto,
            List<DanhMucChiTieu> childCriteria,
            string? tanSuatBaoCao)
        {
            if (childCriteria.Count == 0)
            {
                if (parent.ChiTietGiaoChiTieuCons.Count > 0)
                {
                    var childIds = parent.ChiTietGiaoChiTieuCons.Select(x => x.Id).ToList();
                    var hasActivity = await HasRelatedMonitoringAsync(childIds);
                    if (hasActivity)
                    {
                        throw new Exception("Khong the bo cau hinh phan ra vi da co du lieu theo doi hoac danh gia.");
                    }

                    _context.ChiTietGiaoChiTieus.RemoveRange(parent.ChiTietGiaoChiTieuCons);
                }

                return;
            }

            var payloadByCatalogId = dto.TieuChiCon.ToDictionary(x => x.DanhMucChiTieuId);
            var existingChildrenByCatalogId = parent.ChiTietGiaoChiTieuCons.ToDictionary(x => x.DanhMucChiTieuId);
            var existingCatalogIds = existingChildrenByCatalogId.Keys.OrderBy(x => x).ToList();
            var targetCatalogIds = childCriteria.Select(x => x.Id).OrderBy(x => x).ToList();

            if (!existingCatalogIds.SequenceEqual(targetCatalogIds))
            {
                if (parent.ChiTietGiaoChiTieuCons.Count > 0)
                {
                    var childIds = parent.ChiTietGiaoChiTieuCons.Select(x => x.Id).ToList();
                    var hasActivity = await HasRelatedMonitoringAsync(childIds);
                    if (hasActivity)
                    {
                        throw new Exception("Khong the thay doi bo tieu chi con vi da co du lieu theo doi hoac danh gia.");
                    }

                    _context.ChiTietGiaoChiTieus.RemoveRange(parent.ChiTietGiaoChiTieuCons);
                }

                var recreatedChildren = childCriteria.Select((criterion, index) =>
                {
                    var payload = payloadByCatalogId[criterion.Id];
                    return new ChiTietGiaoChiTieu
                    {
                        DotGiaoChiTieuId = dto.DotGiaoChiTieuId,
                        DanhMucChiTieuId = criterion.Id,
                        DonViNhanId = dto.DonViNhanId,
                        DonViThucHienChinhId = dto.DonViThucHienChinhId,
                        GiaTriMucTieu = payload.GiaTriMucTieu,
                        GiaTriMucTieuText = NormalizeNullable(payload.GiaTriMucTieuText),
                        GiaTriDauKyCoDinh = ResolveGiaTriDauKyCoDinh(criterion.LoaiChiTieu, payload.GiaTriDauKyCoDinh),
                        ChiTietGiaoChaId = parent.Id,
                        GhiChu = NormalizeNullable(payload.GhiChu),
                        ThuTuHienThi = payload.ThuTuHienThi ?? criterion.ThuTuHienThi ?? index + 1,
                        TanSuatBaoCao = tanSuatBaoCao,
                        TrangThai = parent.TrangThai,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = parent.CreatedBy,
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = dto.UpdatedBy
                    };
                }).ToList();

                _context.ChiTietGiaoChiTieus.AddRange(recreatedChildren);
                return;
            }

            foreach (var criterion in childCriteria)
            {
                var existingChild = existingChildrenByCatalogId[criterion.Id];
                var payload = payloadByCatalogId[criterion.Id];

                existingChild.DotGiaoChiTieuId = dto.DotGiaoChiTieuId;
                existingChild.DonViNhanId = dto.DonViNhanId;
                existingChild.DonViThucHienChinhId = dto.DonViThucHienChinhId;
                existingChild.GiaTriMucTieu = payload.GiaTriMucTieu;
                existingChild.GiaTriMucTieuText = NormalizeNullable(payload.GiaTriMucTieuText);
                existingChild.GiaTriDauKyCoDinh = ResolveGiaTriDauKyCoDinh(criterion.LoaiChiTieu, payload.GiaTriDauKyCoDinh);
                existingChild.GhiChu = NormalizeNullable(payload.GhiChu);
                existingChild.ThuTuHienThi = payload.ThuTuHienThi ?? criterion.ThuTuHienThi ?? existingChild.ThuTuHienThi;
                existingChild.TanSuatBaoCao = tanSuatBaoCao;
                existingChild.TrangThai = parent.TrangThai;
                existingChild.UpdatedAt = DateTime.UtcNow;
                existingChild.UpdatedBy = dto.UpdatedBy;
            }
        }

        private async Task<bool> HasRelatedMonitoringAsync(List<long> chiTietIds)
        {
            if (chiTietIds.Count == 0)
            {
                return false;
            }

            var hasTheoDoi = await _context.TheoDoiThucHienKPIs.AnyAsync(x => chiTietIds.Contains(x.ChiTietGiaoChiTieuId));
            if (hasTheoDoi)
            {
                return true;
            }

            return await _context.DanhGiaKPIs.AnyAsync(x => chiTietIds.Contains(x.ChiTietGiaoChiTieuId));
        }

        private async Task RefreshMonitoringAndDanhGiaAsync(IEnumerable<long> chiTietIds, string? actor)
        {
            var normalizedActor = NormalizeActor(actor);

            foreach (var chiTietId in chiTietIds.Distinct())
            {
                await RecalculateTheoDoiChainAsync(chiTietId);
                await _danhGiaKPIService.SynchronizeDanhGiaForChiTietAsync(chiTietId, normalizedActor);
            }
        }

        private async Task RecalculateTheoDoiChainAsync(long chiTietGiaoChiTieuId)
        {
            var giaTriDauKyCoDinh = await _context.ChiTietGiaoChiTieus
                .AsNoTracking()
                .Where(x => x.Id == chiTietGiaoChiTieuId)
                .Select(x => x.GiaTriDauKyCoDinh)
                .FirstOrDefaultAsync() ?? 0;

            var records = await _context.TheoDoiThucHienKPIs
                .Include(x => x.KyBaoCaoKPI)
                .Where(x => x.ChiTietGiaoChiTieuId == chiTietGiaoChiTieuId)
                .ToListAsync();

            if (records.Count == 0)
            {
                return;
            }

            var ordered = records
                .OrderBy(x => x.KyBaoCaoKPI!.Nam)
                .ThenBy(x => ThuTuLoaiKy(x.KyBaoCaoKPI!.LoaiKy))
                .ThenBy(x => x.KyBaoCaoKPI!.SoKy ?? 0)
                .ThenBy(x => x.Id)
                .ToList();

            decimal luyKe = 0;

            foreach (var record in ordered)
            {
                luyKe += record.GiaTriThucHienTrongKy ?? 0;
                record.GiaTriDauKy = giaTriDauKyCoDinh;
                record.GiaTriCuoiKy = giaTriDauKyCoDinh + luyKe;
                record.GiaTriLuyKe = luyKe;
            }

            await _context.SaveChangesAsync();
        }

        private static string? NormalizeTanSuatBaoCao(string? tanSuatBaoCao)
        {
            if (string.IsNullOrWhiteSpace(tanSuatBaoCao))
            {
                return null;
            }

            var value = tanSuatBaoCao.Trim().ToUpper().Replace("_", string.Empty);

            if (!TanSuatBaoCaoHopLe.Contains(value))
            {
                throw new Exception("TanSuatBaoCao khong hop le. Chi chap nhan: THANG, QUY, 6THANG, NAM.");
            }

            return value;
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

        private static ChiTietGiaoChiTieuDto MapToDto(ChiTietGiaoChiTieu x, bool includeChildren = true)
        {
            var children = includeChildren
                ? (x.ChiTietGiaoChiTieuCons ?? Array.Empty<ChiTietGiaoChiTieu>())
                    .OrderBy(child => child.ThuTuHienThi)
                    .ThenBy(child => child.Id)
                    .Select(child => MapToDto(child, false))
                    .ToList()
                : new List<ChiTietGiaoChiTieuDto>();

            return new ChiTietGiaoChiTieuDto
            {
                Id = x.Id,
                DotGiaoChiTieuId = x.DotGiaoChiTieuId,
                TenDotGiaoChiTieu = x.DotGiaoChiTieu?.TenDotGiao,
                DanhMucChiTieuId = x.DanhMucChiTieuId,
                MaDanhMucChiTieu = x.DanhMucChiTieu?.MaChiTieu,
                TenDanhMucChiTieu = x.DanhMucChiTieu?.TenChiTieu,
                LoaiChiTieu = x.DanhMucChiTieu?.LoaiChiTieu,
                DonViTinh = x.DanhMucChiTieu?.DonViTinh,
                TanSuatBaoCao = x.TanSuatBaoCao,
                DonViNhanId = x.DonViNhanId,
                TenDonViNhan = x.DonViNhan?.TenDonVi,
                DonViThucHienChinhId = x.DonViThucHienChinhId,
                TenDonViThucHienChinh = x.DonViThucHienChinh?.TenDonVi,
                GiaTriMucTieu = x.GiaTriMucTieu,
                GiaTriMucTieuText = x.GiaTriMucTieuText,
                GiaTriDauKyCoDinh = x.GiaTriDauKyCoDinh,
                ChiTietGiaoChaId = x.ChiTietGiaoChaId,
                GhiChu = x.GhiChu,
                ThuTuHienThi = x.ThuTuHienThi,
                TrangThai = x.TrangThai,
                CoTieuChiCon = children.Count > 0,
                BatBuocDatTatCaTieuChiCon = x.DanhMucChiTieu?.BatBuocDatTatCaTieuChiCon ?? true,
                TieuChiCon = children,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy
            };
        }

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string NormalizeActor(string? actor)
        {
            return string.IsNullOrWhiteSpace(actor) ? "system" : actor.Trim();
        }

        private static void ValidateGiaTriDauKyCoDinh(string? loaiChiTieu, decimal? giaTriDauKyCoDinh, string label)
        {
            if (IsQualitative(loaiChiTieu))
            {
                return;
            }

            if (giaTriDauKyCoDinh.HasValue && giaTriDauKyCoDinh.Value < 0)
            {
                throw new Exception($"Gia tri dau ky co dinh cua {label} khong duoc nho hon 0.");
            }
        }

        private static decimal? ResolveGiaTriDauKyCoDinh(string? loaiChiTieu, decimal? giaTriDauKyCoDinh)
        {
            if (IsQualitative(loaiChiTieu))
            {
                return null;
            }

            return giaTriDauKyCoDinh ?? 0;
        }

        private static bool IsQualitative(string? loaiChiTieu)
        {
            return string.Equals(loaiChiTieu?.Trim(), "DINH_TINH", StringComparison.OrdinalIgnoreCase);
        }
    }
}
