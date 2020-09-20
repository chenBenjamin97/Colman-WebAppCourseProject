using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Create([Bind("UserID,WaterMeterLastReadDate,WaterMeterID,ImgPath")] WaterTransaction waterTransaction)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(int id, [Bind("UserID,WaterMeterLastReadDate,WaterMeterID,ImgPath")] WaterTransaction waterTransaction)
        {
            if (id != waterTransaction.WaterMeterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterTransactionExists(waterTransaction.WaterMeterID))
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
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", waterTransaction.UserID);
            return View(waterTransaction);
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
            var fileNameSplitted = waterTransaction.ImgPath.Split('.');
            var userID = waterTransaction.UserID;
            var fileExtension = fileNameSplitted[fileNameSplitted.Length - 1];
            var fileToDeletePath = Path.Combine(Config.PhysicalWaterFilesPath, userID + "." + fileExtension);
            System.IO.File.Delete(fileToDeletePath);

            // Delete transaction from DB:
            _context.WaterTransactions.Remove(waterTransaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WaterTransactionExists(int id)
        {
            return _context.WaterTransactions.Any(e => e.WaterMeterID == id);
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
