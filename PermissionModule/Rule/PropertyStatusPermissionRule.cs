namespace PermissionModule.Rule
{
    public class PropertyStatusPermissionRule : IPermissionRule
    {
        public bool IsEnabled(PermissionSettings permissionSettings)
        {
            return permissionSettings.PropertyStatusTypeId != 5;
        }

        public bool IsVisible(PermissionSettings permissionSettings)
        {
            return true;
        }
    }
}