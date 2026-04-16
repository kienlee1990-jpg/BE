using KPITrackerAPI.Interfaces;
using KPITrackerAPI.DTOs.KyBaoCaoKPI;
using Microsoft.AspNetCore.Mvc;

namespace KPITrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KyBaoCaoKPIController : ControllerBase
    {
        private readonly IKyBaoCaoKPIService _service;

        public KyBaoCaoKPIController(IKyBaoCaoKPIService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Không tìm th?y k? báo cáo KPI." });

            return Ok(data);
        }

        [HttpGet("by-nam/{nam}")]
        public async Task<IActionResult> GetByNam(int nam)
        {
            var data = await _service.GetByNamAsync(nam);
            return Ok(data);
        }

        [HttpGet("by-loaiky/{loaiKy}")]
        public async Task<IActionResult> GetByLoaiKy(string loaiKy)
        {
            var data = await _service.GetByLoaiKyAsync(loaiKy);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKyBaoCaoKPIDto dto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateKyBaoCaoKPIDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var data = await _service.UpdateAsync(id, dto);
                if (data == null)
                    return NotFound(new { message = "Không tìm th?y k? báo cáo KPI." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm th?y k? báo cáo KPI." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
