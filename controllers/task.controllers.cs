using Microsoft.AspNetCore.Mvc;
using Data;
using System.Data.Common;
using Microsoft.Extensions.ObjectPool;
using Handlers;
using DTO;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskControllers : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _acessor;

        public TaskControllers(AppDbContext db, IConfiguration config, IHttpContextAccessor acessor)
        {
            _db = db;
            _config = config;
            _acessor = acessor;
        }

        [HttpPost("/")]
        [Authorize]
        public async Task<IActionResult> AddCategory(AddTask req)
        {
            IActionResult result = await TaskH.AddTask(req, _db, _acessor);
            return result;
        }

        [HttpGet("/")]
        [Authorize]
        public async Task<IActionResult> GetAllTasks()
        {
            return await TaskH.GetAllTasks(_db, _acessor);
        }

        [HttpGet("/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int Id)
        {
            return await TaskH.GetTask(Id, _db, _acessor);
        }

        [HttpPut("/{id}")]
        [Authorize]
        public async Task<IActionResult> EditTask(int Id, EditTask req)
        {
            return await TaskH.EditTask(Id, _db, _acessor, req);
        }

        [HttpDelete("/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int Id, EditTask req)
        {
            return await TaskH.DeleteTask(Id, _db, _acessor);
        }

    }
}