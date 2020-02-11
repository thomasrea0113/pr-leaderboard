using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;

namespace Leaderboard.Areas.Leaderboards.Controllers
{
    [Area("Leaderboards")]
    public class LeaderboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leaderboards/Leaderboard
        public async Task<IActionResult> Index()
        {
            return View(await _context.leaderboards.AsQueryable().ToListAsync());
        }

        // GET: Leaderboards/Leaderboard/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboardModel = await _context.leaderboards.AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaderboardModel == null)
            {
                return NotFound();
            }

            return View(leaderboardModel);
        }

        // GET: Leaderboards/Leaderboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaderboards/Leaderboard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] LeaderboardModel leaderboardModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaderboardModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaderboardModel);
        }

        // GET: Leaderboards/Leaderboard/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboardModel = await _context.leaderboards.FindAsync(id);
            if (leaderboardModel == null)
            {
                return NotFound();
            }
            return View(leaderboardModel);
        }

        // POST: Leaderboards/Leaderboard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] LeaderboardModel leaderboardModel)
        {
            if (id != leaderboardModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaderboardModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaderboardModelExists(leaderboardModel.Id))
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
            return View(leaderboardModel);
        }

        // GET: Leaderboards/Leaderboard/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboardModel = await _context.leaderboards.AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaderboardModel == null)
            {
                return NotFound();
            }

            return View(leaderboardModel);
        }

        // POST: Leaderboards/Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var leaderboardModel = await _context.leaderboards.FindAsync(id);
            _context.leaderboards.Remove(leaderboardModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaderboardModelExists(string id)
        {
            return _context.leaderboards.Any(e => e.Id == id);
        }
    }
}
