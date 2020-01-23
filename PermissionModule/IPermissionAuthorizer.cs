using System.Collections.Generic;

namespace PermissionModule
{
    public interface IPermissionAuthorizer
    {
        bool IsAuthorized(CallContext cc, IList<PropertyUserPrivilege> privilegeList, int privilegeId);
    }
}