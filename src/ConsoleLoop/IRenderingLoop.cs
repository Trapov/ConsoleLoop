using System;
using System.Reflection;
using System.Threading;

namespace ConsoleLoop
{
    public interface IRenderingLoop
    {
        RenderingLoopStatuses Status { get; }
        void Block();
    }

    public sealed class ConsoleLoopBuilder
    {
        public ConsoleLoopBuilder(bool allowLongPress = false, CancellationToken cancellationToken = default)
        {
            EventDispatcher = new DefaultInputEventDispatcher(allowLongPress, cancellationToken);
            CancellationToken = cancellationToken;
        }

        public IInputEventDispatcher EventDispatcher { get; }
        public CancellationToken CancellationToken { get; }


        public ConsoleLoopBuilder WithInputEventHandler(Func<ConsoleKeyInfo, bool> needToHandle, Action<ConsoleKeyInfo> handler)
        {
            Console.TreatControlCAsInput = true;
            EventDispatcher.RegisterHandler(new IInputEventDispatcher.Handler(handler, needToHandle));
            return this;
        }

        public ConsoleLoopBuilder<TModel> Model<TModel>(TModel model, out SemaphoreSlim semaphoreSlim)
        {
            semaphoreSlim = new SemaphoreSlim(1, 1);
            return new ConsoleLoopBuilder<TModel>(model, EventDispatcher, semaphoreSlim, CancellationToken);
        }
    }

    public sealed class ConsoleLoopBuilder<TModel>
    {
        public ConsoleLoopBuilder(
            TModel model,
            IInputEventDispatcher inputEventDispatcher,
            SemaphoreSlim semaphoreSlim,
            CancellationToken cancellationToken = default)
        {
            Model = model;
            Tracker = new SnapshotChangesTrackerFor<TModel>(model, semaphoreSlim);
            EventDispatcher = inputEventDispatcher;
            CancellationToken = cancellationToken;
        }

        public TModel Model { get; }
        public IChangesTrackerFor<TModel> Tracker { get; }
        public IInputEventDispatcher EventDispatcher { get; }
        public CancellationToken CancellationToken { get; }

        public ConsoleLoopBuilder<TModel, TView> ToView<TView>()
            where TView : IView<TModel>, new()
        {
            return new ConsoleLoopBuilder<TModel, TView>(Model, Tracker, new TView(), EventDispatcher, CancellationToken);
        }
    }

    public sealed class ConsoleLoopBuilder<TModel, TView>
     where TView : IView<TModel>
    {
        public ConsoleLoopBuilder(TModel model, IChangesTrackerFor<TModel> tracker, TView view, IInputEventDispatcher inputEventDispatcher, CancellationToken cancellationToken = default)
        {
            Model = model;
            View = view;
            Tracker = tracker;
            EventDispatcher = inputEventDispatcher;
            CancellationToken = cancellationToken;
        }

        public TRenderingLoop WithLoop<TRenderingLoop>()
            where TRenderingLoop : IRenderingLoop, IDisposable =>
            (TRenderingLoop)Activator.CreateInstance(typeof(TRenderingLoop), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { Model, Tracker, View, EventDispatcher, CancellationToken }, null);

        public TModel Model { get; }
        public IChangesTrackerFor<TModel> Tracker { get; }
        public TView View { get; }
        public IInputEventDispatcher EventDispatcher { get; }
        public CancellationToken CancellationToken { get; }

    }
}
