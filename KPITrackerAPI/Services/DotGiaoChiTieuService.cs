using KPITrackerAPI.DTOs;
using KPITrackerAPI.DTOs.DotGiaoChiTieu;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using KPITrackerAPI.Data;

namespace KPITrackerAPI.Services
{
    public class DotGiaoChiTieuService : IDotGiaoChiTieuService
    {
        private readonly ApplicationDbContext _context;

        public DotGiaoChiTieuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DotGiaoChiTieuResponseDto> CreateAsync(CreateDotGiaoChiTieuDto dto)
        {
            var exists = await _context.DotGiaoChiTieus
                .AnyAsync(x => x.MaDotGiao == dto.MaDotGiao);

            if (exists)
                throw new Exception("Mã d?t giao dã t?n t?i.");

            ValidateBusinessRules(dto.NamApDung, dto.NguonDotGiao, dto.CapGiao, dto.NgayBatDau, dto.NgayKetThuc);

            var entity = new DotGiaoChiTieu
            {
                MaDotGiao = dto.MaDotGiao.Trim(),
                TenDotGiao = dto.TenDotGiao.Trim(),
                NamApDung = dto.NamApDung,
                NguonDotGiao = dto.NguonDotGiao.Trim(),
                CapGiao = dto.CapGiao.Trim(),
                DonViGiaoId = dto.DonViGiaoId,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                GhiChu = dto.GhiChu,
                TrangThai = "DRAFT",
                CreatedAt = DateTime.UtcNow
            };

            _context.DotGiaoChiTieus.Add(entity);
            await _context.SaveChangesAsync();

            return MapToResponse(entity);
        }

        public async Task<List<DotGiaoChiTieuResponseDto>> GetAllAsync(
            string? keyword,
            int? namApDung,
            string? nguonDotGiao,
            string? capGiao,
            string? trangThai)
        {
            var query = _context.DotGiaoChiTieus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.MaDotGiao.ToLower().Contains(k) ||
                    x.TenDotGiao.ToLower().Contains(k));
            }

            if (namApDung.HasValue)
                query = query.Where(x => x.NamApDung == namApDung.Value);

            if (!string.IsNullOrWhiteSpace(nguonDotGiao))
                query = query.Where(x => x.NguonDotGiao == nguonDotGiao);

            if (!string.IsNullOrWhiteSpace(capGiao))
                query = query.Where(x => x.CapGiao == capGiao);

            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(x => x.TrangThai == trangThai);

            var data = await query
                .OrderByDescending(x => x.NamApDung)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();

            return data.Select(MapToResponse).ToList();
        }

        public async Task<DotGiaoChiTieuResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _context.DotGiaoChiTieus.FindAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<DotGiaoChiTieuResponseDto?> UpdateAsync(long id, UpdateDotGiaoChiTieuDto dto)
        {
            var entity = await _context.DotGiaoChiTieus.FindAsync(id);
            if (entity == null) return null;

            ValidateBusinessRules(dto.NamApDung, dto.NguonDotGiao, dto.CapGiao, dto.NgayBatDau, dto.NgayKetThuc);

            entity.TenDotGiao = dto.TenDotGiao.Trim();
            entity.NamApDung = dto.NamApDung;
            entity.NguonDotGiao = dto.NguonDotGiao.Trim();
            entity.CapGiao = dto.CapGiao.Trim();
            entity.DonViGiaoId = dto.DonViGiaoId;
            entity.NgayBatDau = dto.NgayBatDau;
            entity.NgayKetThuc = dto.NgayKetThuc;
            entity.TrangThai = dto.TrangThai.Trim();
            entity.GhiChu = dto.GhiChu;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DotGiaoChiTieus.FindAsync(id);
            if (entity == null) return false;

            var hasChildren = await _context.ChiTietGiaoChiTieus
                .AnyAsync(x => x.DotGiaoChiTieuId == id);

            if (hasChildren)
                throw new Exception("Ð?t giao dã có chi ti?t giao ch? tiêu, không th? xóa.");

            _context.DotGiaoChiTieus.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DotGiaoChiTieuResponseDto MapToResponse(DotGiaoChiTieu entity)
        {
            return new DotGiaoChiTieuResponseDto
            {
                Id = entity.Id,
                MaDotGiao = entity.MaDotGiao,
                TenDotGiao = entity.TenDotGiao,
                NamApDung = entity.NamApDung,
                NguonDotGiao = entity.NguonDotGiao,
                CapGiao = entity.CapGiao,
                DonViGiaoId = entity.DonViGiaoId,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TrangThai = entity.TrangThai,
                GhiChu = entity.GhiChu,
                CreatedAt = entity.CreatedAt
            };
        }

        private static void ValidateBusinessRules(
            int namApDung,
            string nguonDotGiao,
            string capGiao,
            DateTime ngayBatDau,
            DateTime? ngayKetThuc)
        {
            if (namApDung < 2000 || namApDung > 2100)
                throw new Exception("Nam áp d?ng không h?p l?.");

            var nguonHopLe = new[] { "BO_GIAO", "THANH_PHO_GIAO" };
            if (!nguonHopLe.Contains(nguonDotGiao))
                throw new Exception("Ngu?n d?t giao không h?p l?.");

            var capHopLe = new[] { "BO", "THANH_PHO" };
            if (!capHopLe.Contains(capGiao))
                throw new Exception("C?p giao không h?p l?.");

            if (nguonDotGiao == "BO_GIAO" && capGiao != "BO")
                throw new Exception("Ngu?n B? giao ph?i di kèm c?p giao là BO.");

            if (nguonDotGiao == "THANH_PHO_GIAO" && capGiao != "THANH_PHO")
                throw new Exception("Ngu?n Thành ph? giao ph?i di kèm c?p giao là THANH_PHO.");

            if (ngayKetThuc.HasValue && ngayKetThuc.Value.Date < ngayBatDau.Date)
                throw new Exception("Ngay ket thuc phai lon hon hoac bang ngay bat dau.");
        }
    }
}


