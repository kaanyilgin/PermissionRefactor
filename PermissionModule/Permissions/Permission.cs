namespace PermissionModule.Permissions
{
    public abstract class Permission
    {
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
    }
}