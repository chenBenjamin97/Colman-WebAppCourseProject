using System;
using System.Collections.Generic;
using System.IO;
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
    public class PropertyTaxTransactionsController : Controller
    {
        private readonly MvcProjectContext _context;

        public PropertyTaxTransactionsController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: PropertTaxTransactions
        public async Task<IActionResult> Index()
        {

            if (HttpContext.Session.GetString("IsAdmin").Equals("true"))
            {
                var mvcProjectContext = _context.PropertyTaxTransactions.Include(p => p.User);
                return View(await mvcProjectContext.ToListAsync());
            }
            else
            {
                var userID = HttpContext.Session.GetInt32("UserID");
                var allowedToSee = _context.User.Where(u => u.UserID == userID);
                return View(await allowedToSee.ToListAsync());
            }
        }

        // GET: PropertTaxTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTax = await _context.PropertyTaxTransactions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PropertyID == id);
            if (propertyTax == null)
            {
                return NotFound();
            }

            // NOTICE: 
            ViewData["ImgPath"] = propertyTax.ImgPath;
            return View(propertyTax);
        }

        // GET: PropertTaxTransactions/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID");
            return View();
        }

        // POST: PropertyTaxTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,PropertyID,PropertyTaxContractImg,ImgPath")] PropertyTaxTransaction propertyTax)
        {
            if (ModelState.IsValid)
            {
                propertyTax.UserID = (int)HttpContext.Session.GetInt32("UserID");

                string ImgPathAfterSave = Image.Save(propertyTax.UserID, propertyTax.PropertyTaxContractImg, propertyTax.PropertyID, "PropertyTax");
                if (ImgPathAfterSave == null) // Another file with same name is already exist - "UserID-WaterMeterID" has to be unique
                {
                    ModelState.AddModelError("", "Another transaction with the same User ID Water Meter ID is already exist.");
                    return View();
                }

                propertyTax.Status = Config.TransactionStatus.Open; // Init new transaction status

                propertyTax.ImgPath = ImgPathAfterSave;
                _context.Add(propertyTax);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ManagerOverview");
            }

            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", propertyTax.UserID);
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: PropertTaxTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTax = await _context.PropertyTaxTransactions.FindAsync(id);
            if (propertyTax == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", propertyTax.UserID);
            return View(propertyTax);
        }

        // POST: PropertTaxTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,PropertyID,PropertyTaxContractImg,ImgPath,Status")] PropertyTaxTransaction propertyTaxTransactionAfterEdit)
        {
            var propertyTaxTransactionBeforeEdit = await _context.PropertyTaxTransactions.FindAsync(id);
            if (propertyTaxTransactionBeforeEdit == null)
            {
                return NotFound();
            }

            ModelState.Remove("PropertyID"); // Disabled field, can't be editted

            if (ModelState.IsValid)
            {
                try
                {
                    if (propertyTaxTransactionAfterEdit.PropertyTaxContractImg != null)
                    {
                        string newImgRelativePath = Image.Edit(propertyTaxTransactionBeforeEdit.UserID, propertyTaxTransactionBeforeEdit.ImgPath, propertyTaxTransactionAfterEdit.PropertyTaxContractImg, propertyTaxTransactionBeforeEdit.PropertyID, "PropertyTax");
                        propertyTaxTransactionBeforeEdit.ImgPath = newImgRelativePath;
                    }

                    propertyTaxTransactionBeforeEdit.Status = propertyTaxTransactionAfterEdit.Status;
                    _context.Update(propertyTaxTransactionBeforeEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyTaxExists(propertyTaxTransactionAfterEdit.PropertyID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", propertyTaxTransactionAfterEdit.UserID);
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: PropertTaxTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTax = await _context.PropertyTaxTransactions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PropertyID == id);
            if (propertyTax == null)
            {
                return NotFound();
            }

            return View(propertyTax);
        }

        // POST: PropertTaxTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertTaxTransaction = await _context.PropertyTaxTransactions.FindAsync(id);

            // Delete file from disk:
            Image.Delete(propertTaxTransaction.ImgPath);

            // Delete transaction from DB:
            _context.PropertyTaxTransactions.Remove(propertTaxTransaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManagerOverview");
        }

        private bool PropertyTaxExists(int id)
        {
            return _context.PropertyTaxTransactions.Any(e => e.PropertyID == id);
        }
    }
}
