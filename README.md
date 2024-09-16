# QueryDictionary

## Introduction
QueryDictionary is a library for managing and loading SQL queries from various sources such as embedded resources, folders, and XML files. It provides a convenient way to organize and access your SQL queries in a structured manner.

## Prerequisites
This package targets .NET Standard 2.1, so it should work with most .NET platforms.

## Installation
Use NuGet to install the package. Use can use the UI, or use the following command in the package manager console:
```
Install-Package QueryDictionary
```

or

use the following command in the dotnet CLI:
```
dotnet add package QueryDictionary
```

## Contributing
If you want to contribute, please create a pull request. I will review it as soon as possible.
Use visual studio 2022 version 17.8 or later to build this library. The main library targets NETStandard 2.0, but the tests use .NET 8.0.

## Author
This library was created by Jan Geert Hek, a software developer from the Netherlands. You can find more information about me on my [LinkedIn](https://www.linkedin.com/in/jghek/) page.

## License
This library is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

# Part 1. Loading queries from various sources
## Loading Queries from an Embedded Resource
You can load SQL queries embedded in an assembly using the QueryDictionaryEmbedded class.

```csharp
using QueryDictionary;

// Load queries from the assembly containing the specified type
var queryDictionary = QueryDictionaryEmbedded.LoadAssemblyOfType<SomeType>(namespacePrefix: "YourNamespace.Queries");

// Access a query by its key
string query = queryDictionary["GetOneClient"];
```

## Loading Sql Server Query files from an Embedded Resource
You can also load SQL queries embedded in an assembly with a specific namespace prefix.

```csharp
using QueryDictionary;

// Load queries from the assembly containing the specified type with SQL queries
var queryDictionary = QueryDictionaryEmbedded.LoadAssemblyOfTypeWithSqlQueries<SomeType>(namespacePrefix: "YourNamespace.Queries");

// Access a query by its key
string query = queryDictionary["GetOneClient"];
```

## Loading Queries from a Folder
You can load SQL queries from a folder using the QueryDictionaryFolder class.

```csharp
using QueryDictionary;

// Load queries from a folder
var queryDictionary = QueryDictionaryFolder.LoadSqlServerQueryFolder("path/to/your/folder");

// Access a query by its key
string query = queryDictionary["GetOneClient"];
```

## Loading Queries from an XML File
You can load SQL queries from an XML file using the QueryDictionaryXml class.

```csharp
using QueryDictionary;

// Load queries from an XML file
var queryDictionary = QueryDictionaryXml.ForFile("path/to/your/queries.xml");

// Access a query by its key
string query = queryDictionary["GetOneClient"];
```

# Part 2. Using the QueryDictionary
## Use your DI to inject the QueryDictionary

```csharp
// Load queries from embedded files.
var queryDictionary = QueryDictionaryEmbedded.LoadAssemblyOfTypeWithSqlQueries<SomeType>(namespacePrefix: "YourNamespace.Queries");

// Register the query dictionary as a singleton service
services.AddSingleton<IQueryDictionary>(queryDictionary);
```

### Use the QueryDictionary in your services
QueryDictionary works well with Dapper, a micro ORM for .NET. You can use the QueryDictionary to load SQL queries and execute them with Dapper.

```csharp
public SomeController(IQueryDictionary queryDictionary)
{
    _queryDictionary = queryDictionary;
}

[HttpGet("something/{id}")]
public IActionResult GetSomething(int id)
{
    string query = _queryDictionary["GetSomething"];

    using (var connection = new SqlConnection(...))
    {
        connection.Open();
        var result = connection.Query(query, new { MyThingId = id });
        return Ok(result);
    }
}
```

## Credits
The icon used has been designed by Flaticon.com