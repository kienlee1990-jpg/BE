using KPITrackerAPI.DTOs.ChiTietGiaoChiTieu;
using KPITrackerAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KPITrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietGiaoChiTieuController : ControllerBase
    {
        private readonly IChiTietGiaoChiTieuService _service;

        public ChiTietGiaoChiTieuController(IChiTietGiaoChiTieuService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm th?y chi ti?t giao ch? tiêu." });

            return Ok(result);
        }

        [HttpGet("by-dot/{dotGiaoChiTieuId:long}")]
        public async Task<IActionResult> GetByDotGiaoChiTieuId(long dotGiaoChiTieuId)
        {
            var result = await _service.GetByDotGiaoChiTieuIdAsync(dotGiaoChiTieuId);
            return Ok(result);
        }

        [HttpGet("by-donvi-nhan/{donViNhanId:long}")]
        public async Task<IActionResult> GetByDonViNhanId(long donViNhanId)
        {
            var result = await _service.GetByDonViNhanIdAsync(donViNhanId);
            return Ok(result);
        }

        [HttpGet("children/{chiTietGiaoChaId:long}")]
        public async Task<IActionResult> GetChildren(long chiTietGiaoChaId)
        {
            var result = await _service.GetChildrenAsync(chiTietGiaoChaId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChiTietGiaoChiTieuDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateChiTietGiaoChiTieuDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Không tìm th?y chi ti?t giao ch? tiêu." });

                return Ok(result);
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
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Không tìm th?y chi ti?t giao ch? tiêu." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

