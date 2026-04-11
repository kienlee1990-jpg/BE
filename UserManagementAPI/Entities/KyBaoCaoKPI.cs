using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPI_Tracker_API.Entities
{
    [Table("KyBaoCaoKPI")]
    public class KyBaoCaoKPI
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaKy { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenKy { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string LoaiKy { get; set; } = string.Empty; // THANG, QUY, NAM, 6THANG

        [Required]
        public int Nam { get; set; }

        public int? SoKy { get; set; }

        [Required]
        public DateTime TuNgay { get; set; }

        [Required]
        public DateTime DenNgay { get; set; }

        [Required]
        public DateTime NgayDauKy { get; set; }

        [Required]
        public DateTime NgayCuoiKy { get; set; }

        [MaxLength(30)]
        public string TrangThai { get; set; } = "MOI_TAO";

        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public ICollection<TheoDoiThucHienKPI> TheoDoiThucHienKPIs { get; set; } = new List<TheoDoiThucHienKPI>();
        public ICollection<DanhGiaKPI> DanhGiaKPIs { get; set; } = new List<DanhGiaKPI>();
    }
}