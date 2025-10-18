using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Data;
using VirtualTicketing.Models;
using System.Threading.Tasks;

namespace VirtualTicketing.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event
     public async Task<IActionResult> Index(string? searchString, int? categoryId, DateTime? date, string? sortOrder)
     {
         // Start query
         var eventsQuery = _context.Events
             .Include(e => e.Category)
             .AsQueryable();
     
         // 🔍 Search by name
         if (!string.IsNullOrEmpty(searchString))
         {
             eventsQuery = eventsQuery.Where(e => e.Name.ToLower().Contains(searchString.ToLower()));
         }
     
         // 🏷️ Filter by category
         if (categoryId.HasValue && categoryId > 0)
         {
             eventsQuery = eventsQuery.Where(e => e.CategoryId == categoryId.Value);
         }
     
         // 📅 Filter by date
         if (date.HasValue)
         {
             eventsQuery = eventsQuery.Where(e => e.Date.Date == date.Value.Date);
         }
     
         // ⬇️ Sorting logic
         eventsQuery = sortOrder switch
         {
             "title_desc" => eventsQuery.OrderByDescending(e => e.Name),
             "date" => eventsQuery.OrderBy(e => e.Date),
             "date_desc" => eventsQuery.OrderByDescending(e => e.Date),
             _ => eventsQuery.OrderBy(e => e.Name) // Default sort (A-Z)
         };
     
         // 📋 Pass categories to the dropdown
         ViewData["Categories"] = await _context.Categories.ToListAsync();
     
         // ✅ Return filtered, sorted list to the view
         var events = await eventsQuery.ToListAsync();
         return View(events);
     }


        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // ✅ GET: Event/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // ✅ POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // ✅ GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // ✅ POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == @event.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // ✅ GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // ✅ POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
                _context.Events.Remove(@event);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
