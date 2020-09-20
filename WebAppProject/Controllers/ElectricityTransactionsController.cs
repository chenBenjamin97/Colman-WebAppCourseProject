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
            var mvcProjectContext = _context.ElectricityTransactions.Include(e => e.User);
            return View(await mvcProjectContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("UserID,ElectricityMeterLastRead,ElectricityMeterID,ImgPath")] ElectricityTransaction electricityTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(electricityTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", electricityTransaction.UserID);
            return View(electricityTransaction);
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
        public async Task<IActionResult> Edit(int id, [Bind("UserID,ElectricityMeterLastRead,ElectricityMeterID,ImgPath")] ElectricityTransaction electricityTransaction)
        {
            if (id != electricityTransaction.ElectricityMeterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(electricityTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ElectricityTransactionExists(electricityTransaction.ElectricityMeterID))
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
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "UserID", electricityTransaction.UserID);
            return View(electricityTransaction);
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
            _context.ElectricityTransactions.Remove(electricityTransaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ElectricityTransactionExists(int id)
        {
            return _context.ElectricityTransactions.Any(e => e.ElectricityMeterID == id);
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
