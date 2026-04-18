using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("NhomThiDuaChiTieu")]
    public class NhomThiDuaChiTieu
    {
        public long NhomThiDuaId { get; set; }
        public long DanhMucChiTieuId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(NhomThiDuaId))]
        public NhomThiDua? NhomThiDua { get; set; }

        [ForeignKey(nameof(DanhMucChiTieuId))]
        public DanhMucChiTieu? DanhMucChiTieu { get; set; }
    }
}
