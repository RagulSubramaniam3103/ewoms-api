using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO.UserPost;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserPostHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MasterUser> _userManager;
        public MasterUserPostHandler(ApplicationDbContext context, UserManager<MasterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<string> Handle(MasterUserPostCommand command)
        {
            if (command == null)
                return "Invalid request";

            if (command.Image == null || command.Image.Length == 0)
                return "Image is required";

            var userget = _userManager.Users.FirstOrDefault(u => u.Id == command.UserId);

            var username = "";
            if (userget != null)
            {
                username = userget.UserName.FirstOrDefault().ToString();
            }

            var post = new EWOMS_ClassLibrary.DataIntegration.UserPost
            {
                UserId = command.UserId,
                Caption = command.Caption,
                profileimage = command.Image,
                CreatedAt = command.CreatedAt
            };

            await _context.UserPost.AddAsync(post);
            await _context.SaveChangesAsync();

            return "Post uploaded successfully";
        }

    }
}
