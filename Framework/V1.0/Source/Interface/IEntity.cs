namespace FS.Interface
{
    public interface IEntity<T> where T : struct
    {
        T ID { get; set; }
    }

    public interface IEntity : IEntity<int> { }
}
