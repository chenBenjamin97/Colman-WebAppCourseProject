using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using Newtonsoft.Json;
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
            // Validate logged in here

            ViewModel viewModel = new ViewModel();

            if (isAdmin(this.HttpContext))
            { // Admin
                viewModel.Users = await _context.User.ToListAsync();
                viewModel.WaterTransactions = await _context.WaterTransactions.ToListAsync();
                viewModel.ElectricityTransaction = await _context.ElectricityTransactions.ToListAsync();
                viewModel.PropertyTaxTransactions = await _context.PropertyTaxTransactions.ToListAsync();
                viewModel.ContactApplications = await _context.ContactApplication.ToListAsync();
            } else
            { // Not Admin
                var sessionUserID = HttpContext.Session.GetInt32("UserID");

                viewModel.Users = await _context.User.Where(u => u.UserID == sessionUserID).ToListAsync();
                viewModel.WaterTransactions = await _context.WaterTransactions.Where(u => u.UserID == sessionUserID).ToListAsync();
                viewModel.ElectricityTransaction = await _context.ElectricityTransactions.Where(u => u.UserID == sessionUserID).ToListAsync();
                viewModel.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(u => u.UserID == sessionUserID).ToListAsync();
                viewModel.ContactApplications = await _context.ContactApplication.Where(u => u.UserID == sessionUserID).ToListAsync();
            }

            return View(viewModel);
        }

        // GET: ManagerOverview/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (ValidateAdmin(this.HttpContext) != null)
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

        [HttpPost]
        public async Task<IActionResult> SearchAndResult(string SearchDB, string SearchCatagory, Config.TransactionStatus? wantedStatusWater, Config.TransactionStatus? wantedStatusElectricity, Config.TransactionStatus? wantedStatusPropertyTax, Config.TransactionStatus? wantedStatusContactApp, Config.ContactAppType? ContactType, string? SearchKeyWord)
        {
            ViewData["AfterSearch"] = true;

            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            ViewModel model = new ViewModel();

            // Init:
            model.Users = null;
            model.WaterTransactions = null;
            model.ElectricityTransaction = null;
            model.PropertyTaxTransactions = null;
            model.ContactApplications = null;

            switch (SearchDB)
            {
                case "User":
                    if (SearchCatagory.Equals("UserID"))
                    {
                       model.Users = await _context.User.Where(user => user.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }   
                    else if (SearchCatagory.Equals("FirstName"))
                    {
                        model.Users = await _context.User.Where(user => user.FirstName.Contains(SearchKeyWord)).ToListAsync();
                    } 
                    else if (SearchCatagory.Equals("LastName"))
                    {
                        model.Users = await _context.User.Where(user => user.LastName.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Email"))
                    {
                        model.Users = await _context.User.Where(user => user.Email.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("City"))
                    {
                        model.Users = await _context.User.Where(user => user.PropertyCity.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("EnteranceDate")) {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.Users = await _context.User.Where(user => user.EnteranceDate.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("IsAdmin"))
                    {
                        model.Users = await _context.User.Where(user => user.IsAdmin.Equals(true)).ToListAsync();
                    }

                    break;

                case "WaterTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    } 
                    else if (SearchCatagory.Equals("WaterMeterID"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.WaterMeterID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterLastReadDate"))
                    {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.WaterMeterLastReadDate.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Status"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.Status == wantedStatusWater).ToListAsync();
                    }

                    break;

                case "ElectricityTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterID"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.ElectricityMeterID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterLastReadDate"))
                    {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.ElectricityMeterLastRead.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Status"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.Status == wantedStatusElectricity).ToListAsync();
                    }

                    break;

                case "PropertyTaxTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                       model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("PropertyID"))
                    {
                        model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.PropertyID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Status"))
                    {
                        model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status == wantedStatusPropertyTax).ToListAsync();
                    }

                    break;

                case "ContactApplication":
                    if (SearchCatagory.Equals("UserID"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ApplicationID"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.ContactAppID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ApplicationTitle"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.Title.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ApplicationMessage"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.Message.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("CreationDate"))
                    {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.CreateDate.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ApplicationType"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.ContactType == ContactType).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Status"))
                    {
                        model.ContactApplications = await _context.ContactApplication.Where(app => app.Status == wantedStatusContactApp).ToListAsync();
                    }

                    break;

                default:
                    break;
            }


            return View(model);
        }

        public async Task<IActionResult> Stats()
        {
            // Contact Applications Group By Transaction Type:
            var allContactApps = await _context.ContactApplication.ToListAsync();
            
            var queryTransactionTypes =
                from app in allContactApps
                group app by app.ContactType into newGroup
                orderby newGroup.Key
                select newGroup;

            var ContactAppGroupByWaterJSON = new ContactAppsTypesJSON("Water", 0);         
            var ContactAppGroupByElectricityJSON = new ContactAppsTypesJSON("Electricity", 0);          
            var ContactAppGroupByPropertyTaxJSON = new ContactAppsTypesJSON("Property Tax", 0);

            foreach (var currentKey in queryTransactionTypes)
            {
                switch (currentKey.Key)
                {
                    case Config.ContactAppType.WaterTransaction:
                        ContactAppGroupByWaterJSON.value = currentKey.Count();
                        break;
                    case Config.ContactAppType.ElectricityTransaction:
                        ContactAppGroupByElectricityJSON.value = currentKey.Count();
                        break;
                    case Config.ContactAppType.PropertyTaxTransaction:
                        ContactAppGroupByPropertyTaxJSON.value = currentKey.Count();
                        break;

                    default:
                        break;
                }
            }

            ContactAppsTypesJSON[] ContactAppsGroupByResults = { ContactAppGroupByWaterJSON, ContactAppGroupByElectricityJSON, ContactAppGroupByPropertyTaxJSON };

            ViewData["JSONContactAppsAfterGroupByTransactionTypes"] = JsonConvert.SerializeObject(ContactAppsGroupByResults, Formatting.Indented);

            // Users Group By Enterance Month:
            var allUsers = await _context.User.ToListAsync();
            var UsersMonthsAfterGroupBy = new MonthsWithValueJSON();
            
            var queryUsersEnterenceMonth =
                from user in allUsers
                group user by user.EnteranceDate.Month into newGroup
                orderby newGroup.Key
                select newGroup;

            foreach (var currentKey in queryUsersEnterenceMonth)
            {
                switch (currentKey.Key)
                {
                    case 1:
                        UsersMonthsAfterGroupBy.January = currentKey.Count();
                        break;
                    case 2:
                        UsersMonthsAfterGroupBy.February = currentKey.Count();
                        break;
                    case 3:
                        UsersMonthsAfterGroupBy.March = currentKey.Count();
                        break;
                    case 4:
                        UsersMonthsAfterGroupBy.April = currentKey.Count();
                        break;
                    case 5:
                        UsersMonthsAfterGroupBy.May = currentKey.Count();
                        break;
                    case 6:
                        UsersMonthsAfterGroupBy.June = currentKey.Count();
                        break;
                    case 7:
                        UsersMonthsAfterGroupBy.July = currentKey.Count();
                        break;
                    case 8:
                        UsersMonthsAfterGroupBy.August = currentKey.Count();
                        break;
                    case 9:
                        UsersMonthsAfterGroupBy.September = currentKey.Count();
                        break;
                    case 10:
                        UsersMonthsAfterGroupBy.October = currentKey.Count();
                        break;
                    case 11:
                        UsersMonthsAfterGroupBy.November = currentKey.Count();
                        break;
                    case 12:
                        UsersMonthsAfterGroupBy.December = currentKey.Count();
                        break;

                    default:
                        break;
                }
            }

            ViewData["UsersAfterGroupByEnteranceMonth"] = JsonConvert.SerializeObject(UsersMonthsAfterGroupBy, Formatting.Indented);

            return View();
        }

        [HttpGet]
        public IActionResult SearchAndResult()
        {
            ViewData["AfterSearch"] = false;

            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            return View();
        }

        [HttpGet]
        public IActionResult Article()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JoinSearchAndResult(string? WaterCheckBox, string? ElectricityCheckBox, string? PropertyTaxCheckBox, 
            Config.TransactionStatus? WaterwantedStatus, Config.TransactionStatus? ElectricitywantedStatus, Config.TransactionStatus? PropertyTaxwantedStatus)
        {
            ViewModel model = new ViewModel();

            // Init:
            model.Users = null;
            model.WaterTransactions = null;
            model.ElectricityTransaction = null;
            model.PropertyTaxTransactions = null;
            model.ContactApplications = null;

            if (WaterCheckBox != null && ElectricityCheckBox == null && PropertyTaxCheckBox == null)
            {
                var allWaterOpenTransactions = await _context.WaterTransactions.Where(water => water.Status == WaterwantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                var UserJoinResult =
                    from transaction in allWaterOpenTransactions
                    join user in allUsers
                    on transaction.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Water Transaction: ", WaterwantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox == null && ElectricityCheckBox != null && PropertyTaxCheckBox == null)
            {
                var allElectricityOpenTransactions = await _context.ElectricityTransactions.Where(electricity => electricity.Status == ElectricitywantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                var UserJoinResult =
                    from transaction in allElectricityOpenTransactions
                    join user in allUsers
                    on transaction.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Electricity Transaction: ", ElectricitywantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox == null && ElectricityCheckBox == null && PropertyTaxCheckBox != null)
            {
                var allPropertyTaxOpenTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status == PropertyTaxwantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                var UserJoinResult =
                     from transaction in allPropertyTaxOpenTransactions
                     join user in allUsers
                     on transaction.UserID equals user.UserID into result
                     select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Property Tax Transaction: ", PropertyTaxwantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox != null && ElectricityCheckBox != null && PropertyTaxCheckBox == null)
            {
                var allWaterOpenTransactions = await _context.WaterTransactions.Where(water => water.Status == WaterwantedStatus).ToListAsync();
                var allElectricityOpenTransactions = await _context.ElectricityTransactions.Where(electricity => electricity.Status == ElectricitywantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                // Get all UserID who have both water transaction as wanted && electricity transaction as wanted
                var transactionJoinUserIDResult =
                    from waterTransaction in allWaterOpenTransactions
                    join electricityTransaction in allElectricityOpenTransactions
                    on waterTransaction.UserID equals electricityTransaction.UserID into result
                    select result.Distinct();

                var TransactionsFromResult = transactionJoinUserIDResult.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get User Object by UserID
                var UserJoinResult =
                    from foundTransactions in TransactionsFromResult
                    join user in allUsers
                    on foundTransactions.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Water Transaction And {1} Electricity Transaction: ", WaterwantedStatus, ElectricitywantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox != null && ElectricityCheckBox == null && PropertyTaxCheckBox != null)
            {
                var allWaterOpenTransactions = await _context.WaterTransactions.Where(water => water.Status == WaterwantedStatus).ToListAsync();
                var allPropertyTaxOpenTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status == PropertyTaxwantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                // Get all UserID who have both propertyTax transaction as wanted && water transaction as wanted
                var transactionJoinUserIDResult =
                    from waterTransaction in allWaterOpenTransactions
                    join propertyTaxTransaction in allPropertyTaxOpenTransactions
                    on waterTransaction.UserID equals propertyTaxTransaction.UserID into result
                    select result.Distinct();

                var TransactionsFromResult = transactionJoinUserIDResult.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get User Object by UserID
                var UserJoinResult =
                    from foundTransactions in TransactionsFromResult
                    join user in allUsers
                    on foundTransactions.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Water Transaction And {1} Property Tax Transaction: ", WaterwantedStatus, PropertyTaxwantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox == null && ElectricityCheckBox != null && PropertyTaxCheckBox != null)
            {
                var allElectricityOpenTransactions = await _context.ElectricityTransactions.Where(electricity => electricity.Status == ElectricitywantedStatus).ToListAsync();
                var allPropertyTaxOpenTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status == PropertyTaxwantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                // Get all UserID who have both propertyTax transaction as wanted && electricity transaction as wanted
                var transactionJoinUserIDResult =
                    from electricityTransaction in allElectricityOpenTransactions
                    join propertyTaxTransaction in allPropertyTaxOpenTransactions
                    on electricityTransaction.UserID equals propertyTaxTransaction.UserID into result
                    select result.Distinct();

                var TransactionsFromResult = transactionJoinUserIDResult.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get User Object by UserID
                var UserJoinResult =
                    from foundTransactions in TransactionsFromResult
                    join user in allUsers
                    on foundTransactions.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Electricity Transaction And {1} Property Tax Transaction: ", ElectricitywantedStatus, PropertyTaxwantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            if (WaterCheckBox != null && ElectricityCheckBox != null && PropertyTaxCheckBox != null)
            {
                var allElectricityOpenTransactions = await _context.ElectricityTransactions.Where(electricity => electricity.Status == ElectricitywantedStatus).ToListAsync();
                var allPropertyTaxOpenTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status == PropertyTaxwantedStatus).ToListAsync();
                var allWaterOpenTransactions = await _context.WaterTransactions.Where(water => water.Status == WaterwantedStatus).ToListAsync();
                var allUsers = await _context.User.ToListAsync();

                // Get all UserID who have both propertyTax transaction as wanted && electricity transaction as wanted
                var transactionJoinUserIDResultFirstJoin =
                    from electricityTransaction in allElectricityOpenTransactions
                    join propertyTaxTransaction in allPropertyTaxOpenTransactions
                    on electricityTransaction.UserID equals propertyTaxTransaction.UserID into result
                    select result.Distinct();

                var PropertyTaxAndElectricityOpen = transactionJoinUserIDResultFirstJoin.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get all UserID who have both water transaction as wanted && electricity transaction as wanted
                var transactionJoinUserIDResult =
                    from waterTransaction in allWaterOpenTransactions
                    join electricityTransaction in allElectricityOpenTransactions
                    on waterTransaction.UserID equals electricityTransaction.UserID into result
                    select result.Distinct();

                var WaterAndElectricityOpenSecondJoin = transactionJoinUserIDResult.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get Electricity Transactions of users who have all types of transaction with as wanted status
                var allFoundUserIDsResult =
                    from transactionsFirstJoin in PropertyTaxAndElectricityOpen
                    join transactionsSecondJoin in WaterAndElectricityOpenSecondJoin
                    on transactionsFirstJoin.UserID equals transactionsSecondJoin.UserID into result
                    select result.Distinct();

                var UsersWithAllTransactionsTypesOpen = allFoundUserIDsResult.SelectMany(x => x); // Using Select Many in order to flat from IEnumerable<IEnumerable<TransactionType>> to IEnumerable<TransactionType>

                // Get User Object by UserID
                var UserJoinResult =
                    from transaction in UsersWithAllTransactionsTypesOpen
                    join user in allUsers
                    on transaction.UserID equals user.UserID into result
                    select result;

                ViewData["SearchType"] = string.Format("All Users Who Have {0} Electricity Transaction, {1} Property Tax Transaction And {2} Water Transaction:", ElectricitywantedStatus, PropertyTaxwantedStatus, WaterwantedStatus);
                model.Users = UserJoinResult.Distinct().SelectMany(x => x).ToList(); // Using Select Many in order to flat from IEnumerable<IEnumerable<int>> to IEnumerable<int> and than to List<int>
            }

            return View(model);
        }

        //Search by a number of parameters
        [HttpPost]
        public async Task<IActionResult> UsersAdvancedSearch(string? FirstNameCheckBox, string? LastNameCheckBox, string? CityCheckBox,
            string? wantedFirstName, string? wantedLastName, string? wantedCity)
        {
            ViewModel model = new ViewModel();

            // Init:
            model.Users = null;
            model.WaterTransactions = null;
            model.ElectricityTransaction = null;
            model.PropertyTaxTransactions = null;
            model.ContactApplications = null;

            var allUsers = await _context.User.ToListAsync();
            IEnumerable<User> result = new List<User>();

            if (FirstNameCheckBox != null && LastNameCheckBox == null && CityCheckBox == null)
            {
                result = from u in allUsers
                              where u.FirstName.Contains(wantedFirstName)
                              select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their First Name Contains \"{0}\":", wantedFirstName);
            }

            if (FirstNameCheckBox == null && LastNameCheckBox != null && CityCheckBox == null)
            {
                result = from u in allUsers
                         where u.LastName.Contains(wantedLastName)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their Last Name Contains \"{0}\":", wantedLastName);
            }

            if (FirstNameCheckBox == null && LastNameCheckBox == null && CityCheckBox != null)
            {
                result = from u in allUsers
                         where u.PropertyCity.Contains(wantedCity)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their Property's City Name Contains \"{0}\":", wantedCity);
            }

            if (FirstNameCheckBox != null && LastNameCheckBox != null && CityCheckBox == null)
            {
                result = from u in allUsers
                         where u.FirstName.Contains(wantedFirstName) && u.LastName.Contains(wantedLastName)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their First Name Contains \"{0}\" And Their Last Name Contains \"{1}\":", wantedFirstName, wantedLastName);
            }

            if (FirstNameCheckBox != null && LastNameCheckBox == null && CityCheckBox != null)
            {
                result = from u in allUsers
                         where u.FirstName.Contains(wantedFirstName) && u.PropertyCity.Contains(wantedCity)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their First Name Contains \"{0}\" And Their Property's City Name Contains \"{1}\":", wantedFirstName, wantedCity);
            }

            if (FirstNameCheckBox == null && LastNameCheckBox != null && CityCheckBox != null)
            {
                result = from u in allUsers
                         where u.LastName.Contains(wantedLastName) && u.PropertyCity.Contains(wantedCity)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their Last Name Contains \"{0}\" And Their Property's City Name Contains \"{1}\":", wantedLastName, wantedCity);
            }

            if (FirstNameCheckBox != null && LastNameCheckBox != null && CityCheckBox != null)
            {
                result = from u in allUsers
                         where u.FirstName.Contains(wantedFirstName) && u.LastName.Contains(wantedLastName) && u.PropertyCity.Contains(wantedCity)
                         select u;

                ViewData["SearchResultHeader"] = string.Format("Users Who Their First Name Contains \"{0}\",Users Who Their Last Name Contains \"{1}\" And Their Property's City Name Contains \"{2}\":", wantedFirstName, wantedLastName, wantedCity);
            }

            model.Users = result.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult JoinSearchAndResult()
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            return View();
        }

        public async Task<IActionResult> Predications()
        {
            var MlContext = new MLContext();

            var allUsers = await _context.User.ToListAsync();
            var UsersDataDict = GetDataDict(allUsers);
            int csvLineCount;

            using (var stream = new StreamReader(Config.CsvFilePath))
            {
                int i = 0;
                while (stream.ReadLine() != null) { i++; }

                csvLineCount = i;
            }

            var data = MlContext.Data.LoadFromTextFile<DataStructML>(Config.CsvFilePath,
                hasHeader: false, separatorChar: ',');

            var pipeline = MlContext.Forecasting.ForecastBySsa(
                nameof(CountForecast.ForeCast), // Output column name
                nameof(DataStructML.Count), //Input column name
                windowSize: (int)((csvLineCount/2) - 1), // The length of the window on the series for building the trajectory matrix, must be at least 2 and smaller than half of real data size
                seriesLength: 10, // The length of series that is kept in buffer for modeling
                trainSize: csvLineCount, // Input size for training
                horizon: 3 // Amount of Forecasts to predict
                );

            // Train.
            var model = pipeline.Fit(data);

            // Forecast next 3 values.
            var forecastEngine = model.CreateTimeSeriesEngine<DataStructML, CountForecast>(MlContext);

            var forecasts = forecastEngine.Predict();

            ViewData["In1MonthsFromNowForecast"] = forecasts.ForeCast[0].ToString();
            ViewData["In2MonthsFromNowForecast"] = forecasts.ForeCast[1].ToString();
            ViewData["In3MonthsFromNowForecast"] = forecasts.ForeCast[2].ToString();
            ViewData["NextYearNumber"] = (DateTime.Now.Year + 1).ToString();

            return View();
        }

        private bool ViewModelExists(int id)
        {
            return _context.ViewModel.Any(e => e.ID == id);
        }

        public ActionResult ValidateAdmin (HttpContext context)
        {
            return context.Session.GetString("IsAdmin") == "true" ? null : RedirectToAction("Index", "Home");
        }

        public bool isAdmin(HttpContext context)
        {
            return context.Session.GetString("IsAdmin") == "true" ? true : false;
        }

        public Dictionary<string, int> GetDataDict(List<User> allUsers)
        {
            //var allUsers = await _context.User.ToListAsync();

            // Data saved as: "yearNumber-monthNumber-1 00:00:00" (Added 1 to the day and 00:00:00 to hour in order to keep on Date structure) - count
            var UsersDataReady = new Dictionary<string, int>();

            foreach (var user in allUsers)
            {
                string DateIndex = string.Format("{0}-{1}-1 00:00:00", user.EnteranceDate.Year.ToString(), user.EnteranceDate.Month.ToString());
                try
                {
                    if (user.EnteranceDate.Year <= DateTime.Now.Year) // Do Not Enter To Model Stats where entrance data is in next year
                    {
                        UsersDataReady.Add(DateIndex, 1); // First time a user in this exact year and month register
                    }
                }
                catch (ArgumentException) // Means a date and month same as this user's (key)already exists
                {
                    UsersDataReady[DateIndex]++;
                }
            }

            var csvDataFormat = new StringBuilder();

            foreach (var dictItem in UsersDataReady)
            {
                var newLine = string.Format("{0},{1}", dictItem.Key, dictItem.Value.ToString());
                csvDataFormat.AppendLine(newLine);
            }

            // Create the file, or overwrite if the file exists.
            using (StreamWriter writer = new StreamWriter(new FileStream(Config.CsvFilePath,
            FileMode.Create, FileAccess.Write)))
            {
                writer.Write(csvDataFormat);
            }
            
            return UsersDataReady;
        }
    }

}
