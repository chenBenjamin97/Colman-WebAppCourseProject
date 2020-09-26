using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View(await _context.ContactApplication.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("ContactAppID,Title,Message,ContactType,CreateDate,ImgPath,Status")] ContactApplication contactApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contactApplication);
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
        public async Task<IActionResult> Edit(int id, [Bind("ContactAppID,Title,Message,ContactType,CreateDate,ImgPath,Status")] ContactApplication contactApplication)
        {
            if (id != contactApplication.ContactAppID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactApplicationExists(contactApplication.ContactAppID))
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
            return View(contactApplication);
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
            _context.ContactApplication.Remove(contactApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactApplicationExists(int id)
        {
            return _context.ContactApplication.Any(e => e.ContactAppID == id);
        }
    }
}
