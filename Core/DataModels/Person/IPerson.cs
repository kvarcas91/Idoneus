namespace Core.DataModels
{
    public interface IPerson
    {

        long ID { get; set; }
        string FirstName { get; }
        string LastName { get; }
        string FullName { get;}

    }
}
