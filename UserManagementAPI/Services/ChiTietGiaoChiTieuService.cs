using UserManagementAPI.Data;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Models.DTOs.ChiTietGiaoChiTieu;
using KPI_Tracker_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_Tracker_API.Services
{
    public class ChiTietGiaoChiTieuService : IChiTietGiaoChiTieuService
    {
        private readonly ApplicationDbContext _context;

        private static readonly HashSet<string> TanSuatBaoCaoHopLe = new()
        {
            "THANG",
            "QUY",
            "6THANG",
            "NAM"
        };

        public ChiTietGiaoChiTieuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetAllAsync()
        {
            var items = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<ChiTietGiaoChiTieuDto?> GetByIdAsync(long id)
        {
            var item = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null) return null;

            return MapToDto(item);
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetByDotGiaoChiTieuIdAsync(long dotGiaoChiTieuId)
        {
            var items = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .Where(x => x.DotGiaoChiTieuId == dotGiaoChiTieuId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetByDonViNhanIdAsync(long donViNhanId)
        {
            var items = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .Where(x => x.DonViNhanId == donViNhanId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<List<ChiTietGiaoChiTieuDto>> GetChildrenAsync(long chiTietGiaoChaId)
        {
            var items = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .Where(x => x.ChiTietGiaoChaId == chiTietGiaoChaId)
                .OrderBy(x => x.ThuTuHienThi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<ChiTietGiaoChiTieuDto> CreateAsync(CreateChiTietGiaoChiTieuDto dto)
        {
            await ValidateForeignKeys(
                dto.DotGiaoChiTieuId,
                dto.DanhMucChiTieuId,
                dto.DonViNhanId,
                dto.DonViThucHienChinhId,
                dto.ChiTietGiaoChaId);

            var tanSuatBaoCao = NormalizeTanSuatBaoCao(dto.TanSuatBaoCao);

            var entity = new ChiTietGiaoChiTieu
            {
                DotGiaoChiTieuId = dto.DotGiaoChiTieuId,
                DanhMucChiTieuId = dto.DanhMucChiTieuId,
                DonViNhanId = dto.DonViNhanId,
                DonViThucHienChinhId = dto.DonViThucHienChinhId,
                GiaTriMucTieu = dto.GiaTriMucTieu,
                GiaTriMucTieuText = dto.GiaTriMucTieuText,
                ChiTietGiaoChaId = dto.ChiTietGiaoChaId,
                GhiChu = dto.GhiChu,
                ThuTuHienThi = dto.ThuTuHienThi,
                TanSuatBaoCao = tanSuatBaoCao,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "DA_GIAO" : dto.TrangThai.Trim().ToUpper(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy
            };

            _context.ChiTietGiaoChiTieus.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(created);
        }

        public async Task<ChiTietGiaoChiTieuDto?> UpdateAsync(long id, UpdateChiTietGiaoChiTieuDto dto)
        {
            var entity = await _context.ChiTietGiaoChiTieus.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            await ValidateForeignKeys(
                dto.DotGiaoChiTieuId,
                dto.DanhMucChiTieuId,
                dto.DonViNhanId,
                dto.DonViThucHienChinhId,
                dto.ChiTietGiaoChaId,
                id);

            var tanSuatBaoCao = NormalizeTanSuatBaoCao(dto.TanSuatBaoCao);

            entity.DotGiaoChiTieuId = dto.DotGiaoChiTieuId;
            entity.DanhMucChiTieuId = dto.DanhMucChiTieuId;
            entity.DonViNhanId = dto.DonViNhanId;
            entity.DonViThucHienChinhId = dto.DonViThucHienChinhId;
            entity.GiaTriMucTieu = dto.GiaTriMucTieu;
            entity.GiaTriMucTieuText = dto.GiaTriMucTieuText;
            entity.ChiTietGiaoChaId = dto.ChiTietGiaoChaId;
            entity.GhiChu = dto.GhiChu;
            entity.ThuTuHienThi = dto.ThuTuHienThi;
            entity.TanSuatBaoCao = tanSuatBaoCao;
            entity.TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? entity.TrangThai : dto.TrangThai.Trim().ToUpper();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = dto.UpdatedBy;

            await _context.SaveChangesAsync();

            var updated = await _context.ChiTietGiaoChiTieus
                .Include(x => x.DotGiaoChiTieu)
                .Include(x => x.DanhMucChiTieu)
                .Include(x => x.DonViNhan)
                .Include(x => x.DonViThucHienChinh)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.ChiTietGiaoChiTieus
                .Include(x => x.ChiTietGiaoChiTieuCons)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            if (entity.ChiTietGiaoChiTieuCons.Any())
            {
                throw new Exception("Không thể xóa vì bản ghi này đang có chi tiết giao con.");
            }

            _context.ChiTietGiaoChiTieus.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
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
                throw new Exception("DotGiaoChiTieuId không tồn tại.");

            var danhMucExists = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == danhMucChiTieuId);
            if (!danhMucExists)
                throw new Exception("DanhMucChiTieuId không tồn tại.");

            var donViNhanExists = await _context.DonVis.AnyAsync(x => x.Id == donViNhanId);
            if (!donViNhanExists)
                throw new Exception("DonViNhanId không tồn tại.");

            if (donViThucHienChinhId.HasValue)
            {
                var donViThucHienExists = await _context.DonVis.AnyAsync(x => x.Id == donViThucHienChinhId.Value);
                if (!donViThucHienExists)
                    throw new Exception("DonViThucHienChinhId không tồn tại.");
            }

            if (chiTietGiaoChaId.HasValue)
            {
                if (currentId.HasValue && chiTietGiaoChaId.Value == currentId.Value)
                    throw new Exception("ChiTietGiaoChaId không được trỏ tới chính nó.");

                var chaExists = await _context.ChiTietGiaoChiTieus.AnyAsync(x => x.Id == chiTietGiaoChaId.Value);
                if (!chaExists)
                    throw new Exception("ChiTietGiaoChaId không tồn tại.");
            }
        }

        private static string? NormalizeTanSuatBaoCao(string? tanSuatBaoCao)
        {
            if (string.IsNullOrWhiteSpace(tanSuatBaoCao))
                return null;

            var value = tanSuatBaoCao.Trim().ToUpper();

            if (!TanSuatBaoCaoHopLe.Contains(value))
                throw new Exception("TanSuatBaoCao không hợp lệ. Chỉ chấp nhận: THANG, QUY, 6THANG, NAM.");

            return value;
        }

        private static ChiTietGiaoChiTieuDto MapToDto(ChiTietGiaoChiTieu x)
        {
            return new ChiTietGiaoChiTieuDto
            {
                Id = x.Id,
                DotGiaoChiTieuId = x.DotGiaoChiTieuId,
                TenDotGiaoChiTieu = x.DotGiaoChiTieu?.TenDotGiao,

                DanhMucChiTieuId = x.DanhMucChiTieuId,
                TenDanhMucChiTieu = x.DanhMucChiTieu?.TenChiTieu,

                DonViNhanId = x.DonViNhanId,
                TenDonViNhan = x.DonViNhan?.TenDonVi,

                DonViThucHienChinhId = x.DonViThucHienChinhId,
                TenDonViThucHienChinh = x.DonViThucHienChinh?.TenDonVi,

                GiaTriMucTieu = x.GiaTriMucTieu,
                GiaTriMucTieuText = x.GiaTriMucTieuText,
                ChiTietGiaoChaId = x.ChiTietGiaoChaId,

                GhiChu = x.GhiChu,
                ThuTuHienThi = x.ThuTuHienThi,
                TanSuatBaoCao = x.TanSuatBaoCao,
                TrangThai = x.TrangThai,

                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy
            };
        }
    }
}