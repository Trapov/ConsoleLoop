using ConsoleLoop;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RenderMePlease
{
    public static class Program
    {
        public sealed class MyModel
        {
            public string Last { get; set; }
            public List<string> Collection { get; set; }
        }

        public sealed class MyView : IView<MyModel>
        {
            public string Render(MyModel model)
            {
                return $"Last: {model.Last} | Count: {model.Collection.Count()}";
            }
        }

        public static async Task Main()
        {
            var model = new MyModel
            {
                Collection = new List<string>()
                {
                    "1", "2", "3"
                }
            };
            var cancelletionTokenSource = new CancellationTokenSource();
            new ConsoleLoopBuilder(false, cancelletionTokenSource.Token)
                .WithInputEventHandler(keyInfo => keyInfo.Key == System.ConsoleKey.C && keyInfo.Modifiers == System.ConsoleModifiers.Control, keyInfo =>
                {
                    cancelletionTokenSource.Cancel();
                })
                .WithInputEventHandler(_ => true, keyInfo =>
                {
                    model.Last =
                        string.Concat(
                            keyInfo.Modifiers.ToString().InRed(),
                            " + ",
                            keyInfo.Key.ToString().InGreen()
                        );
                })
                .Model(model)
                .ToView<MyView>()
                .WithLoop<TaskBasedRenderingLoop<MyModel, MyView>>()
                .Block();
        }
    }
}
