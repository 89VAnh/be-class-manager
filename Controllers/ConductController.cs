using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConductController : ControllerBase
    {
        private IConductBusiness _conductBusiness;
        private IClassBusiness _classBusiness;

        public ConductController(IConductBusiness conductBusiness, IMonitorBusiness monitorBusiness, IClassBusiness classBusiness)
        {
            _conductBusiness = conductBusiness;
            _classBusiness = classBusiness;
        }

        [HttpPost]
        [Route("search")]
        public async Task<List<Conduct>> GetClassConducts(SearchConduct search)
        {
            return await _conductBusiness.Search(search);
        }

        [HttpPut]
        [Route("update-multiple-conduct")]
        public async Task<IActionResult> UpdateConducts(Dictionary<string, List<MinConduct>> data)
        {
            try
            {
                if (!await _conductBusiness.UpdateMultiple(data["conducts"]))
                {
                    throw new Exception();
                };
                return Ok("Update success!!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-excel-file-of-class")]
        public async Task<IActionResult> GetExcelFileOfClass(string classId, int semester, string schoolYear, string monitorId)
        {
            _classBusiness.SetMonitor(new MonitorModel() { ClassId = classId, Semester = semester, SchoolYear = schoolYear, MonitorId = monitorId });

            var s = await _conductBusiness.ExportToExcel(classId, semester, schoolYear, monitorId);
            var stream = new MemoryStream(s);

            string filename = $"Tong hop KQRL_HK{semester}_{schoolYear}_{classId}";

            Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}