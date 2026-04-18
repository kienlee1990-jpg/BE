using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("NhomThiDuaDonVi")]
    public class NhomThiDuaDonVi
    {
        public long NhomThiDuaId { get; set; }
        public long DonViId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(NhomThiDuaId))]
        public NhomThiDua? NhomThiDua { get; set; }

        [ForeignKey(nameof(DonViId))]
        public DonVi? DonVi { get; set; }
    }
}
