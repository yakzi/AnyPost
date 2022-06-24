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
        public async Task<IActionResult> Index(string tags)
        {
            if(!String.IsNullOrEmpty(tags))
            {
                return View(await _context.Post.OrderByDescending(i => i.Rating).Where(x => x.Tags.Contains(tags) ).ToListAsync());
            }
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
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
    
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
    }
}
