using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("DanhGiaKPI")]
    public class DanhGiaKPI
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ChiTietGiaoChiTieuId { get; set; }

        [Required]
        public long KyBaoCaoKPIId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriMucTieu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriDauKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriCuoiKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriCungKyNamTruoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ChenhLechSoVoiDauKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TyLeTangTruongSoVoiDauKy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ChenhLechSoVoiCungKyNamTruoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TyLeTangTruongSoVoiCungKyNamTruoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TyLeHoanThanh { get; set; }

        [MaxLength(50)]
        public string? XepLoai { get; set; }

        [MaxLength(50)]
        public string? KetQua { get; set; }

        public string? NhanXetDanhGia { get; set; }

        [MaxLength(100)]
        public string? NguoiDanhGia { get; set; }

        public DateTime? NgayDanhGia { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(ChiTietGiaoChiTieuId))]
        public ChiTietGiaoChiTieu? ChiTietGiaoChiTieu { get; set; }

        [ForeignKey(nameof(KyBaoCaoKPIId))]
        public KyBaoCaoKPI? KyBaoCaoKPI { get; set; }
    }
}
