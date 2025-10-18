using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Data;
using VirtualTicketing.Models;
using System;

namespace VirtualTicketing.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PurchaseController(ApplicationDbContext context) => _context = context;

        // STEP 1: Select events
        public IActionResult Select()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        // STEP 2: Review purchase
        [HttpPost]
        public IActionResult Review(List<int> eventIds, List<int> quantities)
        {
            if (eventIds == null || quantities == null)
                return BadRequest("Missing event or quantity data.");

            var selected = new List<Event>();
            var cleanQuantities = new List<int>();

            for (int i = 0; i < eventIds.Count; i++)
            {
                var qty = (i < quantities.Count) ? quantities[i] : 0;
                if (qty > 0)
                {
                    var ev = _context.Events.Find(eventIds[i]);
                    if (ev != null)
                    {
                        selected.Add(ev);
                        cleanQuantities.Add(qty);
                    }
                }
            }

            ViewData["Quantities"] = cleanQuantities;
            return View(selected);
        }

        // STEP 3: Confirm and save
        [HttpPost]
        public IActionResult Confirm(string guestName, string guestEmail, List<int> eventIds, List<int> quantities)
        {
            if (string.IsNullOrWhiteSpace(guestName) || string.IsNullOrWhiteSpace(guestEmail))
                return BadRequest("Missing guest info.");

            // ✅ Always store UTC time for PostgreSQL compatibility
            var purchase = new Purchase
            {
                GuestName = guestName,
                GuestEmail = guestEmail,
                PurchaseDate = DateTime.UtcNow // ✅ FIXED HERE
            };

            decimal total = 0;
            for (int i = 0; i < eventIds.Count && i < quantities.Count; i++)
            {
                if (quantities[i] > 0)
                {
                    var ev = _context.Events.Find(eventIds[i]);
                    if (ev != null && ev.AvailableTickets >= quantities[i])
                    {
                        ev.AvailableTickets -= quantities[i];
                        var item = new PurchaseItem
                        {
                            EventId = ev.Id,
                            Quantity = quantities[i],
                            Price = ev.Price
                        };
                        total += ev.Price * quantities[i];
                        purchase.Items.Add(item);
                    }
                }
            }

            purchase.TotalCost = total;

            _context.Purchases.Add(purchase);
            _context.SaveChanges();

            return RedirectToAction(nameof(Confirmation), new { id = purchase.Id });
        }

        // STEP 4: Confirmation page
        public IActionResult Confirmation(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Items)
                .ThenInclude(i => i.Event)
                .FirstOrDefault(p => p.Id == id);

            return View(purchase);
        }
    }
}
