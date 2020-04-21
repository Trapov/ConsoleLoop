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
            try
            {
                _semaphoreSlim.Wait();
                var snapshot = Snaphsot.From(model);
                if (Changes.Between(ModelSnapshot, snapshot) == null) return false;
                ModelSnapshot = snapshot;
                return true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }
    }
}
