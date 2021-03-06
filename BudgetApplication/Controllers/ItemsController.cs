﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetApplication.Data;
using BudgetApplication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BudgetApplication.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.Items.Include(i => i.Category).Where(x => x.UserID == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(x => x.UserID == userId), "CategoryID", "CategoryName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var values = new Item
                {
                    ItemID = item.ItemID,
                    CategoryID = item.CategoryID,
                    ItemName = item.ItemName,
                    UserID = userId
                };
                _context.Add(values);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var userId2 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(x => x.UserID == userId2), "CategoryID", "CategoryID", item.CategoryID);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(x => x.UserID == userId), "CategoryID", "CategoryName", item.CategoryID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            if (id != item.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var values = new Item
                    {
                        ItemID = item.ItemID,
                        CategoryID = item.CategoryID,
                        ItemName = item.ItemName,
                        UserID = userId
                    };
                    _context.Update(values);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemID))
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
            var userId2 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(x => x.UserID == userId2), "CategoryID", "CategoryName", item.CategoryID);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.SingleOrDefaultAsync(m => m.ItemID == id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemID == id);
        }
    }
}
