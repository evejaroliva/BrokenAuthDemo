﻿using BrokenAuthDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrokenAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly List<User> _users = new List<User>
        {
            new User { Username = "admin", Password = "123" }, // Weak password
            new User { Username = "user", Password = "password" } // Weak password
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = _users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            if (user == null)
            {
                return Unauthorized($"Invalid credentials {HttpContext.Session.Id}");
            }

            // Clear the current session to invalidate the old session ID
            //HttpContext.Session.Clear();

            // Vulnerable: Reusing session ID without regeneration / No clear session
            // Set the new session data
            HttpContext.Session.SetString("User", user.Username);


            // Ensure the session is saved with a new ID
            //HttpContext.Session.CommitAsync().Wait();

            return Ok($"Login successful {HttpContext.Session.Id}");
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var username = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized($"Not logged in {HttpContext.Session.Id}");
            }
            return Ok($"Welcome, {username}");
        }
    }
}