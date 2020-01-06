namespace Core.DataModels
{
    public interface IContributor : IPerson
    {

        string Initials { get; }

        string InitialColor { get; }

    }
}
