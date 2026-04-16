using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("TheoDoiThucHienKPI")]
    public class TheoDoiThucHienKPI
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ChiTietGiaoChiTieuId { get; set; }

        [Required]
        public long KyBaoCaoKPIId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriDauKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriThucHienTrongKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriCuoiKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriLuyKe { get; set; }

        public string? NhanXet { get; set; }

        [MaxLength(30)]
        public string TrangThai { get; set; } = "MOI_TAO";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [ForeignKey(nameof(ChiTietGiaoChiTieuId))]
        public ChiTietGiaoChiTieu? ChiTietGiaoChiTieu { get; set; }

        [ForeignKey(nameof(KyBaoCaoKPIId))]
        public KyBaoCaoKPI? KyBaoCaoKPI { get; set; }
    }
}
