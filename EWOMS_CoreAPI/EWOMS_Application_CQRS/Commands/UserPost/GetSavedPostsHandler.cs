using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class GetSavedPostsHandler
    {
        private readonly ApplicationDbContext _context;

        public GetSavedPostsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> Handle(string userId)
        {
            var savedPosts = await (from s in _context.SavedPosts
                                    where s.UserId == userId
                                    join p in _context.UserPost on s.PostId equals p.Id
                                    where !p.IsDeleted && p.IsActive
                                    join u in _context.Users on p.UserId equals u.Id into userJoined
                                    from u in userJoined.DefaultIfEmpty()
                                    select new
                                    {
                                        p.Id,
                                        p.UserId,
                                        UserName = u != null ? u.UserName : "Unknown",
                                        UserImage = u != null && u.ProfileImage != null
                                             ? Convert.ToBase64String(u.ProfileImage)
                                             : null,
                                        p.Caption,
                                        PostImage = p.profileimage != null ? Convert.ToBase64String(p.profileimage) : null,
                                        p.IsBlurred,
                                        p.CreatedAt,
                                        LikeCount = _context.PostLikes.Count(l => l.PostId == p.Id),
                                        IsLiked = !string.IsNullOrEmpty(userId) && _context.PostLikes.Any(l => l.PostId == p.Id && l.UserId == userId),
                                        IsSaved = true
                                    })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return savedPosts.Cast<object>().ToList();
        }
    }
}
