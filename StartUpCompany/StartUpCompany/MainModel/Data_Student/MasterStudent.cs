using System.ComponentModel.DataAnnotations;

namespace StartUpCompany.MainModel.Data_Student
{
    public class MasterStudent
    {
        public string UserId { get; set; }
        public MasterUsers User { get; set; }
        [Key]
        public int StudId { get; set; }
        public string? StudentUniqueCode { get; set; }
        public string? StudName { get; set; }
        public bool? IsFreshStudent { get; set; }
        public string? StudPreviousSchool { get; set; }
        public DateTime? StudentJoining { get; set; }
        public string? StudDescription { get; set; }
        public string? StudEmail { get; set; }
        public string? StudPhone { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Village { get; set; }
        public long? Pincode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}
