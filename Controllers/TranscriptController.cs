using BUS.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private Excel _excel;

        public TranscriptController(Excel excel)
        {
            _excel = excel;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile file, string classId, int Semester, string SchoolYear)
        {
            string fileName = file.FileName;

            if (Path.GetExtension(fileName).ToLower() == ".xlsx")
            {
                _excel.LoadFile(file);
                if (!await _excel.LoadToDB(classId, Semester, SchoolYear))
                {
                    return BadRequest($"Không đọc được file {fileName}!");
                }
            }
            else return BadRequest($"Vui lòng chọn file excel có định dạng .xlsx");

            return Ok($"Tải lên file '{fileName}' thành công!");
        }
    }
}