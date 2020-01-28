namespace Core.DataModels
{
    public interface IElement 
    {

        /// <summary>
        /// Element ID
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Element Content
        /// </summary>
        string Content { get; set; }

    }
}
