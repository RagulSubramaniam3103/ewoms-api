using AutoMapper;
using StartUpCompany.MainModel.Data_Admin_Staff;
using StartUpCompany.MainModel.Data_Student;

namespace StartUpCompany.MainModel.Data_AutoMapper.UsersEdit
{
    public class AutoMappingUserEdit : Profile
    {
        public AutoMappingUserEdit()
        {
            CreateMap<DataUserEdit, MasterAdmin>()
                .ForMember(d => d.AdminEmail, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.AdminPhone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.IsCareerStart, o => o.MapFrom(s => s.IsCareerStart))
                .ForMember(d => d.AdminJoiningDate, o => o.MapFrom(s => s.JoiningDate))
                .ForMember(d => d.PreviousSchool, o => o.MapFrom(s => s.PreviousSchool));
            CreateMap<DataUserEdit, MasterStaff>()
                .ForMember(d => d.StaffEmail, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.StaffPhone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.IsCareerStart, o => o.MapFrom(s => s.IsCareerStart))
                .ForMember(d => d.Staffjoining, o => o.MapFrom(s => s.JoiningDate))
                .ForMember(d => d.StaffPreviousSchool, o => o.MapFrom(s => s.PreviousSchool));
            CreateMap<DataUserEdit, MasterStudent>()
                .ForMember(d => d.StudEmail, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.StudPhone, o => o.MapFrom(s => s.Phone))
                .ForMember(d => d.IsFreshStudent, o => o.MapFrom(s => s.IsCareerStart))
                .ForMember(d => d.StudentJoining, o => o.MapFrom(s => s.JoiningDate))
                .ForMember(d => d.StudPreviousSchool, o => o.MapFrom(s => s.PreviousSchool));

        }
    }
}
