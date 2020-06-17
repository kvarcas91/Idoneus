namespace Common
{
    public interface IStorage
    {
        string UserName { get; set; }
        bool IsUserGuest { get; set; }
        bool FirstLoad { get; set; }
        bool IsExporting { get; set; }
    }
}
