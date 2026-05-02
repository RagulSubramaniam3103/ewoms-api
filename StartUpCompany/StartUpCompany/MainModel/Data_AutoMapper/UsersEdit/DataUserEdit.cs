using MediatR;

namespace StartUpCompany.MainModel.Data_AutoMapper.UsersEdit
{
    public class DataUserEdit : IRequest<object>
    {
        public string Email { get; set; }

        public bool? IsCareerStart { get; set; }
        public string PreviousSchool { get; set; }
        public DateTime? JoiningDate { get; set; }

        public string Phone { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string Village { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
    }
}
