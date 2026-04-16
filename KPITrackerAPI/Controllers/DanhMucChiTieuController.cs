using Microsoft.AspNetCore.Mvc;
using KPITrackerAPI.DTOs.DanhMucChiTieu;
using KPITrackerAPI.Interfaces;

namespace KPITrackerAPI.Controllers
{
    [ApiController]
    [Route("api/danh-muc-chi-tieu")]
    public class DanhMucChiTieuController : ControllerBase
    {
        private readonly IDanhMucChiTieuService _service;

        public DanhMucChiTieuController(IDanhMucChiTieuService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDanhMucChiTieuDto dto)
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
            [FromQuery] string? nguonChiTieu,
            [FromQuery] string? loaiChiTieu,
            [FromQuery] string? capApDung,
            [FromQuery] string? trangThaiSuDung,
            [FromQuery] bool? coChoPhepPhanRa)
        {
            var result = await _service.GetAllAsync(
                keyword,
                nguonChiTieu,
                loaiChiTieu,
                capApDung,
                trangThaiSuDung,
                coChoPhepPhanRa);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm th?y danh m?c ch? tiêu." });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDanhMucChiTieuDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Không tìm th?y danh m?c ch? tiêu." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Không tìm th?y danh m?c ch? tiêu." });

            return Ok(new { message = "Xóa danh m?c ch? tiêu thành công." });
        }
    }
}

