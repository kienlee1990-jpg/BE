using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPI_Tracker_API.Entities
{
    [Table("CauHinhNguongDanhGiaKPI")]
    public class CauHinhNguongDanhGiaKPI
    {
        [Key]
        public long Id { get; set; }

        public long? DanhMucChiTieuId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TuTyLe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DenTyLe { get; set; }

        [Required]
        [MaxLength(50)]
        public string XepLoai { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Diem { get; set; }

        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(DanhMucChiTieuId))]
        public DanhMucChiTieu? DanhMucChiTieu { get; set; }
    }
}