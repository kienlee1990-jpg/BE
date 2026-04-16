using KPITrackerAPI.Interfaces;
using KPITrackerAPI.DTOs.DonVi;
using Microsoft.AspNetCore.Mvc;

namespace KPITrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonViController : ControllerBase
    {
        private readonly IDonViService _service;

        public DonViController(IDonViService service)
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
                return NotFound(new { message = "Không tìm th?y don v?." });

            return Ok(data);
        }

        [HttpGet("children/{donViChaId}")]
        public async Task<IActionResult> GetChildren(long donViChaId)
        {
            var data = await _service.GetChildrenAsync(donViChaId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDonViDto dto)
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
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDonViDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var data = await _service.UpdateAsync(id, dto);
                if (data == null)
                    return NotFound(new { message = "Không tìm th?y don v?." });

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
                    return NotFound(new { message = "Không tìm th?y don v?." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
