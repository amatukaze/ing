namespace Sakuno.ING;

public interface IIdentifiable<T>
{
    T Id { get; }
}

public interface IIdentifiable : IIdentifiable<int>
{
}
