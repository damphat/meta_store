namespace meta_store
{
    public interface ILevel
    {

        ILevel Root { get; }
        ILevel Parent { get; }

        ILevel Next(string name);
    }
}
