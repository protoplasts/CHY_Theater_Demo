using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public ActorsController(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }
        // GET: Actors/GetActor/5
        [HttpGet]
        public async Task<IActionResult> GetActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return Json(new { actorId = actor.ActorId, actorName = actor.ActorName });
        }
        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorName")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return Json(new { success = true, actorId = actor.ActorId, actorName = actor.ActorName });
            }
            return Json(new { success = false, message = "Invalid model state" });
        }
       
        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ActorId,ActorName")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, actorId = actor.ActorId, actorName = actor.ActorName });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.ActorId))
                    {
                        return Json(new { success = false, message = "Actor not found" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An error occurred while updating the actor" });
                    }
                }
            }
            return Json(new { success = false, message = "Invalid model state" });
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.ActorId == id);
        }
    }
}
