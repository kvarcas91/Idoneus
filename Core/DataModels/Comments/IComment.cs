namespace Core.DataModels
{
    public interface IComment : IElement, IDateable
    {

        string Header { get; set; }

    }
}
