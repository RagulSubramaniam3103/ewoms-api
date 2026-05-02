using StartUpCompany.CQRSMethod.Queries.Usersabstract;

namespace StartUpCompany.FactoryDI
{
    public interface IFAbstractUserDetails
    {
        AbstractUserIDRole GetHandler(UserRole role);
    }
    public interface IFAbstractAllUserDetails
    {
        AbstractUserDetails GetHandler(UserRole role);
    }
}
