using Microsoft.AspNetCore.Mvc;
using KPITrackerAPI.Interfaces;

namespace KPITrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaKPIController : ControllerBase
    {
        private readonly IDanhGiaKPIService _danhGiaKPIService;

        public DanhGiaKPIController(IDanhGiaKPIService danhGiaKPIService)
        {
            _danhGiaKPIService = danhGiaKPIService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _danhGiaKPIService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _danhGiaKPIService.GetByIdAsync(id);

            if (data == null)
                return NotFound(new { message = "Không tìm th?y DanhGiaKPI." });

            return Ok(data);
        }

        [HttpGet("by-chi-tiet-giao-chi-tieu/{chiTietGiaoChiTieuId:long}")]
        public async Task<IActionResult> GetByChiTietGiaoChiTieuId(long chiTietGiaoChiTieuId)
        {
            var data = await _danhGiaKPIService.GetByChiTietGiaoChiTieuIdAsync(chiTietGiaoChiTieuId);
            return Ok(data);
        }

        [HttpGet("by-ky-bao-cao/{kyBaoCaoKPIId:long}")]
        public async Task<IActionResult> GetByKyBaoCaoKPIId(long kyBaoCaoKPIId)
        {
            var data = await _danhGiaKPIService.GetByKyBaoCaoKPIIdAsync(kyBaoCaoKPIId);
            return Ok(data);
        }

        [HttpPost("recalculate")]
        public async Task<IActionResult> Recalculate([FromQuery] long chiTietGiaoChiTieuId, [FromQuery] long kyBaoCaoKPIId)
        {
            try
            {
                var username = User?.Identity?.Name ?? "admin";

                var result = await _danhGiaKPIService.UpsertDanhGiaKPIAsync(
                    chiTietGiaoChiTieuId,
                    kyBaoCaoKPIId,
                    username
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long chiTietGiaoChiTieuId, [FromQuery] long kyBaoCaoKPIId)
        {
            var deleted = await _danhGiaKPIService.DeleteDanhGiaKPIAsync(chiTietGiaoChiTieuId, kyBaoCaoKPIId);

            if (!deleted)
                return NotFound(new { message = "Không tìm th?y DanhGiaKPI d? xóa." });

            return Ok(new { message = "Xóa DanhGiaKPI thành công." });
        }
    }
}
