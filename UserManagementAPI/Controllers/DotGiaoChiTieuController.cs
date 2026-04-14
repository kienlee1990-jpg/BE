using KPI_Tracker_API.DTOs;
using KPI_Tracker_API.DTOs.DotGiaoChiTieu;
using KPI_Tracker_API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KPI_Tracker_API.Controllers
{
    [ApiController]
    [Route("api/dot-giao-chi-tieu")]
    public class DotGiaoChiTieuController : ControllerBase
    {
        private readonly IDotGiaoChiTieuService _service;

        public DotGiaoChiTieuController(IDotGiaoChiTieuService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDotGiaoChiTieuDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int? namApDung,
            [FromQuery] string? nguonDotGiao,
            [FromQuery] string? capGiao,
            [FromQuery] string? trangThai)
        {
            var result = await _service.GetAllAsync(
                keyword,
                namApDung,
                nguonDotGiao,
                capGiao,
                trangThai);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy đợt giao chỉ tiêu." });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDotGiaoChiTieuDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy đợt giao chỉ tiêu." });

                return Ok(result);
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
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Không tìm thấy đợt giao chỉ tiêu." });

                return Ok(new { message = "Xóa đợt giao chỉ tiêu thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}