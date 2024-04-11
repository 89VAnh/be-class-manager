using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : Controller
    {
        private IDepartmentBussiness _bus;

        public DepartmentController(IDepartmentBussiness bus)
        {
            _bus = bus;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<Department> GetById(string id)
        {
            return _bus.GetById(id);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Department d)
        {
            if (await _bus.Create(d))
                return Ok(new { message = $"Đã tạo khoa {d.Id}!" });
            else
                return BadRequest(new { message = $"Mã khoa {d.Id} đã tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Department d)
        {
            if (await _bus.Update(d))
                return Ok(new { message = $"Sửa thông tin khoá học {d.Id} thành công!" });
            else
                return BadRequest(new { message = $"Mã khoá học {d.Id} không tồn tại!" });
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

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchDepartment sd)
        {
            try
            {
                (List<Department>, long) data;
                if (sd == null)
                {
                    data = await _bus.Search(new SearchDepartment() { pageIndex = 0, pageSize = 0 });
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
                    data = await _bus.Search(sd);
                    return Ok(
                        new
                        {
                            TotalItems = data.Item2,
                            Data = data.Item1,
                            Page = sd.pageIndex,
                            PageSize = sd.pageSize
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