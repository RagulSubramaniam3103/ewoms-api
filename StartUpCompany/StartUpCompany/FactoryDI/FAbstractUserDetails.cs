using StartUpCompany.CQRSMethod.Queries.Usersabstract;

namespace StartUpCompany.FactoryDI
{
    public class FAbstractUserDetails : IFAbstractUserDetails
    {
        private readonly HanndlerUserAdminAbstract _hanndlerUserAdminAbstract;
        private readonly HanndlerUserStudentAbstract _hanndlerUserStudentAbstract;
        private readonly HanndlerUserStaffAbstract _hanndlerUserStaffAbstract;

        public FAbstractUserDetails(HanndlerUserAdminAbstract hanndlerUserAdminAbstract, HanndlerUserStudentAbstract hanndlerUserStudentAbstract, HanndlerUserStaffAbstract hanndlerUserStaffAbstract)
        {
            _hanndlerUserAdminAbstract = hanndlerUserAdminAbstract;
            _hanndlerUserStudentAbstract = hanndlerUserStudentAbstract;
            _hanndlerUserStaffAbstract = hanndlerUserStaffAbstract;
        }

        public AbstractUserIDRole GetHandler(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => _hanndlerUserAdminAbstract,
                UserRole.Student => _hanndlerUserStudentAbstract,
                UserRole.Staff => _hanndlerUserStaffAbstract,
                _ => throw new Exception("No Role")
            };
        }
    }

    public class FAbstractAlluserDetails : IFAbstractAllUserDetails
    {
        private readonly HandlerAllUserDetails_Admin _handlerAllUserDetails_Admin;
        private readonly HandlerAllUserDetails_Student _handlerAllUserDetails_Student;
        private readonly HandlerAllUserDetails_Staff _handlerAllUserDetails_Staff;
        public FAbstractAlluserDetails(HandlerAllUserDetails_Admin handlerAllUserDetails_Admin, HandlerAllUserDetails_Student handlerAllUserDetails_Student,
            HandlerAllUserDetails_Staff handlerAllUserDetails_Staff)
        {
            _handlerAllUserDetails_Admin = handlerAllUserDetails_Admin;
            _handlerAllUserDetails_Student = handlerAllUserDetails_Student;
            _handlerAllUserDetails_Staff = handlerAllUserDetails_Staff;
        }
        public AbstractUserDetails GetHandler(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => _handlerAllUserDetails_Admin,
                UserRole.Student => _handlerAllUserDetails_Student,
                UserRole.Staff => _handlerAllUserDetails_Staff,
                _ => throw new Exception("No Role")
            };
        }
    }
}
