using AgriConnectPrototype.Data;
using AgriConnectPrototype.Models;
using AgriConnectPrototype.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPrototype.Controllers
{
    // Handles farm service request operations
    // Includes workflow validation and currency conversion

    public class FarmServiceRequestsController : Controller
    {
        // Database connection
        private readonly AppDbContext _context;

        // Handles USD to ZAR calculations
        private readonly CurrencyService _currencyService;

        // Connects to exchange rate API
        private readonly ExchangeRateService _exchangeRateService;

        // Handles business rules
        private readonly ServiceRequestRules _serviceRequestRules;

        // Inject required services
        public FarmServiceRequestsController(
            AppDbContext context,
            CurrencyService currencyService,
            ExchangeRateService exchangeRateService,
            ServiceRequestRules serviceRequestRules)
        {
            _context = context;
            _currencyService = currencyService;
            _exchangeRateService = exchangeRateService;
            _serviceRequestRules = serviceRequestRules;
        }

        // Display all service requests
        public async Task<IActionResult> Index()
        {
            var appDbContext =
                _context.FarmServiceRequests
                .Include(f => f.SupplyContract);

            return View(await appDbContext.ToListAsync());
        }

        // Display selected request details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmServiceRequest =
                await _context.FarmServiceRequests
                .Include(f => f.SupplyContract)
                .FirstOrDefaultAsync(
                    m => m.FarmServiceRequestId == id);

            if (farmServiceRequest == null)
            {
                return NotFound();
            }

            return View(farmServiceRequest);
        }

        // Display Create page
        public IActionResult Create()
        {
            ViewData["SupplyContractId"] =
                new SelectList(
                    _context.SupplyContracts,
                    "SupplyContractId",
                    "ProduceType");

            return View();
        }

        // Save service request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("FarmServiceRequestId,SupplyContractId,Description,UsdAmount,ExchangeRate,CostInZar,Status")]
        FarmServiceRequest farmServiceRequest)
        {
            // Get selected contract
            var contract =
                await _context.SupplyContracts
                .FirstOrDefaultAsync(
                    c => c.SupplyContractId ==
                    farmServiceRequest.SupplyContractId);

            // Contract not found
            if (contract == null)
            {
                return NotFound();
            }

            // Apply workflow rule
            // Block requests for Expired or On Hold contracts
            if (!_serviceRequestRules
                .CanCreateServiceRequest(contract.Status))
            {
                ModelState.AddModelError("",
                    "A service request cannot be created because the contract is Expired or On Hold.");

                ViewData["SupplyContractId"] =
                    new SelectList(
                        _context.SupplyContracts,
                        "SupplyContractId",
                        "ProduceType",
                        farmServiceRequest.SupplyContractId);

                return View(farmServiceRequest);
            }

            if (ModelState.IsValid)
            {
                // Get current USD→ZAR rate from API
                var rate =
                    await _exchangeRateService
                    .GetUsdToZarRateAsync();

                // Save exchange rate
                farmServiceRequest.ExchangeRate = rate;

                // Convert USD amount into ZAR
                farmServiceRequest.CostInZar =
                    _currencyService
                    .ConvertUsdToZar(
                    farmServiceRequest.UsdAmount,
                    rate);

                // Save request
                _context.Add(farmServiceRequest);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplyContractId"] =
                new SelectList(
                    _context.SupplyContracts,
                    "SupplyContractId",
                    "ProduceType",
                    farmServiceRequest.SupplyContractId);

            return View(farmServiceRequest);
        }

        // Display Edit page
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmServiceRequest =
                await _context.FarmServiceRequests.FindAsync(id);

            if (farmServiceRequest == null)
            {
                return NotFound();
            }

            ViewData["SupplyContractId"] =
                new SelectList(
                    _context.SupplyContracts,
                    "SupplyContractId",
                    "ProduceType",
                    farmServiceRequest.SupplyContractId);

            return View(farmServiceRequest);
        }

        // Update service request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("FarmServiceRequestId,SupplyContractId,Description,UsdAmount,ExchangeRate,CostInZar,Status")]
            FarmServiceRequest farmServiceRequest)
        {
            if (id != farmServiceRequest.FarmServiceRequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmServiceRequest);

                    await _context.SaveChangesAsync();
                }

                // Handle update conflicts
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmServiceRequestExists(
                        farmServiceRequest.FarmServiceRequestId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(farmServiceRequest);
        }

        // Display Delete page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmServiceRequest =
                await _context.FarmServiceRequests
                .Include(f => f.SupplyContract)
                .FirstOrDefaultAsync(
                    m => m.FarmServiceRequestId == id);

            if (farmServiceRequest == null)
            {
                return NotFound();
            }

            return View(farmServiceRequest);
        }

        // Remove service request
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmServiceRequest =
                await _context.FarmServiceRequests.FindAsync(id);

            if (farmServiceRequest != null)
            {
                _context.FarmServiceRequests.Remove(farmServiceRequest);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check whether request exists
        private bool FarmServiceRequestExists(int id)
        {
            return _context.FarmServiceRequests
                .Any(e => e.FarmServiceRequestId == id);
        }
    }
}