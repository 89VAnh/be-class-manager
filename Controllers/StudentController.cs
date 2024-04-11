using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private IStudentBusiness _bus;

        public StudentController(IStudentBusiness bus)
        {
            _bus = bus;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Student> GetById(string id)
        {
            return _bus.GetById(id);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Student s)
        {
            if (await _bus.Create(s))
                return Ok(new { message = $"Đã tạo sinh viên {s.Id}!" });
            else
                return BadRequest(new { message = $"Mã sinh viên {s.Id} đã tồn tại!" });
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(string id, string classId, int semester, string schoolYear)
        {
            if (await _bus.Delete(id, classId, semester, schoolYear))
                return Ok(new { message = $"Xoá thông tin sinh viên {id} thành công!" });
            else
                return BadRequest(new { message = $"Mã sinh viên {id} không tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Student s)
        {
            if (await _bus.Update(s))
                return Ok(new { message = $"Sửa thông tin sinh viên {s.Id} thành công!" });
            else
                return BadRequest(new { message = $"Mã sinh viên {s.Id} không tồn tại!" });
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchStudent ss)
        {
            try
            {
                (List<Student>, long) data;
                if (ss == null)
                {
                    data = await _bus.Search(new SearchStudent() { PageIndex = 0, PageSize = 0 });
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
                    data = await _bus.Search(ss);
                    return Ok(
                        new
                        {
                            TotalItems = data.Item2,
                            Data = data.Item1,
                            Page = ss.PageIndex,
                            PageSize = ss.PageSize
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}