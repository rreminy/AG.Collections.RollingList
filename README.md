# RollingList
A limited capacity rolling list, implemented using an ordinary List<T>.

Ever wanted to add a lots of items but only care about the last few of them added? Rolling list allows you to do this. Once the list is full, newly added items will roll the rest of the list over.


## Usage
```cs
using AG.Collections;

var list = new RollingList<int>(5) { 10, 15, 20, 25, 30, 35 };
Console.WriteLine(string.Join(", ", list));
// Output: 15, 20, 25, 30, 35
```

## License
MIT

## Credits
Azure Gem
