using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;

namespace WebAppProject.Controllers
{
    public class ContactApplicationsController : Controller
    {
        private readonly MvcProjectContext _context;

        public ContactApplicationsController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: ContactApplications
        public async Task<IActionResult> Index()
        {
            // Validate User logged in here

            if (HttpContext.Session.GetString("IsAdmin").Equals("true"))
            {
                return View(await _context.ContactApplication.ToListAsync());
            }
            else
            {
                var userID = HttpContext.Session.GetInt32("UserID");
                var allowedToSee = _context.ContactApplication.Where(u => u.UserID == userID);
                return View(await allowedToSee.ToListAsync());
            }
        }

        // GET: ContactApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactApplication = await _context.ContactApplication
                .FirstOrDefaultAsync(m => m.ContactAppID == id);
            if (contactApplication == null)
            {
                return NotFound();
            }

            // Notice:
            ViewData["ImgPath"] = contactApplication.ImgPath;
            return View(contactApplication);
        }

        // GET: ContactApplications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContactApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Message,ContactType,Img")] ContactApplication contactApplication)
        {
            if (ModelState.IsValid)
            {
                // Init new Application:
                contactApplication.UserID = (int)HttpContext.Session.GetInt32("UserID");
                contactApplication.CreateDate = DateTime.Today;
                contactApplication.Status = Config.TransactionStatus.Open;

                if (contactApplication.Img != null)
                {
                    _context.Add(contactApplication);
                    await _context.SaveChangesAsync(); // Let this contact application get ID from SQL server

                    contactApplication.ImgPath = Image.Save(contactApplication.UserID, contactApplication.Img, getLastContactAppRegisteredID(), "ContactApplication");

                    _context.Update(contactApplication);
                    await _context.SaveChangesAsync(); // Update image path to this contact app ID
                }
                else // (ID is not needed yet because we are not saving an image)
                {
                    contactApplication.ImgPath = "No Image Attached";

                    _context.Add(contactApplication);
                    await _context.SaveChangesAsync(); // let this contact application get ID from SQL server
                }
            }
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: ContactApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactApplication = await _context.ContactApplication.FindAsync(id);
            if (contactApplication == null)
            {
                return NotFound();
            }
            return View(contactApplication);
        }

        // POST: ContactApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Message,ContactType,Img,Status")] ContactApplication ContactAppAfterEdit)
        {
            var ContactAppBeforeEdit = await _context.ContactApplication.FindAsync(id);
            if (ContactAppBeforeEdit == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (ContactAppAfterEdit.Img != null)
                    {
                        string newImgRelativePath = Image.Edit(ContactAppBeforeEdit.UserID, ContactAppBeforeEdit.ImgPath, ContactAppAfterEdit.Img, ContactAppBeforeEdit.ContactAppID, "ContactApplication");
                        ContactAppBeforeEdit.ImgPath = newImgRelativePath;
                    }

                    ContactAppBeforeEdit.Status = ContactAppAfterEdit.Status;
                    ContactAppBeforeEdit.Title = ContactAppAfterEdit.Title;
                    ContactAppBeforeEdit.Message = ContactAppAfterEdit.Message;
                    ContactAppBeforeEdit.ContactType = ContactAppAfterEdit.ContactType;

                    _context.Update(ContactAppBeforeEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactApplicationExists(ContactAppAfterEdit.ContactAppID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: ContactApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactApplication = await _context.ContactApplication
                .FirstOrDefaultAsync(m => m.ContactAppID == id);
            if (contactApplication == null)
            {
                return NotFound();
            }

            return View(contactApplication);
        }

        // POST: ContactApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactApplication = await _context.ContactApplication.FindAsync(id);

            // Delete file from disk (if image attached to this application):
            if (contactApplication.ImgPath.Equals("No Image Attached") == false) // Means there is an image attached to this application
            {
                Image.Delete(contactApplication.ImgPath);
            }

            // Delete transaction from DB:
            _context.ContactApplication.Remove(contactApplication);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManagerOverview");
        }

        private bool ContactApplicationExists(int id)
        {
            return _context.ContactApplication.Any(e => e.ContactAppID == id);
        }

        private int getLastContactAppRegisteredID()
        {
            int lastRegisteredID = _context.ContactApplication.Max(app => app.ContactAppID);

            return lastRegisteredID;

        }
    }
}
