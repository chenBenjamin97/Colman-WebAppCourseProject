using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using WebAppProject.Data;
using WebAppProject.Models;

namespace WebAppProject.Controllers
{
    public class WaterTransactionsController : Controller
    {
        private readonly MvcProjectContext _context;

        public WaterTransactionsController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: WaterTransactions
        public async Task<IActionResult> Index()
        {
            var mvcProjectContext = _context.WaterTransactions.Include(w => w.User);
            return View(await mvcProjectContext.ToListAsync());
        }

        // GET: WaterTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterTransaction = await _context.WaterTransactions
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WaterMeterID == id);
            if (waterTransaction == null)
            {
                return NotFound();
            }

            // NOTICE: 
            ViewData["ImgPath"] = waterTransaction.ImgPath;
            return View(waterTransaction);
        }

        // GET: WaterTransactions/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID");
            return View();
        }

        // POST: WaterTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,WaterMeterLastReadDate,WaterMeterID,WaterMeterImg,ImgPath")] WaterTransaction waterTransaction)
        {
            if (ModelState.IsValid)
            {
                waterTransaction.UserID = (int)HttpContext.Session.GetInt32("UserID");

                string ImgPathAfterSave = Image.Save(waterTransaction.UserID, waterTransaction.WaterMeterImg, waterTransaction.WaterMeterID, "Water");
                if (ImgPathAfterSave == null) // Another file with same name is already exist - "UserID-WaterMeterID" has to be unique
                {
                    ModelState.AddModelError("", "Another transaction with the same User ID Water Meter ID is already exist.");
                    return View();
                }

                waterTransaction.Status = Config.TransactionStatus.Open; // Init new transaction status

                waterTransaction.ImgPath = ImgPathAfterSave;
                _context.Add(waterTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", waterTransaction.UserID);
            return View(waterTransaction);
        }

        // GET: WaterTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterTransaction = await _context.WaterTransactions.FindAsync(id);
            if (waterTransaction == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", waterTransaction.UserID);
            return View(waterTransaction);
        }

        // POST: WaterTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,WaterMeterLastReadDate,WaterMeterID,WaterMeterImg,ImgPath")] WaterTransaction waterTransactionAfterEdit)
        {
            var waterTransactionBeforeEdit = await _context.WaterTransactions.FindAsync(id);
            if (waterTransactionBeforeEdit == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (waterTransactionAfterEdit.WaterMeterImg != null)
                    {
                        string newImgRelativePath = Image.Edit(waterTransactionBeforeEdit.UserID, waterTransactionBeforeEdit.ImgPath, waterTransactionAfterEdit.WaterMeterImg, waterTransactionBeforeEdit.WaterMeterID, "Water");
                        waterTransactionBeforeEdit.ImgPath = newImgRelativePath;
                    }

                    waterTransactionBeforeEdit.WaterMeterLastReadDate = waterTransactionAfterEdit.WaterMeterLastReadDate;
                    _context.Update(waterTransactionBeforeEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterTransactionExists(waterTransactionAfterEdit.WaterMeterID))
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
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", waterTransactionAfterEdit.UserID);
            return View(waterTransactionAfterEdit);
        }

        // GET: WaterTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterTransaction = await _context.WaterTransactions
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WaterMeterID == id);
            if (waterTransaction == null)
            {
                return NotFound();
            }

            return View(waterTransaction);
        }

        // POST: WaterTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterTransaction = await _context.WaterTransactions.FindAsync(id);

            // Delete file from disk:
            Image.Delete(waterTransaction.ImgPath);

            // Delete transaction from DB:
            _context.WaterTransactions.Remove(waterTransaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WaterTransactionExists(int id)
        {
            return _context.WaterTransactions.Any(e => e.WaterMeterID == id);
        }
    }
}
