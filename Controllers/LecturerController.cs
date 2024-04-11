using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : Controller
    {
        private ILectureBusiness _bus;

        public LecturerController(ILectureBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Lecturer> GetById(string id)
        {
            return _bus.GetById(id);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchLecturer sl)
        {
            try
            {
                (List<Lecturer>, long) data;
                if (sl == null)
                {
                    data = await _bus.Search(new SearchLecturer() { PageIndex = 0, PageSize = 0 });
                    return Ok(
                    new
                    {
                        TotalItems = data.Item2,
                        Data = data.Item1,
                        Page = 0,
                        PageSize = 0
                    }
                    );
                }
                else
                {
                    data = await _bus.Search(sl);
                    return Ok(
                        new
                        {
                            TotalItems = data.Item2,
                            Data = data.Item1,
                            Page = sl.PageIndex,
                            PageSize = sl.PageSize
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Lecturer l)
        {
            if (await _bus.Create(l))
                return Ok(new { message = $"Đã tạo giảng viên {l.Id}!" });
            else
                return BadRequest(new { message = $"Mã giảng viên {l.Id} đã tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Lecturer l)
        {
            if (await _bus.Update(l))
                return Ok(new { message = $"Sửa thông tin giảng viên {l.Id} thành công!" });
            else
                return BadRequest(new { message = $"Mã giảng viên {l.Id} không tồn tại!" });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _bus.Delete(id))
                return Ok(new { message = $"Xoá thông tin giảng viên {id} thành công!" });
            else
                return BadRequest(new { message = $"Mã giảng viên {id} không tồn tại!" });
        }
    }
}