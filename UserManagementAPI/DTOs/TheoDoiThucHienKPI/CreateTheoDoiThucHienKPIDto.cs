using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.Models.DTOs.TheoDoiThucHienKPI
{
    public class CreateTheoDoiThucHienKPIDto
    {
        [Required]
        public long ChiTietGiaoChiTieuId { get; set; }

        [Required]
        public long KyBaoCaoKPIId { get; set; }

        public decimal? GiaTriDauKy { get; set; }

        public decimal? GiaTriThucHienTrongKy { get; set; }

        public string? NhanXet { get; set; }
    }
}