using KPITrackerAPI.DTOs.CauHinhNguongDanhGiaKPI;
using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPITrackerAPI.Controllers
{
    [Route("api/cau-hinh-nguong-danh-gia-kpi")]
    [ApiController]
    [Authorize]
    public class CauHinhNguongDanhGiaKPIController : ControllerBase
    {
        private readonly ICauHinhNguongDanhGiaKPIService _service;

        public CauHinhNguongDanhGiaKPIController(ICauHinhNguongDanhGiaKPIService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CauHinhNguongDanhGiaKPIQueryDto query)
        {
            try
            {
                var data = await _service.GetAllAsync(query);
                return Ok(ApiResponse<List<CauHinhNguongDanhGiaKPIResponse>>.Ok(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                if (data == null)
                    return NotFound(ApiResponse<string>.Fail("Không tìm th?y c?u hình ngu?ng dánh giá KPI."));

                return Ok(ApiResponse<CauHinhNguongDanhGiaKPIResponse>.Ok(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCauHinhNguongDanhGiaKPIDto dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto, User?.Identity?.Name);
                return Ok(ApiResponse<long>.Ok(id, "T?o c?u hình ngu?ng dánh giá KPI thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateCauHinhNguongDanhGiaKPIDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto, User?.Identity?.Name);
                if (!result)
                    return NotFound(ApiResponse<string>.Fail("Không tìm th?y c?u hình ngu?ng dánh giá KPI."));

                return Ok(ApiResponse<bool>.Ok(true, "C?p nh?t c?u hình ngu?ng dánh giá KPI thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound(ApiResponse<string>.Fail("Không tìm th?y c?u hình ngu?ng dánh giá KPI."));

                return Ok(ApiResponse<bool>.Ok(true, "Xóa c?u hình ngu?ng dánh giá KPI thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }
    }
}
