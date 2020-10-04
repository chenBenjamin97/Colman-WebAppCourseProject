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
    public class ElectricityTransactionsController : Controller
    {
        private readonly MvcProjectContext _context;

        public ElectricityTransactionsController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: ElectricityTransactions
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("IsAdmin").Equals("true"))
            {
                var mvcProjectContext = _context.ElectricityTransactions.Include(e => e.User);
                return View(await mvcProjectContext.ToListAsync());
            }
            else
            {
                var userID = HttpContext.Session.GetInt32("UserID");
                var allowedToSee = _context.ElectricityTransactions.Where(u => u.UserID == userID);
                return View(await allowedToSee.ToListAsync());
            }
        }

        // GET: ElectricityTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var electricityTransaction = await _context.ElectricityTransactions
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ElectricityMeterID == id);
            if (electricityTransaction == null)
            {
                return NotFound();
            }
             // Notice:
            ViewData["ImgPath"] = electricityTransaction.ImgPath;
            return View(electricityTransaction);
        }

        // GET: ElectricityTransactions/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID");
            return View();
        }

        // POST: ElectricityTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,ElectricityMeterLastRead,ElectricityMeterID,ElectricityMeterImg,ImgPath")] ElectricityTransaction electricityTransaction)
        {
            if (ModelState.IsValid)
            {
                electricityTransaction.UserID = (int)HttpContext.Session.GetInt32("UserID");

                string ImgPathAfterSave = Image.Save(electricityTransaction.UserID, electricityTransaction.ElectricityMeterImg, electricityTransaction.ElectricityMeterID, "Electricity");
                if (ImgPathAfterSave == null) // Another file with same name is already exist - "UserID-ElectricityMeterID" has to be unique
                {
                    ModelState.AddModelError("", "Another transaction with the same User ID Water Meter ID is already exist.");
                    return View();
                }

                electricityTransaction.Status = Config.TransactionStatus.Open; // Init new transaction status

                electricityTransaction.ImgPath = ImgPathAfterSave;
                _context.Add(electricityTransaction);
                await _context.SaveChangesAsync();
            }

            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", electricityTransaction.UserID);
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: ElectricityTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var electricityTransaction = await _context.ElectricityTransactions.FindAsync(id);
            if (electricityTransaction == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", electricityTransaction.UserID);
            return View(electricityTransaction);
        }

        // POST: ElectricityTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,ElectricityMeterLastRead,ElectricityMeterID,ElectricityMeterImg,Status")] ElectricityTransaction electricityTransactionAfterEdit)
        {
            var ElectricityTransactionBeforeEdit = await _context.ElectricityTransactions.FindAsync(id);
            if (ElectricityTransactionBeforeEdit == null)
            {
                return NotFound();
            }

            ModelState.Remove("ElectricityMeterID"); // Disabled field, can't be editted

            if (ModelState.IsValid)
            {
                try
                {
                    if (electricityTransactionAfterEdit.ElectricityMeterImg != null)
                    {
                        string newImgRelativePath = Image.Edit(ElectricityTransactionBeforeEdit.UserID, ElectricityTransactionBeforeEdit.ImgPath, electricityTransactionAfterEdit.ElectricityMeterImg, ElectricityTransactionBeforeEdit.ElectricityMeterID, "Electricity");
                        ElectricityTransactionBeforeEdit.ImgPath = newImgRelativePath;
                    }

                    ElectricityTransactionBeforeEdit.ElectricityMeterLastRead = electricityTransactionAfterEdit.ElectricityMeterLastRead;
                    ElectricityTransactionBeforeEdit.Status = electricityTransactionAfterEdit.Status;
                    _context.Update(ElectricityTransactionBeforeEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ElectricityTransactionExists(electricityTransactionAfterEdit.ElectricityMeterID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", electricityTransactionAfterEdit.UserID);
            return RedirectToAction("Index", "ManagerOverview");
        }

        // GET: ElectricityTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var electricityTransaction = await _context.ElectricityTransactions
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ElectricityMeterID == id);
            if (electricityTransaction == null)
            {
                return NotFound();
            }

            return View(electricityTransaction);
        }

        // POST: ElectricityTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var electricityTransaction = await _context.ElectricityTransactions.FindAsync(id);

            // Delete file from disk:
            try
            {
            Image.Delete(electricityTransaction.ImgPath);
            } catch
            {
                // Logic Error, Transaction With No Image
            }

            // Delete transaction from DB:
            _context.ElectricityTransactions.Remove(electricityTransaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManagerOverview");
        }

        private bool ElectricityTransactionExists(int id)
        {
            return _context.ElectricityTransactions.Any(e => e.ElectricityMeterID == id);
        }
    }
}
