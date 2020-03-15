# ConsoleLoop. Loooooop.

Simple console loop with with **model**(view-model) and **view** features.

## How does it work?

![](ConsoleLoopDiagram.png)

**View** gets called with the **Model** as an argument with each CHANGE in the **Model**. (in other words it is reactive) and returns a string to write it to the stdout.
