using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.NhomThiDua
{
    public class CreateNhomThiDuaDto
    {
        [Required]
        [MaxLength(255)]
        public string TenNhom { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [MaxLength(30)]
        public string? TrangThai { get; set; }

        public List<long> DonViIds { get; set; } = [];
        public List<long> DanhMucChiTieuIds { get; set; } = [];
    }
}
