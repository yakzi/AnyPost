using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnyPost.Data;
using AnyPost.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AnyPost.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            //var posts = await _context.Post.OrderByDescending(i => i.Rating).ToListAsync();
            //var comments = await _context.Comment.ToListAsync();
            //return View((posts,comments));
            return View(await _context.Post.OrderByDescending(i => i.Rating).ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var postT =  _context.Post.Where(p => p.Id == id).First();
            var comments = await _context.Comment.Where(c => c.PostId == id).OrderByDescending(i => i.CommentDate).ToListAsync();
            AnyPost.Models.Comment newComment = new Comment { PostId = id.Value };
            return View((postT, comments, newComment));
        }

        public async Task<IActionResult> AddComment([Bind("Id,PostId,CommentContent,CommentDate,UserId,UserName")] Comment Item3)
        {
            Item3.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Item3.UserName = User.FindFirstValue(ClaimTypes.Email);
            Item3.CommentDate = DateTime.Now;
            _context.Add(Item3);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = Item3.PostId });
            //RedirectToAction()
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //TODO: DODAJ DO KONTROLERA KOMENTARZA AKCJE TWORZENIA, WYWOLAJ JA Z WIDOKU DETAILS.CSHTML TYLKO ZE 
        //ON CI NIE ZBINDUJE TEGO BO MUSIALBYS JAKOS Z VIEW PRZESLAC DO TAMTEGO KONTROLERA??

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,PostDate,Rating,UserId,UserName,Tags")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.UserName = User.FindFirstValue(ClaimTypes.Email);
                post.PostDate = DateTime.Now;
                post.Rating = 0;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PostDate,Rating,UserId,UserName,Tags")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View();
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        
        public IActionResult RatingUp(int Id)
        {
            var record = _context.Post.Single(record => record.Id == Id);
            record.Rating++;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RatingDown(int Id)
        {
            var record = _context.Post.Single(record => record.Id == Id);
            record.Rating--;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
