using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Data;
using VirtualTicketing.Models;

namespace VirtualTicketing.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: Event (Index with Search, Filter, Sort)
        // ============================================================
        public async Task<IActionResult> Index(string? searchString, int? categoryId, DateTime? date, string? sortOrder)
        {
            var eventsQuery = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(searchString))
                eventsQuery = eventsQuery.Where(e => e.Name.ToLower().Contains(searchString.ToLower()));

            // 🏷️ Filter by Category
            if (categoryId.HasValue && categoryId > 0)
                eventsQuery = eventsQuery.Where(e => e.CategoryId == categoryId.Value);

            // 📅 Filter by Date
            if (date.HasValue)
                eventsQuery = eventsQuery.Where(e => e.Date.Date == date.Value.Date);

            // ⬇️ Sorting
            eventsQuery = sortOrder switch
            {
                "title_desc" => eventsQuery.OrderByDescending(e => e.Name),
                "date" => eventsQuery.OrderBy(e => e.Date),
                "date_desc" => eventsQuery.OrderByDescending(e => e.Date),
                _ => eventsQuery.OrderBy(e => e.Name)
            };

            // 📋 For category dropdown
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            return View(await eventsQuery.ToListAsync());
        }

        // ============================================================
        // DETAILS
        // ============================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        // ============================================================
        // CREATE — GET
        // ============================================================
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // ============================================================
        // CREATE — POST (FIXED for PostgreSQL DateTime)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventItem)
        {
            if (ModelState.IsValid)
            {
                // 🔥 CRITICAL FIX — PostgreSQL requires UTC timestamps
                eventItem.Date = DateTime.SpecifyKind(eventItem.Date, DateTimeKind.Utc);

                _context.Add(eventItem);
                await _context.SaveChangesAsync();

                TempData["success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // ============================================================
        // EDIT — GET
        // ============================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // ============================================================
        // EDIT — POST (FIXED for PostgreSQL DateTime)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event eventItem)
        {
            if (id != eventItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔧 Ensure UTC for PostgreSQL
                    eventItem.Date = DateTime.SpecifyKind(eventItem.Date, DateTimeKind.Utc);

                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == eventItem.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventItem.CategoryId);
            return View(eventItem);
        }

        // ============================================================
        // DELETE — GET
        // ============================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        // ============================================================
        // DELETE — POST
        // ============================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem != null)
                _context.Events.Remove(eventItem);

            await _context.SaveChangesAsync();
            TempData["success"] = "Event deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
