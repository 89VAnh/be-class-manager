using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        private ICourseBusiness _bus;

        public CourseController(ICourseBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Course> GetById(string id)
        {
            return _bus.GetById(id);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchCourse search)
        {
            try
            {
                (List<Course>, long) data;
                if (search == null)
                {
                    data = await _bus.Search(new SearchCourse() { PageIndex = 0, PageSize = 0 });
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
                    data = await _bus.Search(search);
                    return Ok(
                        new
                        {
                            TotalItems = data.Item2,
                            Data = data.Item1,
                            Page = search.PageIndex,
                            PageSize = search.PageSize
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
        public async Task<IActionResult> Create(Course c)
        {
            if (await _bus.Create(c))
                return Ok(new { message = $"Đã tạo khoá học {c.Id}!" });
            else
                return BadRequest(new { message = $"Mã khoá học {c.Id} đã tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Course c)
        {
            if (await _bus.Update(c))
                return Ok(new { message = $"Sửa thông tin khoá học {c.Id} thành công!" });
            else
                return BadRequest(new { message = $"Mã khoá học {c.Id} không tồn tại!" });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _bus.Delete(id))
                return Ok(new { message = $"Xoá thông tin khoá học {id} thành công!" });
            else
                return BadRequest(new { message = $"Mã khoá học {id} không tồn tại!" });
        }
    }
}