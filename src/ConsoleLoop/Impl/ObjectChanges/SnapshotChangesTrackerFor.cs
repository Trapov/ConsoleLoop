namespace ConsoleLoop
{
    public sealed class SnapshotChangesTrackerFor<TModel> : IChangesTrackerFor<TModel>
    {
        private TModel _model;

        public SnapshotChangesTrackerFor(TModel model)
        {
            _model = Snaphsot.From(model);
        }

        public bool IsChanged(TModel model)
        {
            var snapshot = Snaphsot.From(model);
            if (Changes.Between(_model, snapshot) == null) return false;
            _model = snapshot;
            return true;
        }
    }
}
