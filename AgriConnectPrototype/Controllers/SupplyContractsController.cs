using AgriConnectPrototype.Data;
using AgriConnectPrototype.Models;
using AgriConnectPrototype.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPrototype.Controllers
{
    // Handles supply contract operations
    // Includes file upload, search/filter and agreement download

    public class SupplyContractsController : Controller
    {
        // Handles PDF file validation
        private readonly FileValidationService _fileValidationService;

        // Database connection
        private readonly AppDbContext _context;

        // Inject required services
        public SupplyContractsController(
            AppDbContext context,
            FileValidationService fileValidationService)
        {
            _context = context;
            _fileValidationService = fileValidationService;
        }

        // Display all contracts with search/filter options
        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? region,
            string? produceType)
        {
            // Get contracts and farmer information
            var contracts =
                from s in _context.SupplyContracts.Include(s => s.Farmer)
                select s;

            // Filter by start date
            if (startDate.HasValue)
            {
                contracts =
                    from s in contracts
                    where s.StartDate >= startDate.Value
                    select s;
            }

            // Filter by end date
            if (endDate.HasValue)
            {
                contracts =
                    from s in contracts
                    where s.EndDate <= endDate.Value
                    select s;
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                contracts =
                    from s in contracts
                    where s.Status == status
                    select s;
            }

            // Filter by farmer region
            if (!string.IsNullOrEmpty(region))
            {
                contracts =
                    from s in contracts
                    where s.Farmer.Region == region
                    select s;
            }

            // Filter by produce type
            if (!string.IsNullOrEmpty(produceType))
            {
                contracts =
                    from s in contracts
                    where s.ProduceType == produceType
                    select s;
            }

            return View(await contracts.ToListAsync());
        }

        // Display selected contract details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyContract =
                await _context.SupplyContracts
                .Include(s => s.Farmer)
                .FirstOrDefaultAsync(
                    m => m.SupplyContractId == id);

            if (supplyContract == null)
            {
                return NotFound();
            }

            return View(supplyContract);
        }

        // Display Create page
        public IActionResult Create()
        {
            ViewData["FarmerId"] =
                new SelectList(
                    _context.Farmers,
                    "FarmerId",
                    "FullName");

            return View();
        }

        // Save new supply contract
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("SupplyContractId,FarmerId,StartDate,EndDate,Status,ProduceType,ServiceLevel,SignedAgreementPath")]
        SupplyContract supplyContract,
        IFormFile signedAgreement)
        {
            if (ModelState.IsValid)
            {
                // Check uploaded agreement
                if (signedAgreement != null &&
                    signedAgreement.Length > 0)
                {
                    try
                    {
                        // Allow PDF files only
                        _fileValidationService
                            .ValidatePdf(
                            signedAgreement.FileName);
                    }

                    catch (Exception ex)
                    {
                        ModelState.AddModelError(
                            "",
                            ex.Message);

                        ViewData["FarmerId"] =
                            new SelectList(
                                _context.Farmers,
                                "FarmerId",
                                "FullName",
                                supplyContract.FarmerId);

                        return View(supplyContract);
                    }

                    // Create unique filename
                    var fileName =
                        Guid.NewGuid().ToString()
                        + ".pdf";

                    // Save file into uploads folder
                    var uploadPath =
                        Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        fileName);

                    using (var stream =
                        new FileStream(
                            uploadPath,
                            FileMode.Create))
                    {
                        await signedAgreement
                            .CopyToAsync(stream);
                    }

                    // Save file location
                    supplyContract
                        .SignedAgreementPath =
                        "/uploads/" + fileName;
                }

                // Save contract
                _context.Add(supplyContract);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(supplyContract);
        }

        // Display Edit page
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyContract =
                await _context.SupplyContracts
                .FindAsync(id);

            if (supplyContract == null)
            {
                return NotFound();
            }

            return View(supplyContract);
        }

        // Update contract
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SupplyContract supplyContract)
        {
            if (id != supplyContract.SupplyContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplyContract);

                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyContractExists(
                        supplyContract.SupplyContractId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(supplyContract);
        }

        // Display Delete page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyContract =
                await _context.SupplyContracts
                .Include(s => s.Farmer)
                .FirstOrDefaultAsync(
                    m => m.SupplyContractId == id);

            if (supplyContract == null)
            {
                return NotFound();
            }

            return View(supplyContract);
        }

        // Remove contract
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplyContract =
                await _context.SupplyContracts
                .FindAsync(id);

            if (supplyContract != null)
            {
                _context.SupplyContracts.Remove(supplyContract);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check if contract exists
        private bool SupplyContractExists(int id)
        {
            return _context.SupplyContracts
                .Any(e => e.SupplyContractId == id);
        }

        // Download uploaded PDF agreement
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract =
                await _context.SupplyContracts.FindAsync(id);

            if (contract == null ||
                string.IsNullOrEmpty(
                    contract.SignedAgreementPath))
            {
                return NotFound();
            }

            var filePath =
                Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                contract.SignedAgreementPath.TrimStart('/'));

            return PhysicalFile(filePath, "application/pdf", "SignedSupplyAgreement.pdf");
        }
    }
}