using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        private const string ID = "id";

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                // check that email is unique
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered!");
                    return View("Index");
                }
                else
                {
                    // hash password
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                    // save to db
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();

                    // login user
                    HttpContext.Session.SetInt32(ID, newUser.UserId);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if (userInDb != null)
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

                    // result can be compared to 0 for failure
                    if (result != 0)
                    {
                        // success
                        HttpContext.Session.SetInt32(ID, userInDb.UserId);
                        return RedirectToAction("Success");
                    }
                }

                // login failed, don't give any specific information
                ModelState.AddModelError("LoginEmail", "Invalid Email and/or Password");
            }

            return View("Index");
        }

        [HttpGet]
        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("success")]
        public IActionResult Success()
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)    // check id is in session
            {
                var user = dbContext.Users.FirstOrDefault(u => u.UserId == id);

                if (user != null)   // check id is in db
                {
                    return RedirectToAction("Index", "Wedding");
                }
                else
                {
                    return RedirectToAction("Logout");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

    }
}
