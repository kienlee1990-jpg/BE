using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.NhomThiDua;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace KPITrackerAPI.Services
{
    public class NhomThiDuaService : INhomThiDuaService
    {
        private readonly ApplicationDbContext _context;

        public NhomThiDuaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NhomThiDuaDto>> GetAllAsync()
        {
            var items = await BuildQuery()
                .OrderBy(x => x.TenNhom)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<NhomThiDuaDto?> GetByIdAsync(long id)
        {
            var item = await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<NhomThiDuaDto> CreateAsync(CreateNhomThiDuaDto dto)
        {
            await ValidateRequestAsync(dto.TenNhom, dto.DonViIds, dto.DanhMucChiTieuIds, null);

            var maNhom = await GenerateUniqueCodeAsync(dto.TenNhom);
            var entity = new NhomThiDua
            {
                MaNhom = maNhom,
                TenNhom = dto.TenNhom.Trim(),
                MoTa = NormalizeNullable(dto.MoTa),
                TrangThai = NormalizeTrangThai(dto.TrangThai),
                CreatedAt = DateTime.UtcNow
            };

            entity.NhomThiDuaDonVis = dto.DonViIds
                .Distinct()
                .Select(x => new NhomThiDuaDonVi
                {
                    DonViId = x,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            entity.NhomThiDuaChiTieus = dto.DanhMucChiTieuIds
                .Distinct()
                .Select(x => new NhomThiDuaChiTieu
                {
                    DanhMucChiTieuId = x,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            _context.Set<NhomThiDua>().Add(entity);
            await _context.SaveChangesAsync();

            var created = await BuildQuery().FirstAsync(x => x.Id == entity.Id);
            return MapToDto(created);
        }

        public async Task<NhomThiDuaDto?> UpdateAsync(long id, UpdateNhomThiDuaDto dto)
        {
            var entity = await _context.Set<NhomThiDua>()
                .Include(x => x.NhomThiDuaDonVis)
                .Include(x => x.NhomThiDuaChiTieus)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            await ValidateRequestAsync(dto.TenNhom, dto.DonViIds, dto.DanhMucChiTieuIds, id);

            entity.TenNhom = dto.TenNhom.Trim();
            entity.MoTa = NormalizeNullable(dto.MoTa);
            entity.TrangThai = NormalizeTrangThai(dto.TrangThai);
            entity.UpdatedAt = DateTime.UtcNow;

            SyncDonVis(entity, dto.DonViIds.Distinct().ToList());
            SyncChiTieus(entity, dto.DanhMucChiTieuIds.Distinct().ToList());

            await _context.SaveChangesAsync();

            var updated = await BuildQuery().FirstAsync(x => x.Id == id);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.Set<NhomThiDua>()
                .Include(x => x.NhomThiDuaDonVis)
                .Include(x => x.NhomThiDuaChiTieus)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return false;
            }

            if (entity.NhomThiDuaDonVis.Count > 0)
            {
                _context.Set<NhomThiDuaDonVi>().RemoveRange(entity.NhomThiDuaDonVis);
            }

            if (entity.NhomThiDuaChiTieus.Count > 0)
            {
                _context.Set<NhomThiDuaChiTieu>().RemoveRange(entity.NhomThiDuaChiTieus);
            }

            _context.Set<NhomThiDua>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<NhomThiDua> BuildQuery()
        {
            return _context.Set<NhomThiDua>()
                .AsNoTracking()
                .Include(x => x.NhomThiDuaDonVis)
                    .ThenInclude(x => x.DonVi)
                .Include(x => x.NhomThiDuaChiTieus)
                    .ThenInclude(x => x.DanhMucChiTieu)
                        .ThenInclude(x => x!.ChiTieuCha);
        }

        private async Task ValidateRequestAsync(
            string tenNhom,
            List<long> donViIds,
            List<long> danhMucChiTieuIds,
            long? currentId)
        {
            var normalizedName = tenNhom.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new Exception("Tên nhóm thi đua không được để trống.");
            }

            var duplicatedName = await _context.Set<NhomThiDua>().AnyAsync(x =>
                x.TenNhom == normalizedName && (!currentId.HasValue || x.Id != currentId.Value));

            if (duplicatedName)
            {
                throw new Exception("Tên nhóm thi đua đã tồn tại.");
            }

            if (donViIds.Count == 0)
            {
                throw new Exception("Nhóm thi đua phải có ít nhất một đơn vị.");
            }

            if (danhMucChiTieuIds.Count == 0)
            {
                throw new Exception("Nhóm thi đua phải có ít nhất một chỉ tiêu chi tiết.");
            }

            var existingDonViIds = await _context.DonVis
                .Where(x => donViIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var invalidDonViIds = donViIds.Except(existingDonViIds).ToList();
            if (invalidDonViIds.Count > 0)
            {
                throw new Exception("Có đơn vị không tồn tại trong cấu hình nhóm thi đua.");
            }

            var chiTieuMap = await _context.DanhMucChiTieus
                .Where(x => danhMucChiTieuIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.ChiTieuChaId
                })
                .ToListAsync();

            var existingChiTieuIds = chiTieuMap.Select(x => x.Id).ToList();
            var invalidChiTieuIds = danhMucChiTieuIds.Except(existingChiTieuIds).ToList();
            if (invalidChiTieuIds.Count > 0)
            {
                throw new Exception("Có chỉ tiêu chi tiết không tồn tại trong cấu hình nhóm thi đua.");
            }

            var parentIds = await _context.DanhMucChiTieus
                .Where(x => x.ChiTieuChaId.HasValue && danhMucChiTieuIds.Contains(x.ChiTieuChaId.Value))
                .Select(x => x.ChiTieuChaId!.Value)
                .Distinct()
                .ToListAsync();

            if (parentIds.Count > 0)
            {
                throw new Exception("Chỉ được chọn các chỉ tiêu chi tiết, không chọn chỉ tiêu cha có phân rã.");
            }
        }

        private void SyncDonVis(NhomThiDua entity, List<long> donViIds)
        {
            var currentIds = entity.NhomThiDuaDonVis.Select(x => x.DonViId).ToHashSet();

            var toRemove = entity.NhomThiDuaDonVis.Where(x => !donViIds.Contains(x.DonViId)).ToList();
            if (toRemove.Count > 0)
            {
                _context.Set<NhomThiDuaDonVi>().RemoveRange(toRemove);
            }

            var toAdd = donViIds.Where(x => !currentIds.Contains(x)).ToList();
            foreach (var donViId in toAdd)
            {
                entity.NhomThiDuaDonVis.Add(new NhomThiDuaDonVi
                {
                    NhomThiDuaId = entity.Id,
                    DonViId = donViId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private void SyncChiTieus(NhomThiDua entity, List<long> danhMucChiTieuIds)
        {
            var currentIds = entity.NhomThiDuaChiTieus.Select(x => x.DanhMucChiTieuId).ToHashSet();

            var toRemove = entity.NhomThiDuaChiTieus.Where(x => !danhMucChiTieuIds.Contains(x.DanhMucChiTieuId)).ToList();
            if (toRemove.Count > 0)
            {
                _context.Set<NhomThiDuaChiTieu>().RemoveRange(toRemove);
            }

            var toAdd = danhMucChiTieuIds.Where(x => !currentIds.Contains(x)).ToList();
            foreach (var danhMucChiTieuId in toAdd)
            {
                entity.NhomThiDuaChiTieus.Add(new NhomThiDuaChiTieu
                {
                    NhomThiDuaId = entity.Id,
                    DanhMucChiTieuId = danhMucChiTieuId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private async Task<string> GenerateUniqueCodeAsync(string tenNhom)
        {
            var slug = Slugify(tenNhom);
            var candidate = string.IsNullOrWhiteSpace(slug) ? "NHOM-THI-DUA" : slug;
            var current = candidate;
            var index = 1;

            while (await _context.Set<NhomThiDua>().AnyAsync(x => x.MaNhom == current))
            {
                current = $"{candidate}-{index}";
                index++;
            }

            return current;
        }

        private static string Slugify(string value)
        {
            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var ch in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory == System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(ch))
                {
                    builder.Append(char.ToUpperInvariant(ch == 'đ' ? 'd' : ch == 'Đ' ? 'D' : ch));
                }
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_')
                {
                    builder.Append('-');
                }
            }

            return builder.ToString().Trim('-');
        }

        private static string NormalizeTrangThai(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "HOAT_DONG" : value.Trim().ToUpper();
        }

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static NhomThiDuaDto MapToDto(NhomThiDua entity)
        {
            return new NhomThiDuaDto
            {
                Id = entity.Id,
                MaNhom = entity.MaNhom,
                TenNhom = entity.TenNhom,
                MoTa = entity.MoTa,
                TrangThai = entity.TrangThai,
                DonVis = entity.NhomThiDuaDonVis
                    .Where(x => x.DonVi != null)
                    .OrderBy(x => x.DonVi!.MaDonVi)
                    .Select(x => new NhomThiDuaDonViDto
                    {
                        DonViId = x.DonViId,
                        MaDonVi = x.DonVi!.MaDonVi,
                        TenDonVi = x.DonVi.TenDonVi,
                        LoaiDonVi = x.DonVi.LoaiDonVi
                    })
                    .ToList(),
                ChiTieus = entity.NhomThiDuaChiTieus
                    .Where(x => x.DanhMucChiTieu != null)
                    .OrderBy(x => x.DanhMucChiTieu!.MaChiTieu)
                    .Select(x => new NhomThiDuaChiTieuDto
                    {
                        DanhMucChiTieuId = x.DanhMucChiTieuId,
                        MaChiTieu = x.DanhMucChiTieu!.MaChiTieu,
                        TenChiTieu = x.DanhMucChiTieu.TenChiTieu,
                        ChiTieuChaId = x.DanhMucChiTieu.ChiTieuChaId,
                        TenChiTieuCha = x.DanhMucChiTieu.ChiTieuCha?.TenChiTieu
                    })
                    .ToList(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
