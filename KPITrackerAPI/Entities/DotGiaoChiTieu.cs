using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("DotGiaoChiTieu")]
    public class DotGiaoChiTieu
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaDotGiao { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenDotGiao { get; set; } = string.Empty;

        public int NamApDung { get; set; }

        [Required]
        [MaxLength(50)]
        public string NguonDotGiao { get; set; } = string.Empty; // BO_GIAO / THANH_PHO_GIAO

        [Required]
        [MaxLength(50)]
        public string CapGiao { get; set; } = string.Empty; // BO / THANH_PHO

        public long DonViGiaoId { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; } = "DRAFT"; // DRAFT / PUBLISHED / CLOSED

        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ChiTietGiaoChiTieu> ChiTietGiaoChiTieux { get; set; } = new List<ChiTietGiaoChiTieu>();
    }
}

