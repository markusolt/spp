# Simple Pre Processor

*SPP* is a simple pre processor that can be used to create templates for languages that have insufficient templating support.

It can be used to automatically construct sql views that have obvious repeating patterns and would be tedious and error prone to maintain by hand.

To do this *SPP* supports object and array values and allows enumeration of either. In addition to this external `.spp` files can be included at any time to create templates and create an easy to use file structure.

Comments are lines prefixed with `--`, range comments do not exist.

The current implementaion of *SPP* lacks some obvious functions as I am adding them as I need them.
Unfortunately I now want to quickly shut this project down and focus on other things.
Adding new functions or automatic variables is easy, assuming you are willing to recompile the project.

Navigate to the file `/src/spp/Instruciton.cs` or `/src/spp/data/Auto.cs`.
At the top of these files you will find a `static readonly Dictionary`.
This is the main index of all known functions and automatic variables.

Adding a new function simply means adding a new entry to this dictionary and creating an appropriately named static method below.
Use the existing methods as a reference.
Notice that for functions there are two valid method signatures.

The simple signature
```csharp
private static Value _get (Compiler compiler, Value v1, Value v2) {
  return v1[v2.AsString()];
}
```
is given fully evaluated values `v1` and `v2`.
This limits you to two arguments.
Use `AsString()`, `AsInt()`, `AsBool()` or `AsEnumerable()` to require a value to be of this type.

The complex signature
```csharp
private static Value _if (Compiler compiler, Expression[] nodes) {
  if (nodes[0].Evaluate(compiler).AsBool()) {
    return nodes[1].Evaluate(compiler);
  }

  if (nodes[2] != null) {
    return nodes[2].Evaluate(compiler);
  }

  return Value.Empty;
}
```
is given unevaluated expressions in the form of an `Expression[]`.
Make sure to evaluate these expressions before using them as values.
This is needed when not wanting to evaluate some arguments or when exceeding the limit of two arguments.

## Example

```spp
--
-- This is a comment.
--

# output("out.txt")
This text will be copied into out.txt.

-- To define variables use
# let(foo, "bar")
We can now reference the variable foo by writing $foo or $(foo).
In the output file this will read as "...writing bar or bar."

# close()
-- Any additional text here would raise an error as the output file was closed.
-- Open a new output file using
# output("out2.txt")

-- spp also supports arrays and objects using [] and {} similar to json.
# let(myList, [4, 5, 6])
# for(i, myList, [output("out$i.txt"), write("Hello World for the $(i)-th time!"), close()])

-- To insert an external spp file at this position use
# input("other.spp")

-- to load an spp template you could write all arguments into an object and input the spp file using these arguments
# let(args, {"name": "demo", "count": 3})
# using(args, input("template.spp"))
-- inside of template.spp you can now refer to the arguments as the variables name and count.
-- variables like myList will also still be accessible, although members of args are given priority.

-- similarly you can specify multple runs of a template using an enclosing array and the for loop
# let(args_list, [
  {"name": "demo_1"},
  {"name": "demo_2"}
])
# for(args, args_list, using(args, input("template.spp")))
```

## Functions

### `basename(filepath)`

Returns the filename without extension of the specified `filepath`.

### `cdinput(path)`

Changes the input directory to the specified `path`.

### `cdoutput(path)`

Changes the output directory to the specified `path`.

### `close()`

Closes the writer to the current output file.

### `equals(v1, v2)`

Compares the values `v1` and `v2` by comparing their string representation.

Although being a potentially slow solution this approach yields intuitive results.
Perfect copies of the same data structure will result in `true`.

### `error(message)`

Stops compilation and raises an error at the position of `message` using the value of `message` as a message.

### `error(message, v)`

Stops compilation and raises an error using the value of `message` as a message.

The error is raised at the position where `v` was originally defined.
Use this function when the object `v` does not meet your expectations.
The user is then directed to the definition of `v`.

### `files(path)`

Returns a list of filepaths matching the specified `path`.
The argument `path` may contain wildcards.

Use this function to get a list of all files in a directory with a specified prefix and extension by calling `files("demo/tmp_*.spp")`.
This will return the list `["C:/[...]/demo/tmp_one.spp", "C:/[...]/demo/tmp_two.spp"]`.

### `for(var, list, expr)`

The expression `expr` will be execute for each value in `list`.
The argument `list` may be an array or an object.
The variable `var` will be set to the current value and can be referenced from within `expr`.

Use an array of expressions in place of `expr` if multiple expressions are needed.
Alternatively write the expressions in an external file and use the `input` function as `expr` to load the file for each iteration.
This will parse the external file once per iteration which may slow down compilation.

### `get(obj, key)`

Returns the value of the entry of `obj` with the key `key`.
Raises an error if no such entry exists.

### `if(b1, expr)`

The expression `expr` will be executed only if `b1` is `true`.

### `if(b1, expr1, expr2)`

The expression `expr1` will be executed only if `b1` is `true`, otherwise the expression `expr2` is executed.

### `input(filepath)`

The compiler processes the external file `filepath` before resuming the current process.

### `isnull(val)`

Returns a `true` if `val` is null.

### `isnull(v1, v2)`

Returns `v1` unless it is null in which case it return `v2`.

### `let(var, val)`

Creates the variable `var` and sets it to the value `val`.

### `loadtext(filepath)`

Returns a text value representing the contents of the file at `filepath`.

### `loadvalue(filepath)`

Loads the first spp value specified in the file at `filepath`.
The loaded file should like as follows.

```spp

-- initial whitespace and comments

# {
	"message": "this value will be returned by loadvalue"
}

-- anything after the first value is ignored

# [
	"this array is ignored"
]
```

### `not(b1)`

Returns the inverse of the bool `b1`.

### `output(filepath)`

Creates a writer writing to the file `filepath`.
After this function was called content may be specified between spp statements.

```spp
-- do not write any content here

# output("out.txt")

This text will be writting into the file 'out.txt'.

# let(foo, "Hello World.")

Any mentions of variables such as $(foo) will be replaced.
```

### `push(collection, entry)`

Adds the entry `entry` to the collection `collection`.

If `collection` is an array `entry` can be any value.
If `collection` is an object then `entry` has to be a key value pair.
A key value pair is an object with the entries `key` and `value`.

```spp
# let(obj, {})
# push(obj, {"key": "foo", "value": "bar"})

# equals(
  obj,
  {"foo": "bar"}
) --> true
```

### `using(obj, expr)`

Any variable names mentioned in `expr` will first look for a member of that name within `obj`.

This function can be nested.

```spp
# let(one, 1)

# equals(
  using({"two": 2}, using({"three": 3}, [one, two, three])),
  [1, 2 ,3]
) --> true
```

### `warn(message)`

Prints the message `message` with the prefix `Warning: ` to the console output.

### `write(text)`

Writes the text `text` to the currently selected writer.
Raises an error if there is no active writer.

## Automatic Variables

### `false`

Returns the boolean `false`.

### `null`

Returns the null value.

```spp
# isnull(null) --> true
```

### `this`

Refers to the currently used object by the enclosing `using` function.

```spp
# using({"foo": "bar"}, error("missing entries", this))
```

### `true`

Returns the boolean `true`
