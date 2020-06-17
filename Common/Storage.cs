namespace Common
{
    public class Storage : IStorage
    {
        public string UserName { get; set; }
        public bool IsUserGuest { get; set; }
        public bool FirstLoad { get; set; } = true;
        public bool IsExporting { get; set; } = false;
    }
}
