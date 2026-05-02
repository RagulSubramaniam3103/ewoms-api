namespace StartUpCompany.CQRSMethod.Queries.Usersabstract
{
    public class Encapsulationclass
    {
        private string _UserId;
        public string UId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
    }

    public class EnumuserRole
    {
        private string _UserRole;
        public string URole
        {
            get { return _UserRole; }
            set { _UserRole = value; }
        }
        public void SetUserRole(UserRole role)
        {
            _UserRole = role.ToString();
        }
    }
    public enum UserRole
    {
        Admin,
        Student,
        Staff
    }

    public abstract class IUserAbstract
    {
        public Encapsulationclass UserId { get; set; } = new Encapsulationclass();
        public abstract Task<object> ExecutObject();
    }

    public abstract class AbstractUserIDRole
    {
        public Encapsulationclass UserId { get; set; } = new Encapsulationclass();
        public EnumuserRole UserRole { get; set; } = new EnumuserRole();
        public abstract Task<object> ExecuteData();
    }

    public abstract class AbstractUserDetails
    {
        public EnumuserRole UserRole { get; set; } = new EnumuserRole();
        public abstract Task<object> ExecuteDetails();
    }
}
