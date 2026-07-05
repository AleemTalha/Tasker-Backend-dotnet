using Microsoft.AspNetCore.Mvc;
using DTO.User;
using Data;
using Handlers;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _acessor;

        public UserController(AppDbContext db, IConfiguration config, IHttpContextAccessor acessor)
        {
            _db = db;
            _config = config;
            _acessor = acessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(SignUp req)
        {
            var result = await UserH.Register(req, _db);
            return result;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login req)
        {
            var result = await UserH.Login(req, _db, _config);
            return result;
        }


        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await UserH.GetProfile(_acessor, _db);
            return result;
        }

    }
}