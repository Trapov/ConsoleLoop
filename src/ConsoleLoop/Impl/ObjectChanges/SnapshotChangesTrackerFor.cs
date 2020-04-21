using System;
using System.Threading;

namespace ConsoleLoop
{
    public sealed class SnapshotChangesTrackerFor<TModel> : IChangesTrackerFor<TModel>
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public SnapshotChangesTrackerFor(TModel model, SemaphoreSlim semaphoreSlim)
        {
            ModelSnapshot = Snaphsot.From(model);
            _semaphoreSlim = semaphoreSlim;
        }

        public TModel ModelSnapshot { get; private set; }

        public bool IsChanged(TModel model)
        {
            TModel snapshot = default;
            try
            {
                _semaphoreSlim.Wait();
                snapshot = Snaphsot.From(model);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            if (Changes.Between(ModelSnapshot, snapshot) == null) return false;
            ModelSnapshot = snapshot;
            return true;
        }
    }
}
