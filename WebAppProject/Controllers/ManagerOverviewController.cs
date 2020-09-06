using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;

namespace WebAppProject.Controllers
{
    public class ManagerOverviewController : Controller
    {
        private readonly MvcProjectContext _context;

        public ManagerOverviewController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: ManagerOverview
        public async Task<IActionResult> Index()
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            ViewModel viewModel = new ViewModel
            {
                Users = await _context.User.ToListAsync(),
                WaterTransactions = await _context.WaterTransactions.ToListAsync(),
                ElectricityTransaction = await _context.ElectricityTransactions.ToListAsync()
            };
            return View(viewModel);
        }

        // GET: ManagerOverview/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: ManagerOverview/Create
        public IActionResult Create()
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            return View();
        }

        // POST: ManagerOverview/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID")] ViewModel viewModel)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (ModelState.IsValid)
            {
                _context.Add(viewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: ManagerOverview/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel.FindAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: ManagerOverview/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID")] ViewModel viewModel)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id != viewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViewModelExists(viewModel.ID))
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
            return View(viewModel);
        }

        // GET: ManagerOverview/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: ManagerOverview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            var viewModel = await _context.ViewModel.FindAsync(id);
            _context.ViewModel.Remove(viewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViewModelExists(int id)
        {
            return _context.ViewModel.Any(e => e.ID == id);
        }

        public ActionResult ValidateAdmin (HttpContext context)
        {
            return context.Session.GetString("IsAdmin") == "true" ? null : RedirectToAction("Index", "Home");
        }
    }
}
