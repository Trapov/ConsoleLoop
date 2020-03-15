namespace ConsoleLoop
{
    public interface IChangesTrackerFor<TModel>
    {
        bool IsChanged(TModel model);
    }
}
