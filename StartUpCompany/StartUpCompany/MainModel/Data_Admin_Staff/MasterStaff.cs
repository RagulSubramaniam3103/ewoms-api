using System.ComponentModel.DataAnnotations;

namespace StartUpCompany.MainModel.Data_Admin_Staff
{
    public class MasterStaff
    {
        public string UserId { get; set; }
        public MasterUsers User { get; set; }
        [Key]
        public int StaffId { get; set; }
        public string? StaffUniqueCode { get; set; }
        public string? StaffName { get; set; }
        public bool? IsCareerStart { get; set; }
        public string? StaffPreviousSchool { get; set; }
        public DateTime? Staffjoining { get; set; }
        public string? StaffEmail { get; set; }
        public string? StaffPhone { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Village { get; set; }
        public long? Pincode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}
