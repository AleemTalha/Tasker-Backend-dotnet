using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using DTO.Category;
using Microsoft.AspNetCore.Mvc;

namespace Handlers
{
    public static class CategoryH
    {
        public static async Task<IActionResult> AddCategory(CreateCategoryDTO ct, AppDbContext db)
        {
            try
            {
                var existingCategory = await db.Category
                    .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == ct.Name.ToLower().Trim());

                if (existingCategory != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Category with the same name already exists."
                    });
                }

                if (string.IsNullOrWhiteSpace(ct.Name) || ct.Name.Length > 100)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Name is required and cannot exceed 100 characters."
                    });
                }

                if (ct.Description != null && ct.Description.Length > 500)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Description cannot exceed 500 characters."
                    });
                }

                var category = new Category
                {
                    Name = ct.Name,
                    Description = ct.Description
                };

                await db.Category.AddAsync(category);
                await db.SaveChangesAsync();

                return new ObjectResult(new
                {
                    category,
                    message = "Category created successfully."
                })
                {
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<IActionResult> GetAllCategories(AppDbContext db)
        {
            try
            {
                var categories = await db.Category
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Description,
                        c.CreatedAt,
                        c.UpdatedAt
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    categories,
                    message = "OK"
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<IActionResult> GetCategoryById(int id, AppDbContext db)
        {
            try
            {
                var category = await db.Category.FindAsync(id);

                if (category == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Category not found."
                    });
                }

                return new OkObjectResult(new
                {
                    category,
                    message = "OK"
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<IActionResult> EditCategory(int id, CreateCategoryDTO ct, AppDbContext db)
        {
            try
            {
                var category = await db.Category.FindAsync(id);

                if (category == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Category not found."
                    });
                }

                category.Name = ct.Name;
                category.Description = ct.Description;
                category.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    category,
                    message = "Category updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<IActionResult> DeleteCategory(int id, AppDbContext db)
        {
            try
            {
                var category = await db.Category.FindAsync(id);

                if (category == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Category not found."
                    });
                }

                db.Category.Remove(category);
                await db.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    message = "Category deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    error = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }
    }

   
}