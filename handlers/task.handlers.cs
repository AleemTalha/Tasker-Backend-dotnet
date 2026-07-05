using Microsoft.AspNetCore.Mvc;
using DTO;
using Data;
using Models;
using Microsoft.Extensions.ObjectPool;
using Microsoft.EntityFrameworkCore;
namespace Handlers
{
    public static class TaskH
    {
        public static async Task<IActionResult> AddTask(AddTask req, AppDbContext db, IHttpContextAccessor accessor)
        {
            try
            {
                var User = accessor.HttpContext?.User;

                string StrId = User?.FindFirst("Id")!.Value!;

                if (string.IsNullOrEmpty(StrId))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Invalid token or user not found"
                    });
                }

                int UserId = int.Parse(StrId);



                if (string.IsNullOrWhiteSpace(req.Title))
                {
                    return new ObjectResult(new
                    {
                        message = "Invalid Title"
                    })
                    {
                        StatusCode = 400
                    };
                }


                var newTask = new ToDoTask
                {
                    Title = req.Title,
                    Description = req.Description ?? "",
                    UserId = UserId,
                    CategoryId = req.CategoryId,
                };

                await db.Task.AddAsync(newTask);
                await db.SaveChangesAsync();


                return new ObjectResult(new
                {
                    message = "Tasks created successfully"
                });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ObjectResult(new
                {
                    message = "Internal server error"
                });
            }
        }

        public static async Task<IActionResult> GetAllTasks(AppDbContext db, IHttpContextAccessor accessor)
        {
            try
            {

                var user = accessor?.HttpContext?.User;
                var idStr = user?.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(idStr))
                {
                    Console.WriteLine("User ID claim is missing in the token.");
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Invalid token or user not found"
                    });
                }
                int idInt = int.Parse(idStr);


                var tasks = await db.Task
                .Where(t => t.UserId == idInt)
                .ToListAsync();

                return new OkObjectResult(new
                {
                    message = "Fetching successful",
                    tasks
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

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

        public static async Task<IActionResult> GetTask(int Id, AppDbContext db, IHttpContextAccessor accessor)
        {
            try
            {
                var user = accessor.HttpContext?.User;
                var idStr = user?.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(idStr))
                {
                    Console.WriteLine("User ID claim is missing in the token.");
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Invalid token or user not found"
                    });
                }
                int idInt = int.Parse(idStr);
                var task = await db.Task
                .Where(t => t.UserId == idInt && Id == t.Id)
                .FirstOrDefaultAsync();

                if (task == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Task not found"
                    });
                }

                return new OkObjectResult(new
                {
                    message = "Task found",
                    task = task
                });
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return new ObjectResult(new
                {
                    message = "Internal server error",
                    error = err.Message
                });
            }
        }

        public static async Task<IActionResult> DeleteTask(int Id, AppDbContext db, IHttpContextAccessor accessor)
        {
            try
            {
                var user = accessor.HttpContext?.User;
                var idStr = user?.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(idStr))
                {
                    Console.WriteLine("User ID claim is missing in the token.");
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Invalid token or user not found"
                    });
                }
                int idInt = int.Parse(idStr);

                var task = await db.Task
                .FirstOrDefaultAsync(t => t.UserId == idInt && t.Id == Id);

                if (task == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Task not found"
                    });
                }

                db.Task.Remove(task);
                await db.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    message = "Task deleted successfully"
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    ex
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static async Task<IActionResult> EditTask(int Id, AppDbContext db, IHttpContextAccessor accessor, EditTask req)
        {
            try
            {
                var user = accessor.HttpContext?.User;
                var idStr = user?.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(idStr))
                {
                    Console.WriteLine("User ID claim is missing in the token.");
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Invalid token or user not found"
                    });
                }
                int idInt = int.Parse(idStr);

                var task = await db.Task
                .FirstOrDefaultAsync(t => t.UserId == idInt && Id == t.Id);

                if (task == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "Task not found"
                    });
                }

                task.Title = req.Title;
                task.Description = req.Description ?? "";
                task.progress = req.progress;
                task.isActive = req.isActive;

                await db.SaveChangesAsync();


                return new OkObjectResult(new
                {
                    message = "Task updated successfully",
                    data = task
                });





            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ObjectResult(new
                {
                    message = "Internal Server Error",
                    ex
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}