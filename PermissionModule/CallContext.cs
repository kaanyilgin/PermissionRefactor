namespace PermissionModule
{
    public class CallContext
    {
        public int PropertyId { get; set; }
        public string LoginName { get; set; }
        public bool IsSpecialCustomer { get; set; }
        public bool IsPrivate { get; set; }
    }
}