using PermissionModule.Permissions;

namespace PermissionModule
{
    public interface IPermissionFactory
    {
        Permissions.Permission GetPermission(PermissionSettings permissionSettings);
    }
}