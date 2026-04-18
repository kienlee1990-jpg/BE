using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("NhomThiDua")]
    public class NhomThiDua
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaNhom { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenNhom { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [MaxLength(30)]
        public string TrangThai { get; set; } = "HOAT_DONG";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<NhomThiDuaDonVi> NhomThiDuaDonVis { get; set; } = new List<NhomThiDuaDonVi>();
        public ICollection<NhomThiDuaChiTieu> NhomThiDuaChiTieus { get; set; } = new List<NhomThiDuaChiTieu>();
    }
}
