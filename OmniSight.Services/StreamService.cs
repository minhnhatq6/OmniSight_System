using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities; // Nhớ check đúng namespace của bạn
using OmniSight.Data;

namespace OmniSight.Services
{
    public class StreamService
    {
        private readonly OmniSightDbContext _context;

        public StreamService(OmniSightDbContext context)
        {
            _context = context;
        }

        // 1. Chức năng Đăng bài mới lên bảng tin
        public async Task<Core.Entities.Stream> CreatePostAsync(int classId, int authorId, string content)
        {
            var post = new Core.Entities.Stream
            {
                ClassId = classId,
                AuthorId = authorId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            // Tên DbSet có thể là Streams (bạn check lại trong DbContext nhé)
            _context.Streams.Add(post);
            await _context.SaveChangesAsync();

            return post;
        }

        // 2. Lấy danh sách bài đăng của một lớp học (Sắp xếp bài mới nhất lên đầu)
        public async Task<List<Core.Entities.Stream>> GetPostsByClassIdAsync(int classId)
        {
            return await _context.Streams
                .Include(s => s.Author) // Lệnh Include này rất quan trọng: Nó giúp "kéo" luôn thông tin người đăng (để lát nữa UI lấy Tên người đăng hiển thị ra)
                .Where(s => s.ClassId == classId)
                .OrderByDescending(s => s.CreatedAt) // Mới nhất xếp trên
                .ToListAsync();
        }
    }
}