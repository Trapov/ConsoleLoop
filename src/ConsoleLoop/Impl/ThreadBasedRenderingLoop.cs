using System;
using System.Threading;

namespace ConsoleLoop
{
    public sealed class ThreadBasedRenderingLoop<TModel, TView> : IRenderingLoop, IDisposable
        where TView : IView<TModel>
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IInputEventDispatcher _inputEventDispatcher;

        public TView View { get; }
        public TModel Model { get; }
        public IChangesTrackerFor<TModel> Tracker { get; }

        internal ThreadBasedRenderingLoop(TModel model, IChangesTrackerFor<TModel> tracker, TView view, IInputEventDispatcher inputEventDispatcher, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            View = view;
            Model = model;
            Tracker = tracker;
            _inputEventDispatcher = inputEventDispatcher;
            Status = RenderingLoopStatuses.Running;

            RunningThread = new Thread(Loop)
            {
                Name = nameof(ThreadBasedRenderingLoop<TModel, TView>),
                IsBackground = true,
                Priority = ThreadPriority.Highest,
            };

            RunningThread.Start();
        }

        public RenderingLoopStatuses Status { get; private set; } = RenderingLoopStatuses.NotActive;

        internal Thread RunningThread { get; private set; }
        public void Block() => RunningThread.Join();

        public void Dispose()
        {
            Status = RenderingLoopStatuses.Terminated;
            RunningThread.Join(TimeSpan.FromMilliseconds(10));
        }

        private void Loop()
        {
            Console.Clear();
            var viewRendered = View.Render(Model);
            Console.Write(viewRendered);

            while (!(_cancellationToken.IsCancellationRequested || Status == RenderingLoopStatuses.Terminated))
            {
                if (Console.KeyAvailable)
                    _inputEventDispatcher.DispatchKeyEvent(Console.ReadKey(true));

                if (Tracker.IsChanged(Model))
                {
                    Console.Clear();
                    viewRendered = View.Render(Model);
                    Console.Write(viewRendered);
                }

                Thread.Sleep(10);
            }
        }
    }
}
