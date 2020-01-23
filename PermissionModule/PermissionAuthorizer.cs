using System.Collections.Generic;
using System.Linq;

namespace PermissionModule
{
    public class PermissionAuthorizer : IPermissionAuthorizer
    {
        public bool IsAuthorized(CallContext cc, IList<PropertyUserPrivilege> privilegeList, int privilegeId)
        {
            bool isAuthorized = false;

            if (cc.IsPrivate)
            {
                isAuthorized = privilegeList.Any(privilege => privilege.PrivilegeId == privilegeId);
            }
            else
            {
                isAuthorized = privilegeList.Any(privilege => privilege.PrivilegeId == privilegeId &&
                                                              (privilege.PropertyId == cc.PropertyId || privilege.IsSystemPrivilege.Value)
                );
            }

            return isAuthorized;
        }
    }
}