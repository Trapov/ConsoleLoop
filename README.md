# Update
The project was unsuccessful because I had to use **locks** to prevent different threads from accessing the model. I don't like the design I end up with in the end so **I'm dropping** this project. It works as expected but not as clever as the other frameworks. Fell free to do whatever you want with it.

# ConsoleLoop. Loooooop.

Simple console loop with with **model**(view-model) and **view** features.

## Packages Builds
[![Build Status](https://dev.azure.com/trapov/ConsoleLoop/_apis/build/status/Trapov.ConsoleLoop?branchName=master)](https://dev.azure.com/trapov/ConsoleLoop/_build/latest?definitionId=1&branchName=master) 

[![](https://img.shields.io/nuget/v/ConsoleLoooooop?style=for-the-badge)](https://www.nuget.org/packages/ConsoleLoooooop/)
## How does it work?

![](ConsoleLoopDiagram.png)
**View** gets called with the **Model** as an argument with each CHANGE in the **Model**. (in other words it is reactive) and returns a string to write it to the stdout.

Declare a model, view and create a looop.
```csharp
var Model = new FileMergeSortModel();

// have to use it
SemaphoreSlim semaphoreSlim = null;
// bad design ^^

var renderingLoop = new ConsoleLoopBuilder(cancellationToken: CancellationTokenSource.Token)
         .WithInputEventHandler(keyInfo => keyInfo.Key == System.ConsoleKey.UpArrow, _ =>
         {
             Model.Skip -= 10;
          })
		  .WithInputEventHandler(keyInfo => keyInfo.Key == System.ConsoleKey.DownArrow, _ =>
		  {
		    Model.Skip += 10;
		  })
          .Model(Model, out semaphoreSlim)
         .ToView<FileMergeSortView>()
         .WithLoop<TaskBasedRenderingLoop<FileMergeSortModel, FileMergeSortView>>();
```

## Example of using

Example using this library in external merge sorting. 
![](https://github.com/Trapov/merge-sort-files/raw/master/example.gif)
