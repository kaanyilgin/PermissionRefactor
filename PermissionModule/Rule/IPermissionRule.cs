namespace PermissionModule.Rule
{
    public interface IPermissionRule
    {
        bool IsEnabled(PermissionSettings permissionSettings);
        bool IsVisible(PermissionSettings permissionSettings);
    }
}