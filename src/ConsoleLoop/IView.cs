namespace ConsoleLoop
{
    public interface IView<TModel>
    {
        string Render(TModel model);
    }
}
