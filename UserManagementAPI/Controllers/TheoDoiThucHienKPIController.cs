using KPI_Tracker_API.Interfaces;
using KPI_Tracker_API.Models.DTOs.TheoDoiThucHienKPI;
using Microsoft.AspNetCore.Mvc;

namespace KPI_Tracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheoDoiThucHienKPIController : ControllerBase
    {
        private readonly ITheoDoiThucHienKPIService _service;

        public TheoDoiThucHienKPIController(ITheoDoiThucHienKPIService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Không tìm thấy theo dõi thực hiện KPI." });

            return Ok(data);
        }

        [HttpGet("by-chitiet/{chiTietGiaoChiTieuId:long}")]
        public async Task<IActionResult> GetByChiTietGiaoChiTieuId(long chiTietGiaoChiTieuId)
        {
            var data = await _service.GetByChiTietGiaoChiTieuIdAsync(chiTietGiaoChiTieuId);
            return Ok(data);
        }

        [HttpGet("by-kybaocao/{kyBaoCaoKPIId:long}")]
        public async Task<IActionResult> GetByKyBaoCaoKPIId(long kyBaoCaoKPIId)
        {
            var data = await _service.GetByKyBaoCaoKPIIdAsync(kyBaoCaoKPIId);
            return Ok(data);
        }

        [HttpGet("by-chitiet-va-ky")]
        public async Task<IActionResult> GetByChiTietVaKy([FromQuery] long chiTietGiaoChiTieuId, [FromQuery] long kyBaoCaoKPIId)
        {
            var data = await _service.GetByChiTietVaKyAsync(chiTietGiaoChiTieuId, kyBaoCaoKPIId);
            if (data == null)
                return NotFound(new { message = "Không tìm thấy theo dõi thực hiện KPI." });

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTheoDoiThucHienKPIDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var data = await _service.CreateAsync(dto);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTheoDoiThucHienKPIDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var data = await _service.UpdateAsync(id, dto);
                if (data == null)
                    return NotFound(new { message = "Không tìm thấy theo dõi thực hiện KPI." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy theo dõi thực hiện KPI." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}