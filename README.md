# SoccerOpenServer ⚽

SoccerOpenServer is a **REST API backend** built with **ASP.NET Core** that provides the server-side infrastructure for **soccer tactic and team management**.
It handles data storage, business logic, authentication, and serves API endpoints for managing teams, players, tactics, and competitions.

This project uses **.NET 10.0** with **Entity Framework Core** and **SQL Server**.

---

## Project Overview

SoccerOpenServer represents the **backend part** of the Soccer Open Simulator (SoccerOS) project.
Its main goal is to provide a robust and secure API for managing soccer-related data and operations.

The application provides:
- RESTful API endpoints for soccer data management
- User authentication and authorization with JWT tokens  
- Database management for teams, players, tactics, and competitions  
- Business logic for team generation and tactical configurations  
- CORS support for frontend integration  

---

## Technologies & Features

The backend is built with modern .NET technologies:

- **ASP.NET Core 10.0** - Web API framework
- **Entity Framework Core 10.0** - ORM for database operations
- **SQL Server** - Relational database (LocalDB for development)  
- **JWT Authentication** - Secure token-based authentication  
- **Swagger/OpenAPI** - API documentation and testing interface  
- **CORS** - Cross-Origin Resource Sharing for frontend integration  

### Key Features

- **Authentication System**: User registration, login, and JWT token management  
- **Team Management**: Create and manage football teams  
- **Player Management**: Handle player data, roles, positions, and statistics  
- **Tactics System**: Define formations and tactical setups  
- **Competition Management**: Organize leagues and tournaments  
- **Contract System**: Manage player and staff contracts  
- **Staff Management**: Handle coaching and support staff  

---

## Frontend Integration

This backend is designed to work with the **SoccerOpenFrontend** Angular application:

🔗 https://github.com/tom-papaioannou/SoccerOpenFrontend

The frontend is built with **Angular** and provides:
- Interactive user interface for tactic management  
- Visual formation setup tools  
- Team configuration screens  
- Modern web interface for all soccer management features

Running both projects together allows you to explore the **full scope of the Soccer Open Simulator project**, with a clear separation between frontend UI and backend logic.

---

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), version 10.0.400 or a newer patch in the 10.0.4xx feature band (see `global.json`)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)  
- A code editor (Visual Studio, VS Code, or Rider)  

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/tom-papaioannou/SoccerOpenServer.git
   cd SoccerOpenServer
   ```

2. **Configure the database connection**
   
   Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FootballOpenDatabase;Trusted_Connection=True;"
   }
   ```

3. **Configure JWT settings**
   
   Add JWT configuration to `appsettings.Development.json`:
   ```json
   "Jwt": {
     "Key": "your-base64-encoded-secret-key",
     "Issuer": "SoccerOpenServer",
     "Audience": "SoccerOpenFrontend"
   }
   ```

4. **Apply database migrations**
   ```bash
   dotnet tool restore
   dotnet ef database update --project SoccerOpenServer
   ```

5. **Run the development server**
   ```bash
   dotnet run --project SoccerOpenServer
   ```

   The API will be available at:
   - HTTPS: `https://localhost:7000`
   - HTTP: `http://localhost:5000`
   - Swagger UI: `https://localhost:7000/swagger`

---

## API Documentation

Once the server is running, you can access the interactive API documentation:

- **Swagger UI**: Navigate to `https://localhost:7000/swagger`  

This provides a complete overview of all available endpoints, request/response models, and allows you to test the API directly from your browser.

---

## Project Structure

```
SoccerOpenServer/
├── Controllers/          # API endpoint controllers
│   ├── AuthController.cs
│   ├── TeamsController.cs
│   ├── TacticsController.cs
│   └── ...
├── Models/              # Data models and DTOs
│   ├── People/
│   ├── Teams/
│   ├── Competitions/
│   └── ...
├── Context/             # Entity Framework DbContext
├── Services/            # Business logic services
├── Migrations/          # EF Core database migrations
└── Program.cs           # Application entry point
```

---

## Development

### Running Migrations

To create a new migration after model changes:
```bash
dotnet tool restore
dotnet ef migrations add MigrationName --project SoccerOpenServer
dotnet ef database update --project SoccerOpenServer
```

### Building the Project

```bash
dotnet restore SoccerOpenServer.sln
dotnet build SoccerOpenServer.sln --configuration Release --no-restore
dotnet publish SoccerOpenServer --configuration Release --no-restore
```

### Running Tests

```bash
dotnet test SoccerOpenServer.sln --configuration Release --no-build
```

This checkout contains no .NET test project. The test command alone does not establish test coverage or a passing test suite.

### .NET 10 deployment

Framework-dependent deployments require the ASP.NET Core 10 runtime; IIS deployments require the .NET 10 Hosting Bundle. Use a Visual Studio release that supports SDK 10.0.400, or the .NET CLI. No Docker or CI/CD configuration is included in this repository.

The framework upgrade does not introduce a database migration. Existing migrations remain unchanged. EF collection queries retain their previous OPENJSON translation. Before rollout, validate authentication, registration, team/tactic operations and database queries against a disposable copy of the database; application startup performs seed writes.

See [the migration assessment and validation report](NET10-MIGRATION.md) for compatibility details and remaining validation limits.

---

## License

This project is licensed under the **MIT License**.

Copyright (c) 2026 Tom Papaioannou

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

See the [LICENSE](LICENSE) file for full details.

---

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

---

## Contact

For questions or feedback, please open an issue on GitHub.
