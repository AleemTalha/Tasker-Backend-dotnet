using Microsoft.AspNetCore.Mvc;
using Handlers;
using DTO.Category;
using Data;

namespace Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryDTO req)
        {
            return await CategoryH.AddCategory(req, _db);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            return await CategoryH.GetAllCategories(_db);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            return await CategoryH.GetCategoryById(id, _db);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditCategory(int id, CreateCategoryDTO req)
        {
            return await CategoryH.EditCategory(id, req, _db);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return await CategoryH.DeleteCategory(id, _db);
        }
    }
}