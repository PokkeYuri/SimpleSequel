<p align="center">
  <a>
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://github.com/PokkeYuri/SimpleSequel/assets/86960788/21096292-0d1c-4217-bce9-cb653fff301e">
      <img src="https://github.com/PokkeYuri/SimpleSequel/assets/86960788/21096292-0d1c-4217-bce9-cb653fff301e" height="128">
    </picture>
  <h1 align="center">Simple Sequel</h1>
  </a>
</p>


SimpleSequel is a lightweight .NET library designed to simplify interaction with databases. It provides intuitive extensions for executing SQL commands and queries and is built on top of ADO.NET.

This library requires .NET 8.

## Features

- Fetch and execute from the database with minimal boilerplate code.
- Execute SQL statements directly from string objects.
- Easily convert database rows into .NET objects or specific classes.
- Automatically handles the opening and closing of your database connection as necessary.

<!--- 
## Installation

To use SimpleSequel in your project, install it via NuGet:

```bash
Install-Package SimpleSequel
```
-->

## Usage

Before using SimpleSequel in your project, ensure you have a database that is compatible with ADO.NET, as the library utilizes `System.Data.Common.DbConnection` class from the ADO.NET framework for database connections. This step is essential for setting up the library to work seamlessly with your database management system (DBMS).

```csharp
using SimpleSequel;

// Initialize SimpleSequel with your DBMS connection
SimpleSequelManager.Initialize(DbConnection.Connection);
```
Below are some examples of how to use SimpleSequel extensions in your .NET applications.
For more usage and examples please refer to the tests included in the 'SqlExtensionsTests' project within this repository.

### Executing a SQL statement
```cs
// Execute a statement
"INSERT INTO KeyValueTable VALUES ( 'Gandalf', 'Wizard' )".ExecuteStatement();

// Execute a statement async
await "INSERT INTO KeyValueTable VALUES ( 'Gandalf', 'Wizard' )".ExecuteStatementAsync();
```

### Execute and getting a custom value
```cs
// Execute a command and get a scalar value
var value = "SELECT Value FROM KeyValueTable WHERE Key = 'Gandalf'".ExecuteScalar<int>();

// Execute a command asynchronously and get a scalar value
var valueAsync = await "SELECT Value FROM KeyValueTable WHERE Key = 'Gandalf'".ExecuteScalarAsync<int>();
```

### Executing for SQL query
```cs
// Execute a query and get a DbDataReader
DbDataReader reader = "SELECT * FROM KeyValueTable".ExecuteReader();

// Execute a query asynchronously and get a DbDataReader
DbDataReader readerAsync = await "SELECT * FROM KeyValueTable".ExecuteReaderAsync();
```

### Execute and getting an object from custom class
```cs
class KeyValue
{
    public string Key { get; set; }
    public string Value { get; set; }
}

// Execute and get only first row as custom object
KeyValue keyValue = "SELECT * FROM KeyValueTable ORDER BY Key".ExecuteClass<KeyValue>();
```

## Todo's

- Add XML documentation comments
- Complete support for both synchronous and asynchronous operations
- Implement logging
- Complete CRUD operations for ORM
- Implement methods using DbParameters to prevent SQL Injections

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
