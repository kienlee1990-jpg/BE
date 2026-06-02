using KPITrackerAPI.Interfaces;
using KPITrackerAPI.DTOs.TheoDoiThucHienKPI;
using Microsoft.AspNetCore.Mvc;

namespace KPITrackerAPI.Controllers
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
                return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

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
                return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

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
                    return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id:long}/submit")]
        public async Task<IActionResult> Submit(long id)
        {
            try
            {
                var data = await _service.SubmitAsync(id);
                if (data == null)
                    return NotFound(new { message = "Khong tim thay theo doi thuc hien KPI." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:long}/approve")]
        public async Task<IActionResult> Approve(long id)
        {
            try
            {
                var data = await _service.ApproveAsync(id);
                if (data == null)
                    return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:long}/return-for-reentry")]
        public async Task<IActionResult> ReturnForReEntry(long id, [FromBody] ReturnTheoDoiThucHienKPIDto? dto)
        {
            try
            {
                var result = await _service.ReturnForReEntryAsync(id, dto?.LyDo);
                if (!result)
                    return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

                return Ok(new { message = "Ðã g?i tr? yêu c?u nh?p l?i." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id:long}/resubmit-returned")]
        public async Task<IActionResult> ResubmitReturned(long id)
        {
            try
            {
                var data = await _service.ResubmitReturnedAsync(id);
                if (data == null)
                    return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

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
                    return NotFound(new { message = "Không tìm th?y theo dõi th?c hi?n KPI." });

                return Ok(new { message = "Xóa thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
