using MediatR;

namespace StartUpCompany.CQRSMethod.Command.UserAdminEdit
{
    public class UserEditAdminCommand : IRequest<object>
    {
        public string Email { get; set; }
        public bool? IsCareerStart { get; set; }
        public string? PreviousSchool { get; set; }
        public DateTime? AdminJoiningDate { get; set; }
        public string? AdminPhone { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Village { get; set; }
        public long? Pincode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}
