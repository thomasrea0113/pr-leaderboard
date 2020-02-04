using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Data;

namespace Leaderboard.Areas.Profiles.Controllers
{
    [Area("Profiles")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Profiles/Profile
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserProfiles.ToListAsync());
        }

        // GET: Profiles/Profile/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfileModel = await _context.UserProfiles
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userProfileModel == null)
            {
                return NotFound();
            }

            return View(userProfileModel);
        }

        // GET: Profiles/Profile/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,IsActive")] UserProfileModel userProfileModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProfileModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProfileModel);
        }

        // GET: Profiles/Profile/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfileModel = await _context.UserProfiles.FindAsync(id);
            if (userProfileModel == null)
            {
                return NotFound();
            }
            return View(userProfileModel);
        }

        // POST: Profiles/Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,IsActive")] UserProfileModel userProfileModel)
        {
            if (id != userProfileModel.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProfileModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileModelExists(userProfileModel.UserId))
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
            return View(userProfileModel);
        }

        // GET: Profiles/Profile/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfileModel = await _context.UserProfiles
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userProfileModel == null)
            {
                return NotFound();
            }

            return View(userProfileModel);
        }

        // POST: Profiles/Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userProfileModel = await _context.UserProfiles.FindAsync(id);
            _context.UserProfiles.Remove(userProfileModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProfileModelExists(string id)
        {
            return _context.UserProfiles.Any(e => e.UserId == id);
        }
    }
}
