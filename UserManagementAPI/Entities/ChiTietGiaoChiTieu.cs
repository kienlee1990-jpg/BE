using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPI_Tracker_API.Entities
{
    [Table("ChiTietGiaoChiTieu")]
    public class ChiTietGiaoChiTieu
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long DotGiaoChiTieuId { get; set; }

        [Required]
        public long DanhMucChiTieuId { get; set; }

        [Required]
        public long DonViNhanId { get; set; }

        public long? DonViThucHienChinhId { get; set; }

        public decimal? GiaTriMucTieu { get; set; }

        public string? GiaTriMucTieuText { get; set; }

        public long? ChiTietGiaoChaId { get; set; }

        public string? GhiChu { get; set; }

        public int? ThuTuHienThi { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; } = "DA_GIAO";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [ForeignKey(nameof(DotGiaoChiTieuId))]
        public DotGiaoChiTieu? DotGiaoChiTieu { get; set; }

        [ForeignKey(nameof(DanhMucChiTieuId))]
        public DanhMucChiTieu? DanhMucChiTieu { get; set; }

        [ForeignKey(nameof(DonViNhanId))]
        public DonVi? DonViNhan { get; set; }

        [ForeignKey(nameof(DonViThucHienChinhId))]
        public DonVi? DonViThucHienChinh { get; set; }

        [ForeignKey(nameof(ChiTietGiaoChaId))]
        public ChiTietGiaoChiTieu? ChiTietGiaoCha { get; set; }
        [MaxLength(30)]
        public string? TanSuatBaoCao { get; set; }

        public ICollection<ChiTietGiaoChiTieu> ChiTietGiaoChiTieuCons { get; set; } = new List<ChiTietGiaoChiTieu>();
    }
}