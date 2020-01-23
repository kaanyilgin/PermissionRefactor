using System.Collections.Generic;

namespace PermissionModule
{
    public interface IAuthorizationService
    {
        List<PropertyUserPrivilege> GetPrivilegesByUserProperty(string loginName, int propertyId, PrivilegeCategoryEnum privilegeCategoryEnum);
    }
}