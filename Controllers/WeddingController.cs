using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;


namespace WeddingPlanner.Controllers
{
    public class WeddingController : Controller
    {
        private MyContext dbContext;
        private const string ID = "id";

        public WeddingController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("weddings")]
        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                // var weddings = dbContext.Weddings.Include(w => w.GuestList).ThenInclude(gw => gw.User);
                var weddings = dbContext.GetAllWeddings();

                ViewBag.UserId = id.Value;


                return View(weddings);
            }
            else
            {
                return Redirect("/logout");
            }
        }

        [HttpGet]
        [Route("weddings/{wid}")]
        public IActionResult ShowWedding(int wid)
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                var wedding = dbContext.Weddings.Include(w => w.GuestList)
                                                .ThenInclude(u => u.User)
                                                .FirstOrDefault(x => x.WeddingId == wid);
                if (wedding != null)
                {
                    return View(wedding);
                }
                else
                {
                    return Redirect("/weddings");
                }
            }
            else
            {
                return Redirect("/logout");
            }
        }

        [HttpGet]
        [Route("weddings/new")]
        public IActionResult NewWeddingForm()
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                return View();
            }
            else
            {
                return Redirect("/logout");
            }
        }

        [HttpPost]
        [Route("weddings/new")]
        public IActionResult CreateNewWedding(Wedding wedding)
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                if (ModelState.IsValid)
                {
                    if (dbContext.InsertWedding(wedding, id.Value))
                    {
                        return Redirect($"/weddings/{wedding.WeddingId}");
                    }

                    return Redirect("/weddings");
                }
                else
                {
                    return View("NewWeddingForm");
                }
            }
            else
            {
                return Redirect("/logout");
            }
        }

        [HttpGet]
        [Route("weddings/{wid}/rsvp")]
        public IActionResult JoinWedding(int wid)
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                dbContext.JoinWeddingGuests(wid, id.Value);

                return Redirect($"/weddings");
            }

            return Redirect("/weddings");
        }


        [HttpGet]
        [Route("weddings/{wid}/unrsvp")]
        public IActionResult LeaveWedding(int wid)
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                dbContext.LeaveWedding(wid, id.Value);

                return Redirect($"/weddings");
            }

            return Redirect("/weddings");
        }

        [HttpGet]
        [Route("weddings/{wid}/delete")]
        public IActionResult DeleteWedding(int wid)
        {
            int? id = HttpContext.Session.GetInt32(ID);
            if (id.HasValue)
            {
                dbContext.DeleteWedding(wid, id.Value);
            }

            return Redirect("/weddings");
        }
    }
}