namespace ConsoleLoop
{
    public sealed class SnapshotChangesTrackerFor<TModel> : IChangesTrackerFor<TModel>
    {
        public SnapshotChangesTrackerFor(TModel model)
        {
            ModelSnapshot = Snaphsot.From(model);
        }

        public TModel ModelSnapshot { get; private set; }

        public bool IsChanged(TModel model)
        {
            var snapshot = Snaphsot.From(model);
            if (Changes.Between(ModelSnapshot, snapshot) == null) return false;
            ModelSnapshot = snapshot;
            return true;
        }
    }
}
