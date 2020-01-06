namespace Core.DataModels
{
    public interface IInteractiveComponent : IComponent
    {

        bool Copy(string name);

        bool Move(string name);

        bool Rename(string name);

        bool Delete();

        bool Replace(string name);
    }
}
