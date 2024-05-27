
# ASP.NET REST API

## Overview

This project is a sample ASP.NET Core REST API application, demonstrating the implementation of a web API using ASP.NET Core 3.x. It includes various features like CRUD operations, data validation, and entity management.

## Features

- CRUD operations for managing entities
- Data validation and error handling
- DTO (Data Transfer Objects) and parameter binding
- Dependency injection and service configuration

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) 3.x or later
- A database (e.g., SQL Server, SQLite)

## Setup

1. Clone the repository:
   ```sh
   git clone https://github.com/ElaineLi0320/ASP.NET-REST-API.git
   cd ASP.NET-REST-API
   ```

2. Restore dependencies:
   ```sh
   dotnet restore
   ```

3. Update the database:
   ```sh
   dotnet ef database update
   ```

4. Run the application:
   ```sh
   dotnet run
   ```

## Project Structure

- **Controllers**: Contains the API controllers.
- **Data**: Contains the DbContext and data-related configurations.
- **DtoParameters**: Contains DTOs and parameter classes.
- **Entities**: Contains the entity models.
- **Services**: Contains service classes for business logic.

## Usage

Use tools like Postman or cURL to interact with the API endpoints. For example:

```sh
curl -X GET http://localhost:5000/api/entities
```

## Contributing

Contributions are welcome! Please create an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License.
