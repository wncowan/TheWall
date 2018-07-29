using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private WallContext _context;

        public HomeController(WallContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model)
        {

            if(ModelState.IsValid)
            {
                User CheckUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                if(CheckUser != null)
                {
                    TempData["EmailInUseError"] = "Email Aleady in use";
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    User user = new User(){
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now
                    };
                    user.Password = Hasher.HashPassword(user, model.Password);
                    _context.Add(user);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("currentUserId", user.UserId);
                    HttpContext.Session.SetString("currentFirstName", user.FirstName);
                    return RedirectToAction("Dashboard");
                }
            }
            else{
                return View("Index");
            }
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel model)
        {
            
            User logUser = _context.Users.SingleOrDefault(_user => _user.Email == model.Email);
            if (logUser == null)
            {
                TempData["EmailError"] = "Email Not Found!";
                return RedirectToAction("Index");
            }
            else
            {
                if(logUser != null && model.Password != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    // Pass the user object, the hashed password, and the PasswordToCheck
                    if(0 != Hasher.VerifyHashedPassword(logUser, logUser.Password, model.Password))
                    {
                        //Handle success
                        HttpContext.Session.SetInt32("currentUserId", logUser.UserId);
                        HttpContext.Session.SetString("currentFirstName", logUser.FirstName);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["PasswordError"] = "Invalid password";
                        return View("Index");
                    }
                }
                return View("Index");
                // if (logUser.Password == user.Password)
                // {
                //     return RedirectToAction("Success");
                // }
                // else
                // {
                //     TempData["PasswordError"] = "Invalid password";
                //     return RedirectToAction("Index");
                // }
            }
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                List<Message> wallMessages = _context.Messages
                                                        .Include(m => m.User)
                                                        .Include(m => m.Comments)
                                                        .ThenInclude(c => c.User)
                                                        .ToList();
                ViewBag.wallMessages = wallMessages;
                ViewBag.currentUser = currentUser;
                return View();
            }
        }

        [HttpPost]
        [Route("addmessage")]
        public IActionResult AddMessage(MessageViewModel model)
        {
            if(ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("currentUserId");
                Message message = new Message(){
                    Content = model.Content,
                    UserId = (int)userId,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now
                };
                _context.Add(message);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else{
                TempData["MessageEmpty"] = "Message is empty";
                return RedirectToAction("Dashboard");
            }
            

        }

        [HttpPost]
        [Route("addcomment/{messageid}")]
        public IActionResult AddComment(string CommentContent, int messageid)
        {
            if(CommentContent == null){
                TempData["CommentError"] = "Comment field empty";
                return RedirectToAction("Dashboard");
            }
            else
            {
                int? userId = HttpContext.Session.GetInt32("currentUserId");
                Comment comment = new Comment(){
                    Content = CommentContent,
                    UserId = (int)userId,
                    MessageId = messageid,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now
                };
                _context.Add(comment);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
