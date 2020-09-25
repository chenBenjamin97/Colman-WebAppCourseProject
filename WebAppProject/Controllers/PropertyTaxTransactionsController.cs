using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;

namespace WebAppProject.Controllers
{
    public class PropertyTaxController : Controller
    {
        private readonly MvcProjectContext _context;

        public PropertyTaxController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: PropertyTaxes
        public async Task<IActionResult> Index()
        {
            var mvcProjectContext = _context.PropertyTaxTransactions.Include(p => p.User);
            return View(await mvcProjectContext.ToListAsync());
        }

        // GET: PropertyTaxes/Details/5
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

            return View(propertyTax);
        }

        // GET: PropertyTaxes/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID");
            return View();
        }

        // POST: PropertyTaxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,PropertyID,ImgPath")] PropertyTaxTransaction propertyTax)
        {

            if (ModelState.IsValid)
            {
                propertyTax.Status = Config.TransactionStatus.Open;
                _context.Add(propertyTax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", propertyTax.UserID);
            return View(propertyTax);
        }

        // GET: PropertyTaxes/Edit/5
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

        // POST: PropertyTaxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,PropertyID,ImgPath")] PropertyTaxTransaction propertyTax)
        {
            if (id != propertyTax.PropertyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyTax);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyTaxExists(propertyTax.PropertyID))
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
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", propertyTax.UserID);
            return View(propertyTax);
        }

        // GET: PropertyTaxes/Delete/5
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

        // POST: PropertyTaxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyTax = await _context.PropertyTaxTransactions.FindAsync(id);
            _context.PropertyTaxTransactions.Remove(propertyTax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyTaxExists(int id)
        {
            return _context.PropertyTaxTransactions.Any(e => e.PropertyID == id);
        }
        public IActionResult CreateNewTransaction()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewTransaction(WaterTransaction userInput)
        {
            string newFileName = userInput.UserID.ToString() + Path.GetExtension(userInput.WaterMeterImg.FileName);

            string PhysicalAddressOfSaveNewFile = Path.Combine(Config.PhysicalWaterFilesPath, newFileName);
            userInput.WaterMeterImg.CopyTo(new FileStream(PhysicalAddressOfSaveNewFile, FileMode.Create));

            string RelativeAddressOfNewFile = Path.Combine(Config.RelativeWaterFilesPath, newFileName);
            WaterTransaction insertToDb = new WaterTransaction { UserID = userInput.UserID, ImgPath = RelativeAddressOfNewFile, WaterMeterID = userInput.WaterMeterID, WaterMeterLastReadDate = userInput.WaterMeterLastReadDate };

            _context.WaterTransactions.Add(insertToDb);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
