using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;

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
            // Validate User logged in here

            if (HttpContext.Session.GetString("IsAdmin").Equals("true"))
            {
                return View(await _context.User.ToListAsync());
            }
            else
            {
                var userID = HttpContext.Session.GetInt32("UserID");
                var allowedToSee = _context.User.Where(u => u.UserID == userID);
                return View(await allowedToSee.ToListAsync());
            }
        }

        /**********************************************   log in and register   **************************************************************/
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            HttpContext.Session.SetInt32("UserID", -1);
            HttpContext.Session.SetString("UserName", "null");
            HttpContext.Session.SetString("IsAdmin", "false");
            HttpContext.Session.SetString("FirstName", "null");

            ViewData["isHomePage"] = "true";

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {

                var objUser = _context.User.Where(u => u.UserName.Equals(username) && u.Password.Equals(password)); 
                    
                if (objUser.FirstOrDefault() != null) // Found user with matched Username AND Password
                {
                    if (objUser.FirstOrDefault().IsAdmin == false) //check if is not admin
                    {
                        HttpContext.Session.SetString("IsAdmin", "false");
                        HttpContext.Session.SetInt32("UserID", objUser.First().UserID);
                        HttpContext.Session.SetString("UserName", objUser.First().UserName.ToString());
                        HttpContext.Session.SetString("FirstName", objUser.First().FirstName.ToString());
                    } else {
                            HttpContext.Session.SetInt32("UserID", objUser.First().UserID);
                            HttpContext.Session.SetString("UserName", objUser.First().UserName.ToString());
                            HttpContext.Session.SetString("IsAdmin", "true");
                            HttpContext.Session.SetString("FirstName", objUser.First().FirstName.ToString());
                    }
                } else {
                    ModelState.AddModelError("", "Wrong usnername / password");

                    HttpContext.Session.SetInt32("UserID", -1);
                    HttpContext.Session.SetString("UserName", "null");
                    HttpContext.Session.SetString("IsAdmin", "false");
                    HttpContext.Session.SetString("FirstName", "null");

                    return View();
                }
            }

            return RedirectToAction("Index", "ManagerOverview"); // Good Login
        }


        public ActionResult LogOff()
        {
        HttpContext.Session.SetInt32("UserID", -1);
        HttpContext.Session.SetString("UserName", "null");
        HttpContext.Session.SetString("IsAdmin", "false");
        HttpContext.Session.SetString("FirstName", "null");
        return RedirectToAction("Login", "Users");
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
            ViewData["isHomePage"] = "true";

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
            user.UserID = id; // This is a disabled field. no one can edit this (not admins as well)

            ModelState.Remove("UserName"); // UserName Cannot be editted - a disabled field
            ModelState.Remove("UserID"); // UserID Cannot be editted - a disabled field

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    
                    HttpContext.Session.SetString("FirstName", user.FirstName);
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

                return RedirectToAction("Index", "ManagerOverview");
            }
            else
            {
                //ModelState.AddModelError("", "This Email address / username is already taken");

                //Throwing a message from User Validations if needed
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
            return RedirectToAction("Index", "ManagerOverview");
        }

        [HttpGet]
        public async Task<IActionResult> UserAddressMap(int id)
        {
            var user = await _context.User.FindAsync(id);
            
            var PropertyCity = user.PropertyCity;
            PropertyCity = PropertyCity.Trim();
            string PropertyCityForEmbedMap = PropertyCity.Replace(" ", "+");

            var PropertyStreet = user.PropertyStreet;
            PropertyStreet = PropertyStreet.Trim();
            string PropertyStreetForEmbedMap = PropertyStreet.Replace(" ", "+");

            var PropertyStreetNumber = user.PropertyStreetNumber.ToString();
            PropertyStreetNumber = PropertyStreetNumber.Trim();

            string FullUserAddressMapQueryFormat = string.Format("{0}+{1},{2}+Israel", PropertyStreetForEmbedMap, PropertyStreetNumber, PropertyCityForEmbedMap);

            string MapToUserAddressSrc = string.Format("https://www.google.com/maps/embed/v1/place?key={0}&q={1}", Config.GoogleMapsAPIKey, FullUserAddressMapQueryFormat);

            ViewData["MapIframeSrc"] = MapToUserAddressSrc;
            ViewData["RequestUserID"] = id;

            return View();
        }
        
        [HttpGet]
        public async Task<string> GetNearbyResturants(int id)
        {
            string responseBody = null;
            string radius = "1500"; // meters

            var user = await _context.User.FindAsync(id);

            var PropertyCity = user.PropertyCity;
            PropertyCity = PropertyCity.Trim();
            string PropertyCityForGeoLocation = PropertyCity.Replace(" ", "+");

            var PropertyStreet = user.PropertyStreet;
            PropertyStreet = PropertyStreet.Trim();
            string PropertyStreetForGeoLocation = PropertyStreet.Replace(" ", "+");

            var PropertyStreetNumber = user.PropertyStreetNumber.ToString();
            PropertyStreetNumber = PropertyStreetNumber.Trim();

            string FullUserAddressGeoLocationFormat = string.Format("{0}+{1}+{2}+Israel", PropertyStreetForGeoLocation, PropertyStreetNumber, PropertyCityForGeoLocation);

            string UserAddressGeoLocation = await GeoCodeAddress(FullUserAddressGeoLocationFormat);

            string NearbyResturantsSrc = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0}&radius={1}&type=restaurant&key={2}", UserAddressGeoLocation, radius, Config.GoogleMapsAPIKey);

            HttpResponseMessage response = await Config.APIHttpClient.GetAsync(NearbyResturantsSrc);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }

            return responseBody;
        }

        public async Task<string> GeoCodeAddress(string queryAddress)
        {
            string GeoCodeAddressURL = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", queryAddress, Config.GoogleMapsAPIKey);

            HttpResponseMessage response = await Config.APIHttpClient.GetAsync(GeoCodeAddressURL);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                JObject responseBodyJSON = JObject.Parse(responseBody);

                string lat = (string)responseBodyJSON.SelectToken("results[0].geometry.location.lat");
                string lng = (string)responseBodyJSON.SelectToken("results[0].geometry.location.lng");

                return string.Format("{0},{1}", lat, lng);
            }

            return null;
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
        }
    }
}
