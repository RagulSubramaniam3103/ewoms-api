using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserStoryHandler
    {
        private readonly ApplicationDbContext _context;
        public MasterUserStoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(string userId, string? caption, byte[] imageBytes)
        {
            try
            {
                var story = new UserStory
                {
                    UserId = userId,
                    Caption = caption,
                    StoryImage = imageBytes,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    IsActive = true,
                    IsBlurred = false,
                    ViewCount = 0
                };

                Console.WriteLine($"[DEBUG] Attempting to save story for User: {userId}");
                await _context.UserStories.AddAsync(story);
                await _context.SaveChangesAsync();

                return "Intelligence broadcast shared successfully.";
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                var deepInner = ex.InnerException?.InnerException?.Message ?? "";
                throw new Exception($"DB Error: {inner} {deepInner}");
            }
        }

        public async Task<string> MarkAsSeen(int storyId, string viewerId)
        {
            var alreadySeen = await _context.UserStoryViews
                .AnyAsync(v => v.StoryId == storyId && v.ViewerId == viewerId);

            if (!alreadySeen)
            {
                var view = new UserStoryView
                {
                    StoryId = storyId,
                    ViewerId = viewerId,
                    ViewedAt = DateTime.UtcNow
                };
                await _context.UserStoryViews.AddAsync(view);
                await _context.SaveChangesAsync();
            }
            return "Marked as seen";
        }

        public async Task<List<object>> GetStories(string currentUserId)
        {
            var now = DateTime.UtcNow;

            // 1. Fetch ALL active stories for the Community Tray (Global)
            var stories = await _context.UserStories
                .Where(s => s.IsActive && s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var seenStoryIds = await _context.UserStoryViews
                .Where(v => v.ViewerId == currentUserId)
                .Select(v => v.StoryId)
                .ToListAsync();

            // 4. Group by user for the frontend tray
            var grouped = stories.GroupBy(s => s.UserId).Select(g => new
            {
                UserId = g.Key,
                UserName = _context.Users.Where(u => u.Id == g.Key).Select(u => u.UserName).FirstOrDefault() ?? "Team Member",
                UserImage = _context.Users.Where(u => u.Id == g.Key).Select(u => u.ProfileImage).FirstOrDefault(),
                UserRole = _context.Master_Admins.Any(a => a.UserId == g.Key) ? "admin" :
                           _context.Master_MasterManager.Any(m => m.UserId == g.Key) ? "manager" : "user",
                IsPrivate = _context.Users.Where(u => u.Id == g.Key).Select(u => u.IsPrivate).FirstOrDefault(),
                HasUnseen = g.Any(s => !seenStoryIds.Contains(s.Id)),
                Segments = g.Select(s => new
                {
                    s.Id,
                    DisplayImage = Convert.ToBase64String(s.StoryImage),
                    s.Caption,
                    s.CreatedAt,
                    IsSeen = seenStoryIds.Contains(s.Id)
                }).ToList()
            }).ToList<object>();

            return grouped;
        }
    }
}

