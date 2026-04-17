using KPITrackerAPI.Constants;
using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.CauHinhNguongDanhGiaKPI;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Responses;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class CauHinhNguongDanhGiaKPIService : ICauHinhNguongDanhGiaKPIService
    {
        private readonly ApplicationDbContext _context;

        public CauHinhNguongDanhGiaKPIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CauHinhNguongDanhGiaKPIResponse>> GetAllAsync(CauHinhNguongDanhGiaKPIQueryDto query)
        {
            var q = _context.CauHinhNguongDanhGiaKPIs
                .Include(x => x.DanhMucChiTieu)
                .AsQueryable();

            if (query.DanhMucChiTieuId.HasValue)
            {
                q = q.Where(x => x.DanhMucChiTieuId == query.DanhMucChiTieuId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.TieuChiDanhGia))
            {
                var normalizedTieuChiDanhGia = DanhGiaKPIConstants.NormalizeCode(query.TieuChiDanhGia);
                q = q.Where(x => x.TieuChiDanhGia == normalizedTieuChiDanhGia);
            }

            if (!string.IsNullOrWhiteSpace(query.QuyTacDanhGia))
            {
                var normalizedQuyTacDanhGia = DanhGiaKPIConstants.NormalizeCode(query.QuyTacDanhGia);
                q = q.Where(x => x.QuyTacDanhGia == normalizedQuyTacDanhGia);
            }

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();

                q = q.Where(x =>
                    x.XepLoai.Contains(keyword) ||
                    x.DieuKienThoiHan.Contains(keyword) ||
                    (x.TieuChiDanhGia != null && x.TieuChiDanhGia.Contains(keyword)) ||
                    (x.DanhMucChiTieu != null &&
                     (x.DanhMucChiTieu.MaChiTieu.Contains(keyword) ||
                      x.DanhMucChiTieu.TenChiTieu.Contains(keyword))) ||
                    (x.GhiChu != null && x.GhiChu.Contains(keyword)));
            }

            return await q
                .OrderBy(x => x.DanhMucChiTieuId.HasValue ? 0 : 1)
                .ThenBy(x => x.TieuChiDanhGia)
                .ThenBy(x => x.QuyTacDanhGia)
                .ThenBy(x => x.DieuKienThoiHan)
                .ThenBy(x => x.TuTyLe)
                .Select(x => new CauHinhNguongDanhGiaKPIResponse
                {
                    Id = x.Id,
                    DanhMucChiTieuId = x.DanhMucChiTieuId,
                    TenChiTieu = x.DanhMucChiTieu != null ? x.DanhMucChiTieu.TenChiTieu : null,
                    TieuChiDanhGia = x.TieuChiDanhGia,
                    QuyTacDanhGia = x.QuyTacDanhGia,
                    TuTyLe = x.TuTyLe,
                    DenTyLe = x.DenTyLe,
                    XepLoai = x.XepLoai,
                    DieuKienThoiHan = x.DieuKienThoiHan,
                    Diem = x.Diem,
                    GhiChu = x.GhiChu
                })
                .ToListAsync();
        }

        public async Task<CauHinhNguongDanhGiaKPIResponse?> GetByIdAsync(long id)
        {
            return await _context.CauHinhNguongDanhGiaKPIs
                .Include(x => x.DanhMucChiTieu)
                .Where(x => x.Id == id)
                .Select(x => new CauHinhNguongDanhGiaKPIResponse
                {
                    Id = x.Id,
                    DanhMucChiTieuId = x.DanhMucChiTieuId,
                    TenChiTieu = x.DanhMucChiTieu != null ? x.DanhMucChiTieu.TenChiTieu : null,
                    TieuChiDanhGia = x.TieuChiDanhGia,
                    QuyTacDanhGia = x.QuyTacDanhGia,
                    TuTyLe = x.TuTyLe,
                    DenTyLe = x.DenTyLe,
                    XepLoai = x.XepLoai,
                    DieuKienThoiHan = x.DieuKienThoiHan,
                    Diem = x.Diem,
                    GhiChu = x.GhiChu
                })
                .FirstOrDefaultAsync();
        }

        public async Task<long> CreateAsync(CreateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            var (normalizedXepLoai, normalizedTieuChiDanhGia, normalizedQuyTacDanhGia) = await ValidateAndNormalizeAsync(dto);

            var entity = new CauHinhNguongDanhGiaKPI
            {
                DanhMucChiTieuId = dto.DanhMucChiTieuId,
                TieuChiDanhGia = normalizedTieuChiDanhGia,
                QuyTacDanhGia = normalizedQuyTacDanhGia,
                TuTyLe = dto.TuTyLe,
                DenTyLe = dto.DenTyLe,
                XepLoai = normalizedXepLoai,
                DieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan),
                Diem = dto.Diem,
                GhiChu = NormalizeNullable(dto.GhiChu),
                CreatedAt = DateTime.UtcNow
            };

            _context.CauHinhNguongDanhGiaKPIs.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(long id, UpdateCauHinhNguongDanhGiaKPIDto dto, string? username)
        {
            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            var (normalizedXepLoai, normalizedTieuChiDanhGia, normalizedQuyTacDanhGia) = await ValidateAndNormalizeAsync(dto, id);

            entity.DanhMucChiTieuId = dto.DanhMucChiTieuId;
            entity.TieuChiDanhGia = normalizedTieuChiDanhGia;
            entity.QuyTacDanhGia = normalizedQuyTacDanhGia;
            entity.TuTyLe = dto.TuTyLe;
            entity.DenTyLe = dto.DenTyLe;
            entity.XepLoai = normalizedXepLoai;
            entity.DieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan);
            entity.Diem = dto.Diem;
            entity.GhiChu = NormalizeNullable(dto.GhiChu);
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.CauHinhNguongDanhGiaKPIs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            _context.CauHinhNguongDanhGiaKPIs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<(string xepLoai, string tieuChiDanhGia, string quyTacDanhGia)> ValidateAndNormalizeAsync(CreateCauHinhNguongDanhGiaKPIDto dto, long? currentId = null)
        {
            if (dto == null)
            {
                throw new Exception("Dữ liệu gửi lên không hợp lệ.");
            }

            var normalizedXepLoai = DanhGiaKPIConstants.NormalizeCode(dto.XepLoai);
            var normalizedDieuKienThoiHan = DanhGiaKPIConstants.NormalizeCode(dto.DieuKienThoiHan);
            var normalizedTieuChiDanhGia = DanhGiaKPIConstants.NormalizeCode(dto.TieuChiDanhGia);
            var normalizedQuyTacDanhGia = DanhGiaKPIConstants.NormalizeCode(dto.QuyTacDanhGia);

            if (string.IsNullOrWhiteSpace(normalizedTieuChiDanhGia))
            {
                throw new Exception("TieuChiDanhGia khong duoc de trong.");
            }

            if (!DanhGiaKPIConstants.AllowedTieuChiDanhGia.Contains(normalizedTieuChiDanhGia))
            {
                throw new Exception("TieuChiDanhGia khong hop le.");
            }

            if (normalizedTieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh)
            {
                normalizedQuyTacDanhGia = DanhGiaKPIConstants.QuyTacDanhGia.MacDinh;
            }
            else
            {
                normalizedQuyTacDanhGia = string.IsNullOrWhiteSpace(normalizedQuyTacDanhGia)
                    ? DanhGiaKPIConstants.QuyTacDanhGia.DatToiThieu
                    : normalizedQuyTacDanhGia;
            }

            if (string.IsNullOrWhiteSpace(normalizedQuyTacDanhGia) ||
                !DanhGiaKPIConstants.AllowedQuyTacDanhGia.Contains(normalizedQuyTacDanhGia))
            {
                throw new Exception("QuyTacDanhGia khong hop le.");
            }

            if (string.IsNullOrWhiteSpace(normalizedXepLoai))
            {
                throw new Exception("Xếp loại không được để trống.");
            }

            if (!DanhGiaKPIConstants.AllowedXepLoai.Contains(normalizedXepLoai))
            {
                throw new Exception("Xếp loại không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(normalizedDieuKienThoiHan))
            {
                throw new Exception("Điều kiện thời hạn không được để trống.");
            }

            if (!DanhGiaKPIConstants.AllowedDieuKienThoiHan.Contains(normalizedDieuKienThoiHan))
            {
                throw new Exception("Điều kiện thời hạn không hợp lệ.");
            }

            if (dto.TuTyLe > dto.DenTyLe)
            {
                throw new Exception("Tỷ lệ bắt đầu không được lớn hơn tỷ lệ kết thúc.");
            }

            if (dto.DanhMucChiTieuId.HasValue)
            {
                var existsDanhMuc = await _context.DanhMucChiTieus.AnyAsync(x => x.Id == dto.DanhMucChiTieuId.Value);
                if (!existsDanhMuc)
                {
                    throw new Exception("Danh mục chỉ tiêu không tồn tại.");
                }
            }

            ValidateBusinessRules(
                normalizedXepLoai,
                normalizedDieuKienThoiHan,
                normalizedTieuChiDanhGia,
                normalizedQuyTacDanhGia,
                dto.TuTyLe,
                dto.DenTyLe);

            var isDuplicate = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                (!currentId.HasValue || x.Id != currentId.Value) &&
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.TieuChiDanhGia == normalizedTieuChiDanhGia &&
                x.QuyTacDanhGia == normalizedQuyTacDanhGia &&
                x.TuTyLe == dto.TuTyLe &&
                x.DenTyLe == dto.DenTyLe &&
                x.XepLoai == normalizedXepLoai &&
                x.DieuKienThoiHan == normalizedDieuKienThoiHan);

            if (isDuplicate)
            {
                throw new Exception("Cấu hình ngưỡng đánh giá đã tồn tại.");
            }

            var hasOverlap = await _context.CauHinhNguongDanhGiaKPIs.AnyAsync(x =>
                (!currentId.HasValue || x.Id != currentId.Value) &&
                x.DanhMucChiTieuId == dto.DanhMucChiTieuId &&
                x.TieuChiDanhGia == normalizedTieuChiDanhGia &&
                x.QuyTacDanhGia == normalizedQuyTacDanhGia &&
                x.DieuKienThoiHan == normalizedDieuKienThoiHan &&
                x.TuTyLe <= dto.DenTyLe &&
                x.DenTyLe >= dto.TuTyLe);

            if (hasOverlap)
            {
                throw new Exception("Khoảng tỷ lệ đang bị chồng lấn với cấu hình ngưỡng khác cùng điều kiện thời hạn.");
            }

            return (normalizedXepLoai, normalizedTieuChiDanhGia, normalizedQuyTacDanhGia);
        }

        private static void ValidateBusinessRules(
            string xepLoai,
            string dieuKienThoiHan,
            string tieuChiDanhGia,
            string quyTacDanhGia,
            decimal tuTyLe,
            decimal denTyLe)
        {
            if (xepLoai == DanhGiaKPIConstants.XepLoai.KhongHoanThanh &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan)
            {
                throw new Exception("Khong hoan thanh chi ap dung khi da den han.");
            }

            if (xepLoai == DanhGiaKPIConstants.XepLoai.ChuaHoanThanh &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan)
            {
                throw new Exception("Chua hoan thanh chi ap dung khi chua den han.");
            }

            if ((xepLoai == DanhGiaKPIConstants.XepLoai.HoanThanh ||
                 xepLoai == DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc) &&
                dieuKienThoiHan != DanhGiaKPIConstants.DieuKienThoiHan.MacDinh)
            {
                throw new Exception("Hoan thanh va hoan thanh vuot muc chi dung voi dieu kien thoi han mac dinh.");
            }

            if (tieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh ||
                quyTacDanhGia == DanhGiaKPIConstants.QuyTacDanhGia.KhongVuotNguong)
            {
                if (xepLoai == DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc)
                {
                    throw new Exception("Quy tac danh gia hien tai khong ap dung muc hoan thanh vuot muc.");
                }
            }

            if (tieuChiDanhGia == DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh)
            {
                if (tuTyLe < 0 || denTyLe > 100)
                {
                    throw new Exception("Nguong danh gia cho chi tieu dinh tinh chi duoc nam trong khoang 0 den 100 phan tram.");
                }

                if (quyTacDanhGia != DanhGiaKPIConstants.QuyTacDanhGia.MacDinh)
                {
                    throw new Exception("Chi tieu dinh tinh chi su dung quy tac danh gia mac dinh.");
                }
            }

            if (quyTacDanhGia == DanhGiaKPIConstants.QuyTacDanhGia.KhongVuotNguong && denTyLe > 100)
            {
                throw new Exception("Quy tac khong vuot nguong chi cho phep cau hinh den 100 phan tram.");
            }
        }

        private static string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}

