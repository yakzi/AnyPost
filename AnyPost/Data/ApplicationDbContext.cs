using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AnyPost.Models;

namespace AnyPost.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AnyPost.Models.Post> Post { get; set; }
        public DbSet<AnyPost.Models.Comment> Comment { get; set; }
    }
}
