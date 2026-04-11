using UserManagementAPI.Data;
using KPI_Tracker_API.Entities;
using KPI_Tracker_API.Interfaces;
using KPI_Tracker_API.Models.DTOs.DonVi;
using Microsoft.EntityFrameworkCore;
using System;
using UserManagementAPI.Data;

namespace KPI_Tracker_API.Services
{
    public class DonViService : IDonViService
    {
        private readonly ApplicationDbContext _context;

        public DonViService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonViDto>> GetAllAsync()
        {
            var items = await _context.DonVis
                .Include(x => x.DonViCha)
                .OrderBy(x => x.MaDonVi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<DonViDto?> GetByIdAsync(long id)
        {
            var item = await _context.DonVis
                .Include(x => x.DonViCha)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<DonViDto>> GetChildrenAsync(long donViChaId)
        {
            var items = await _context.DonVis
                .Include(x => x.DonViCha)
                .Where(x => x.DonViChaId == donViChaId)
                .OrderBy(x => x.MaDonVi)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<DonViDto> CreateAsync(CreateDonViDto dto)
        {
            await ValidateBeforeSave(dto.MaDonVi, dto.DonViChaId, null);

            var entity = new DonVi
            {
                MaDonVi = dto.MaDonVi.Trim(),
                TenDonVi = dto.TenDonVi.Trim(),
                LoaiDonVi = dto.LoaiDonVi.Trim(),
                DonViChaId = dto.DonViChaId,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "HOAT_DONG" : dto.TrangThai.Trim(),
                DiaChi = dto.DiaChi,
                NguoiDaiDien = dto.NguoiDaiDien,
                SoDienThoai = dto.SoDienThoai,
                Email = dto.Email,
                GhiChu = dto.GhiChu,
                CreatedAt = DateTime.UtcNow
            };

            _context.DonVis.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.DonVis
                .Include(x => x.DonViCha)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(created);
        }

        public async Task<DonViDto?> UpdateAsync(long id, UpdateDonViDto dto)
        {
            var entity = await _context.DonVis.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            await ValidateBeforeSave(dto.MaDonVi, dto.DonViChaId, id);

            entity.MaDonVi = dto.MaDonVi.Trim();
            entity.TenDonVi = dto.TenDonVi.Trim();
            entity.LoaiDonVi = dto.LoaiDonVi.Trim();
            entity.DonViChaId = dto.DonViChaId;
            entity.TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? entity.TrangThai : dto.TrangThai.Trim();
            entity.DiaChi = dto.DiaChi;
            entity.NguoiDaiDien = dto.NguoiDaiDien;
            entity.SoDienThoai = dto.SoDienThoai;
            entity.Email = dto.Email;
            entity.GhiChu = dto.GhiChu;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updated = await _context.DonVis
                .Include(x => x.DonViCha)
                .FirstAsync(x => x.Id == entity.Id);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DonVis.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            var hasChildren = await _context.DonVis.AnyAsync(x => x.DonViChaId == id);
            if (hasChildren)
                throw new Exception("Không thể xóa vì đơn vị này đang có đơn vị con.");

            var isUsedInChiTietGiao = await _context.ChiTietGiaoChiTieus.AnyAsync(x =>
                x.DonViNhanId == id || x.DonViThucHienChinhId == id);

            if (isUsedInChiTietGiao)
                throw new Exception("Không thể xóa vì đơn vị này đang được sử dụng trong chi tiết giao chỉ tiêu.");

            _context.DonVis.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task ValidateBeforeSave(string maDonVi, long? donViChaId, long? currentId)
        {
            var maDonViTrim = maDonVi.Trim();

            var existedMaDonVi = await _context.DonVis.AnyAsync(x =>
                x.MaDonVi == maDonViTrim && (!currentId.HasValue || x.Id != currentId.Value));

            if (existedMaDonVi)
                throw new Exception("Mã đơn vị đã tồn tại.");

            if (donViChaId.HasValue)
            {
                if (currentId.HasValue && donViChaId.Value == currentId.Value)
                    throw new Exception("Đơn vị cha không được trỏ tới chính nó.");

                var parentExists = await _context.DonVis.AnyAsync(x => x.Id == donViChaId.Value);
                if (!parentExists)
                    throw new Exception("Đơn vị cha không tồn tại.");
            }
        }

        private static DonViDto MapToDto(DonVi x)
        {
            return new DonViDto
            {
                Id = x.Id,
                MaDonVi = x.MaDonVi,
                TenDonVi = x.TenDonVi,
                LoaiDonVi = x.LoaiDonVi,
                DonViChaId = x.DonViChaId,
                TenDonViCha = x.DonViCha != null ? x.DonViCha.TenDonVi : null,
                TrangThai = x.TrangThai,
                DiaChi = x.DiaChi,
                NguoiDaiDien = x.NguoiDaiDien,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,
                GhiChu = x.GhiChu,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }
    }
}