namespace PermissionModule
{
    public class PermissionModel
    {
        public ActionEnum Action { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
    }
}