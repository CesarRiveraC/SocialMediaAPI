using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;

namespace SocialMedia.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialMediaContext _context;

        public PostRepository(SocialMediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post> GetPost(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(x => x.PostId == id);
        }

        public async Task InsertPost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            var currentPost = await GetPost(post.PostId);
            currentPost.Date = post.Date;
            currentPost.Description = post.Description;
            currentPost.Image = post.Image;

            int rows = await _context.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeletePost(int id)
        {
            var currentPost = await GetPost(id);
            _context.Posts.Remove(currentPost);
            int rows = await _context.SaveChangesAsync();
            return rows>0;
        }
    }
}