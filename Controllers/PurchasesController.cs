using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Data;
using VirtualTicketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualTicketing.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===========================
        //  PURCHASES INDEX (VIEW ALL)
        // ===========================
        public IActionResult Index()
        {
            var purchases = _context.Purchases
                .Include(p => p.Items)
                .ThenInclude(i => i.Event)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();

            return View(purchases);
        }

        // ===========================
        //  STEP 1 — SELECT EVENTS
        // ===========================
        public IActionResult Select()
        {
            var events = _context.Events
                .Include(e => e.Category)
                .OrderBy(e => e.Name)
                .ToList();

            return View(events);
        }

        // ===========================
        //  STEP 2 — REVIEW ORDER
        // ===========================
        [HttpPost]
        public IActionResult Review(List<int> eventIds, List<int> quantities)
        {
            if (eventIds == null || quantities == null)
                return BadRequest("Missing event or quantity data.");

            var selectedEvents = new List<Event>();
            var cleanQuantities = new List<int>();

            for (int i = 0; i < eventIds.Count; i++)
            {
                int qty = (i < quantities.Count) ? quantities[i] : 0;
                if (qty > 0)
                {
                    var ev = _context.Events.Find(eventIds[i]);
                    if (ev != null)
                    {
                        selectedEvents.Add(ev);
                        cleanQuantities.Add(qty);
                    }
                }
            }

            ViewData["Quantities"] = cleanQuantities;
            return View(selectedEvents);
        }

        // ===========================
        //  STEP 3 — CONFIRM PURCHASE
        // ===========================
        [HttpPost]
        public IActionResult Confirm(string guestName, string guestEmail, List<int> eventIds, List<int> quantities)
        {
            if (string.IsNullOrWhiteSpace(guestName) || string.IsNullOrWhiteSpace(guestEmail))
                return BadRequest("Missing guest information.");

            if (eventIds == null || quantities == null)
                return BadRequest("Missing event/quantity information.");

            var purchase = new Purchase
            {
                GuestName = guestName,
                GuestEmail = guestEmail,
                PurchaseDate = DateTime.UtcNow,        // PostgreSQL UTC FIX
                Items = new List<PurchaseItem>()
            };

            decimal totalCost = 0;

            for (int i = 0; i < eventIds.Count && i < quantities.Count; i++)
            {
                int qty = quantities[i];
                if (qty <= 0) continue;

                var ev = _context.Events.Find(eventIds[i]);
                if (ev != null && ev.AvailableTickets >= qty)
                {
                    ev.AvailableTickets -= qty;

                    purchase.Items.Add(new PurchaseItem
                    {
                        EventId = ev.Id,
                        Quantity = qty,
                        Price = ev.Price
                    });

                    totalCost += ev.Price * qty;
                }
            }

            purchase.TotalCost = totalCost;

            _context.Purchases.Add(purchase);
            _context.SaveChanges();

            return RedirectToAction(nameof(Confirmation), new { id = purchase.Id });
        }

        // ===========================
        //  STEP 4 — CONFIRMATION PAGE
        // ===========================
        public IActionResult Confirmation(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Items)
                .ThenInclude(i => i.Event)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // ===========================
        //  DETAILS PAGE (VIEW SINGLE)
        // ===========================
        public IActionResult Details(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Items)
                .ThenInclude(i => i.Event)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // ===========================
        //  DELETE PURCHASE (OPTIONAL)
        // ===========================
        public IActionResult Delete(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Items)
                .ThenInclude(i => i.Event)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefault(p => p.Id == id);

            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
