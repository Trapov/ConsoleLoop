using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ConsoleLoop
{
    internal sealed class DefaultInputEventDispatcher : IInputEventDispatcher
    {
        private readonly IList<IInputEventDispatcher.Handler> _handlers = new List<IInputEventDispatcher.Handler>();
        private readonly Channel<ConsoleKeyInfo> _channel = Channel.CreateBounded<ConsoleKeyInfo>(new BoundedChannelOptions(1)
        {
            AllowSynchronousContinuations = false,
            Capacity = 1,
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = true
        });
        private readonly CancellationToken _cancellationToken;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Task RunningTask { get; }
        public ConsoleKeyInfo? LastWritten = null;

        public bool AllowLongPress { get; }

        public DefaultInputEventDispatcher(bool allowLongPress = false, CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
            AllowLongPress = allowLongPress;
            RunningTask = DispatchingLoop();
        }

        public void DispatchKeyEvent(ConsoleKeyInfo consoleKeyInfo)
        {
            if (AllowLongPress == false)
            {
                _stopwatch.Stop();
                if (LastWritten != null && LastWritten == consoleKeyInfo && _stopwatch.Elapsed < TimeSpan.FromMilliseconds(100))
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    return;
                }
                _stopwatch.Reset();
                _stopwatch.Start();
            }

            LastWritten = consoleKeyInfo;
            _channel.Writer.TryWrite(consoleKeyInfo);
        }

        public async Task DispatchingLoop()
        {
            while (_cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var consoleKeyInfo = await _channel.Reader.ReadAsync(_cancellationToken);
                    foreach (var handler in _handlers)
                    {
                        if (handler.NeedToHandle(consoleKeyInfo))
                            handler.Handle(consoleKeyInfo);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail("Errors while trying to handle the message.", ex.Message);
                }
            }
        }

        public void RegisterHandler(IInputEventDispatcher.Handler handler)
        {
            _handlers.Add(handler);
        }
    }
}
