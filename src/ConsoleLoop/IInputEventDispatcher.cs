using System;

namespace ConsoleLoop
{
    public interface IInputEventDispatcher
    {
        public class Handler
        {
            public Handler(Action<ConsoleKeyInfo> handle, Func<ConsoleKeyInfo, bool> needToHandle)
            {
                Handle = handle;
                NeedToHandle = needToHandle;
            }

            public Action<ConsoleKeyInfo> Handle { get; }
            public Func<ConsoleKeyInfo, bool> NeedToHandle { get; }
        }

        void RegisterHandler(Handler handler);
        void DispatchKeyEvent(ConsoleKeyInfo consoleKeyInfo);
    }
}