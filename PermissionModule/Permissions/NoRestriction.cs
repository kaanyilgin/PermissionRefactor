namespace PermissionModule.Permissions
{
    public class NoRestriction : Permission
    {
        public NoRestriction()
        {
            this.IsEnabled = true;
            this.IsVisible = true;
        }
    }
}