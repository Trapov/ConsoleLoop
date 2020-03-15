using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLoop
{
    public sealed class TaskBasedRenderingLoop<TModel, TView> : IRenderingLoop, IDisposable
        where TView : IView<TModel>
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IInputEventDispatcher _inputEventDispatcher;
        public IChangesTrackerFor<TModel> Tracker { get; }

        internal TaskBasedRenderingLoop(TModel model, IChangesTrackerFor<TModel> tracker, TView view, IInputEventDispatcher inputEventDispatcher, CancellationToken cancellationToken)
        {
            View = view;
            Tracker = tracker;
            Model = model;
            _inputEventDispatcher = inputEventDispatcher;
            _cancellationToken = cancellationToken;
            RunningTask = Loop();
        }

        public RenderingLoopStatuses Status { get; private set; } = RenderingLoopStatuses.NotActive;
        public TView View { get; }
        public TModel Model { get; }
        public Task RunningTask { get; }

        private static readonly TimeSpan LoopDelay = TimeSpan.FromMilliseconds(10);

        public void Dispose()
        {
            Status = RenderingLoopStatuses.Terminated;
            RunningTask.Dispose();
        }

        private async Task Loop()
        {
            Console.Clear();
            var viewRendered = View.Render(Model);
            Console.Write(viewRendered);

            while (!_cancellationToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                    _inputEventDispatcher.DispatchKeyEvent(Console.ReadKey(true));

                if (Tracker.IsChanged(Model))
                {
                    Console.Clear();
                    viewRendered = View.Render(Model);
                    Console.Write(viewRendered);
                }

                await Task.Delay(delay: LoopDelay,
                                 cancellationToken: _cancellationToken).ConfigureAwait(false);
            }
        }

        public void Block() => RunningTask.Wait(_cancellationToken);
    }
}
