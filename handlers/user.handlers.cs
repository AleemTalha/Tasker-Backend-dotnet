using Models;
using DTO.User;
using Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Helpers;

namespace Handlers
{
    public static class UserH
    {
        public static async Task<IActionResult> Register(SignUp req, AppDbContext db)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Email) || req.Email.Length > 100)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Email is required and cannot exceed 100 characters."
                    });
                }

                if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "Password is required and must be at least 6 characters long."
                    });
                }

                var existingUser = await db.User
                    .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == req.Email.ToLower().Trim());

                if (existingUser != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = "User with this email already exists."
                    });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

                var newUser = new User
                {
                    FullName = req.FullName,
                    Email = req.Email.ToLower().Trim(),
                    Password = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await db.User.AddAsync(newUser);
                await db.SaveChangesAsync();


                return new OkObjectResult(new
                {
                    newUser,
                    message = "Ok",

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

        public static async Task<IActionResult> Login(Login req, AppDbContext db, IConfiguration config)
        {
            try
            {


                var user = await db.User
                .Where(u => u.Email.ToLower().Trim() == req.Email.ToLower().Trim())
                .Select(u => new
                {
                    u.Email,
                    u.Password,
                    u.FullName,
                    u.Id,
                    u.IsActive
                })
                .FirstOrDefaultAsync();


                if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
                {
                    return new BadRequestObjectResult(new { message = "Invalid Email or Password" }) { StatusCode = 400 };
                }

                var token = JwtHelper.GenerateToken(user.Email, user.FullName, user.Id, config);

                return new ObjectResult(new
                {
                    message = "Login successful",
                    token = token
                })
                {
                    StatusCode = 200
                };
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

        public static async Task<IActionResult> GetProfile(IHttpContextAccessor accessor, AppDbContext db)
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
                var dbUser = await db.User
                .Where(u => u.Id == idInt)
                .Select(
                    u => new
                    {
                        u.FullName,
                        u.Email,
                        u.CreatedAt,
                        u.UpdatedAt,
                        u.IsActive
                    }
                )
                .FirstOrDefaultAsync();
                if (dbUser == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        message = "User not found"
                    });
                }
                return new OkObjectResult(new
                {
                    message = "Profile fetched successfully",
                    user = dbUser
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