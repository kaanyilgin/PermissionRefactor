using PermissionModule.Permissions;

namespace PermissionModule
{
    public interface IPermissionFactory
    {
        Permission GetPermission(ActionEnum action, bool isPrivate);
    }
}