namespace PermissionModule.Rule
{
    public class PrivatePermissionRule : IPermissionRule
    {
        public bool IsEnabled(PermissionSettings permissionSettings)
        {
            return !permissionSettings.IsPrivate;
        }

        public bool IsVisible(PermissionSettings permissionSettings)
        {
            return !permissionSettings.IsPrivate;
        }
    }
}