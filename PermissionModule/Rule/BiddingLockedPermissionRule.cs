namespace PermissionModule.Rule
{
    public class BiddingLockedPermissionRule : IPermissionRule
    {
        public bool IsEnabled(PermissionSettings permissionSettings)
        {
            return !permissionSettings.IsBiddingLocked;
        }

        public bool IsVisible(PermissionSettings permissionSettings)
        {
            return !permissionSettings.IsBiddingLocked;
        }
    }
}