namespace PermissionModule.Permissions
{
    public abstract class Permission
    {
        public bool IsEnabled { get; protected set; }
        public bool IsVisible { get; protected set; }

        protected Permission()
        {
            this.IsEnabled = true;
            this.IsVisible = true;
        }
    }
}