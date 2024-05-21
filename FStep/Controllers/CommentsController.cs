using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FStep.Data;

namespace FStep.Controllers
{
    public class CommentsController : Controller
    {
        private readonly Fstep1Context _context;

        public CommentsController(Fstep1Context context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var fstep1Context = _context.Comments.Include(c => c.IdPostNavigation).Include(c => c.IdUserNavigation);
            return View(await fstep1Context.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.IdPostNavigation)
                .Include(c => c.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdComment == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["IdPost"] = new SelectList(_context.Posts, "IdPost", "IdPost");
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdComment,Content,Date,IdUser,IdPost")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPost"] = new SelectList(_context.Posts, "IdPost", "IdPost", comment.IdPost);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", comment.IdUser);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["IdPost"] = new SelectList(_context.Posts, "IdPost", "IdPost", comment.IdPost);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", comment.IdUser);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdComment,Content,Date,IdUser,IdPost")] Comment comment)
        {
            if (id != comment.IdComment)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.IdComment))
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
            ViewData["IdPost"] = new SelectList(_context.Posts, "IdPost", "IdPost", comment.IdPost);
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "IdUser", comment.IdUser);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.IdPostNavigation)
                .Include(c => c.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdComment == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.IdComment == id);
        }
    }
}
