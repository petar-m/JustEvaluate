
# JustEvaluate  

[![build](https://github.com/petar-m/JustEvaluate/actions/workflows//build.yml/badge.svg)](https://github.com/petar-m/JustEvaluate/actions)
[![NuGet](https://img.shields.io/nuget/v/JustEvaluate.svg)](https://www.nuget.org/packages/JustEvaluate)  

## What does it do?

Evaluates math expressions like  

```
(1.34 * MyArgument) / 2 + MyFunction(4, MyArgument)
```  

## How does it do it?  

The expression is parsed, converted to expression tree, compiled and the result delegate is cached.

## What Expressions will it understand?  

The syntax is very narrow:  
 - numbers with `.` for decimal separator
 - operators `* / + - `
 - boolean expressions `&` (logical AND), `|` (logical OR)  
 - relational expressions `>` (greater than), `>=` (greater or equal to), `<` (less than), `<=` (less or equal to)  
 - equality expressions `=` (equal to), `<>` (not equal to)
 - brackets `()` depending on context determine precedence of operations or enclose function arguments
 - function arguments separator `,`
 - arguments
 - built-in functions  
 - user-defined functions
  
### Everything is Decimal  

Expressions deal with only one type: `decimal`. Function parameters and return values, arguments, numeric literals - everything is `decimal`. Result of expression evaluation is also a `decimal`. 

### Operator Precedence  

| higher to lower   |
| ----------------- |
| `*` `/`           |
| `+` `-`           |
| `>` `<` `<=` `>=` |
| `=` `<>`          |
| `&`               |
| `\|`              |


### Boolean Expressions  

Since the operands of boolean expression are decimals, they are evaluated as follows: 
 - non-zero values are treated as **true**
 - zero values are treated as **false**  
 - boolean logic is applied to produce **1** or **0** of type `decimal`  

### Relational and Equality Expressions  

Similar to boolean expressions, relational and equality expressions produce **1** or **0** of type `decimal`  

### Arguments  

Arguments are passed as properties of custom or anonymous type returning `decimal`. Argument names are case insensitive.

```csharp
var result = evaluator.Evaluate("x + 1", new { X = 1m });
```  
or  

```csharp
class Args
{
    ...
    public decimal X { get; }
}
...
var result = evaluator.Evaluate("x + 1", new Args{ X = 1 });
```   

#### (1.1.0) Arguments as Dictionary  

`Dictionary<string, decimal>` and `IDictionary<string, decimal>` are supported as an argument. When building expression the dictionary indexer is used instead of property getter.  

```csharp
var result = evaluator.Evaluate("x + 1", new Dictionary<string, decimal>{ {"x", 1} });
```   
There are some caveats to have in mind:  
- only `Dictionary<string, decimal>` and `IDictionary<string, decimal>` are supported
- any dictionary with other key/value types than `string`/`decimal` will be treated as regular object and a property with the given name will be looked for
- case sensitivity of arguments will be determined by the dictionary instance   

### Built-In Functions  

Functions that are "known" and can be used without additional setup. Their names are "reserved" and cannot be overwritten by user-defined ones.  
Curently supported are `if` and `not`.  
[more](https://github.com/petar-m/JustEvaluate/wiki/Utility-Functions)  


### User-Defined Functions  

Functions are defined as `Expression<Func<decimal>>`. They always return `decimal` and can have 0 to 8 parameters, again `decimal`. Function should be registered before usage.  

```csharp
var registry = new FunctionsRegistry().Add("MyFunction", (x, y) => x * y + 1);
var evaluator = new Evaluator(new Parser(), new Builder(registry), new CompiledExpressionsCache());
...
decimal result = evaluator.Evaluate("MyFunction(2, 3)"); // result = 7
```  
Function names  are case insensitive and can consist of anything but special characters `+ - * / . , ( ) > < = | &`   
There are some functions defined and ready for use - check out [Utility Functions](https://github.com/petar-m/JustEvaluate/wiki/Utility-Functions)  

### Aliases  

Type properties can be referenced by multiple names using `AliasAttribute`.
Aliases should be unique, cannot be the same as a property name, and cannot contain any special characters `+ - * / . , ( ) > < = | &`. 
  
```csharp
class Args
{
    [Alias("Some cool name")]
    [Alias("Another cool name")]
    public decimal X { get; }
}

var result = evaluator.Evaluate("Some cool name + Another cool name", new Args{ X = 1m }); // -> 2
```  

Functions can also have aliases, built-in and user defined both. Aliases should be unique, can not be the same as already defined function, and can not contain special characters.  

```csharp
var registry = new FunctionsRegistry().Add("MyFunction", (x, y) => x * y + 1)
                                      .AddFunctionAlias(function: "MyFunction", alias: "Multiply and Add One")
                                      .AddFunctionAlias(function: "if", alias: "iif");;
var evaluator = new Evaluator(new Parser(), new Builder(registry), new CompiledExpressionsCache());
...
decimal result = evaluator.Evaluate("IIF(Multiply and Add One(2, 3), 2, 1)"); // result = 2
```  


Aliases for both arguments and functions are case insensitive.  

#### (1.2.0) Option for Text Boolean Operators (`AND`, `OR`)  

In addition to `&` and `|` using `AND` and `OR` is also supported if enabled by `ParserOptions`:   

```csharp
var parser = new Parser(new ParserOptions { EnableAndAsText = true, EnableOrAsText = true});
...
var result = evaluator.Evaluate("0 or 1 and 1");
```  

`AND` and `OR` are case insensitive.  
They are not part of the parsing logic, instead they are directly replaced before parsing (`' AND '` with  `' & '`, `' OR '` with `' | '`).
This may cause invalid expression if there are arguments named `AND` or `OR` because if surrounded by whitespace they will be replaced.  
Since whitespace in not a terminal character, in example the expression `black and white` may be an argument name. But if  `Parser.EnableAndAsText = true`
 it will become `black & white` which in this case will be wrong.



### Caching  

Parsing and compiling expression trees is costly, so once compiled the resulting delegate is cached with the expression itself as a key.  

**Note** the cache is unbounded at this time so it can be essentially a memory leak if using ad-hoc expressions.  

### Examples  

Check out the [examples](https://github.com/petar-m/JustEvaluate/tree/main/test/JustEvaluate.Examples)

## Performance Considerations  

Coded calculation will **always** outperform this library.  

1. Parsing and compiling the expression is the most expensive operation - this is why compiled expressions are cached. Use single instance of the evaluator - it has no other state but the cache anyway.  
2. Parametrize expressions when possible.
3. There is a cost of cache lookup.
4. A [benchmark](https://github.com/petar-m/JustEvaluate/blob/main/test/JustEvaluate.Benchmark/BasicBenchmark.cs) for very simple expression shows ~2 times slower performance (mostly because of cache lookup). For more complicated expressions this may become way much slower.


Even at least twice slower, this is still in the realm of nanoseconds. It may or may not be a problem, depending on the application.  

As  with everything performance related - always measure your concrete case. 