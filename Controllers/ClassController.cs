using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : Controller
    {
        private IClassBusiness _classBusiness;
        private ICourseBusiness _courseBusiness;
        private IMonitorBusiness _monitorBusiness;

        public ClassController(IClassBusiness classBusiness, ICourseBusiness courseBusiness, IMonitorBusiness monitorBusiness)
        {
            _classBusiness = classBusiness;
            _courseBusiness = courseBusiness;
            _monitorBusiness = monitorBusiness;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Class> GetById(string id)
        {
            return await _classBusiness.GetById(id);
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(Dictionary<string, SearchClass> formData)
        {
            try
            {
                (List<Class>, long) data;
                SearchClass param = formData["params"];
                if (param == null)
                {
                    data = await _classBusiness.SearchClass(new SearchClass() { PageIndex = 0, PageSize = 0, year = DateTime.Now.Year });
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
                    data = await _classBusiness.SearchClass(param);
                    return Ok(
                        new
                        {
                            TotalItems = data.Item2,
                            Data = data.Item1,
                            Page = param.PageIndex,
                            PageSize = param.PageSize
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
        public async Task<IActionResult> Create(Class c)
        {
            if (await _classBusiness.Create(c))
                return Ok(new { message = $"Đã tạo lớp {c.Id}!" });
            else
                return BadRequest(new { message = $"Mã lớp {c.Id} đã tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Class c)
        {
            if (await _classBusiness.Update(c))
                return Ok(new { message = $"Sửa thông tin lớp {c.Id} thành công!" });
            else
                return BadRequest(new { message = $"Mã lớp {c.Id} không tồn tại!" });
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _classBusiness.Delete(id))
                return Ok(new { message = $"Xoá thông tin lớp {id} thành công!" });
            else
                return BadRequest(new { message = $"Mã lớp {id} không tồn tại!" });
        }

        [HttpPost]
        [Route("create-class-courses")]
        public async Task<IActionResult> CreateCourses(ClassCourse classCourse)
        {
            if (await _courseBusiness.CreateMultiCourse(classCourse))
                return Ok(new { message = "Thêm thành công!" });
            else
                return BadRequest(new { message = "Thêm không thành công!" });
        }

        [HttpPost]
        [Route("delete-class-courses")]
        public async Task<IActionResult> DeleteCourses(ClassCourse classCourse)
        {
            if (await _courseBusiness.DeleteMultiCourse(classCourse))
                return Ok(new { message = "Xoá thành công!" });
            else
                return BadRequest(new { message = "Xoá không thành công!" });
        }

        [HttpGet]
        [Route("get-school-year-dropdown")]
        public Task<List<string>> GetSchoolYearDropdown(string id)
        {
            return _classBusiness.GetSchoolYearDropdown(id);
        }

        [HttpPost]
        [Route("set-monitor")]
        public async Task<IActionResult> SetMonitor(MonitorModel? monitor)
        {
            if (await _classBusiness.SetMonitor(monitor))
            {
                return Ok("Chọn lớp trưởng thành công!");
            }
            else
            {
                return BadRequest("Chọn lớp trưởng không thành công!");
            }
        }

        [HttpGet]
        [Route("get-monitor")]
        public async Task<Student> GetMonitor(string classId, int semester, string schoolYear)
        {
            return await _monitorBusiness.GetByClassId(classId, semester, schoolYear);
        }

        [HttpGet]
        [Route("get-courses")]
        public async Task<List<Course>> GetCourses(string id, int semester, string schoolYear)
        {
            return await _courseBusiness.GetCoursesOfClass(id, semester, schoolYear);
        }


    }
}