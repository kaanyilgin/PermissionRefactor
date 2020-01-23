using System.Collections.Generic;

namespace PermissionModule
{
    public class PermissionSettings
    {
        public ActionEnum Action { get; set; }
        public bool IsPrivate { get; set; }
        public int PropertyStatusTypeId { get; set; }
        public CallContext CallContext { get; set; }
        public IList<PropertyUserPrivilege> PrivilegesByUserRoom { get; set; }
        public bool IsBiddingLocked { get; set; }
    }
}