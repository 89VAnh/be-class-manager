using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountBusiness _bus;

        public AccountController(IAccountBusiness accountBusiness)
        {
            _bus = accountBusiness;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Account loginAccount)
        {
            Account account = await _bus.Login(loginAccount.Username, loginAccount.Password);
            if (account == null)
                return StatusCode(401, new { message = "Tài khoản hoặc mật khẩu không đúng!" });

            var res = new { username = account.Username, email = account.Email, name = account.Name, role = account.Role, token = account.token };

            return Ok(res);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePw([FromBody] Dictionary<string, string> data)
        {
            try
            {
                string username = data["username"];
                string password = data["password"];
                string new_password = data["new_password"];

                Account account = await _bus.Login(username, password);
                if (account == null)
                    return BadRequest(new { message = "Mật khẩu hiện tại không chính xác" });

                if (await _bus.ChangePw(username, new_password))
                    return Ok(new { message = "Đổi mật khẩu thành công" });
                else
                    return BadRequest(new { message = "Đổi mật khẩu không thành công" });
            }
            catch
            {
                return BadRequest(new { message = "Không đầy đủ thông tin các trường" });
            }
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchAccount search)
        {
            try
            {
                (List<Account>, long) data;
                if (search == null)
                {
                    data = await _bus.Search(new SearchAccount() { PageIndex = 0, PageSize = 0 });
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

        [HttpGet]
        [Route("{username}")]
        public Task<Account> GetByUsername(string username)
        {
            return _bus.GetAccount(username);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Account a)
        {
            if (await _bus.Create(a))
                return Ok(new { message = $"Đã tạo tài khoản {a.Username}!" });
            else
                return BadRequest(new { message = $"Tài khoản {a.Username} đã tồn tại!" });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(Account a)
        {
            if (await _bus.Update(a))
                return Ok(new { message = $"Sửa thông tin tài khoản {a.Username} thành công!" });
            else
                return BadRequest(new { message = $"Tài khoản {a.Username} không tồn tại!" });
        }

        [HttpDelete]
        [Route("delete/{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            if (await _bus.Delete(username))
                return Ok(new { message = $"Xoá tài khoản {username} thành công!" });
            else
                return BadRequest(new { message = $"Tài khoản {username} không tồn tại!" });
        }
    }
}