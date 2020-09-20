using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace WebAppProject.Controllers
{

    public class UsersController : Controller
    {
        private readonly MvcProjectContext _context;

        public UsersController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        /**********************************************   log in and register   **************************************************************/
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {

                var objUser = _context.User.Where(u => u.UserName.Equals(username) && u.Password.Equals(password)) ; 

                    if (objUser.FirstOrDefault() != null)
                    {


                        if (objUser.FirstOrDefault().IsAdmin == false) //check if is not admin
                        {
                            HttpContext.Session.SetString("IsAdmin", "false");
                            HttpContext.Session.SetString("UserId", objUser.First().UserID.ToString());
                            HttpContext.Session.SetString("UserName", objUser.First().UserName.ToString());

                          //  return RedirectToAction("Home", "Index"); // what is the page that whould open?
                        }
                        else
                        {
                                HttpContext.Session.SetString("UserId", objUser.First().UserID.ToString());
                                HttpContext.Session.SetString("UserName", objUser.First().UserName.ToString());
                                HttpContext.Session.SetString("IsAdmin", "true");
                               // return RedirectToAction("Index", "AdminInfoes");// what is the page that whould open?
                        }

                         return RedirectToAction("Index"); // what is the page that whould open?


                      }

                else
                    {
                                return RedirectToAction("Index","Home" );// what is the page that whould open?
                     }
                }
            
            return View(); //change 
        }


    [HttpPost]
    public ActionResult LogOff()
        {
        HttpContext.Session.SetString("UserId", null);
        HttpContext.Session.SetString("UserName", null);
        HttpContext.Session.SetString("IsAdmin", null);
        return RedirectToAction("Home", "???");   //change 
        }

        public ActionResult LogedOff()
        {
            return View();  //change 
        }


        [HttpPost]
        public ActionResult LoggedIn()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();//change 
            }
            else
            {
                return RedirectToAction("Home", "Flights");  //change 
            }
        }




        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,FirstName,LastName,Email,EnteranceDate,PropertyCity,PropertyStreet,PropertyStreetNumber,ApartmentNumber,IsAdmin,UserName,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                var objUser = await _context.User.Where(u => u.Email.Equals(user.Email) || u.UserName.Equals(user.UserName)).ToListAsync();
                if (objUser.Count == 0)
                {
                    user.IsAdmin = false;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                ModelState.AddModelError("", "This Email address / username is already taken");
                }

            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,FirstName,LastName,Email,EnteranceDate,PropertyCity,PropertyStreet,PropertyStreetNumber,ApartmentNumber,IsAdmin,UserName,Password")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var objUser = await _context.User.Where(u => u.Email.Equals(user.Email) || u.UserName.Equals(user.UserName)).ToListAsync();
                if (objUser.Count == 0 || (objUser.Count == 1 && objUser[0].UserID.Equals(id)))
                {
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.UserID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "This Email address / username is already taken");
                }
            }
            return View();
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
        }
    }
}
