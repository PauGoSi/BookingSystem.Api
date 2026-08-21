# BookingSystem.Api

A RESTful booking system built with ASP.NET Core Web API.

This portfolio project demonstrates a production-oriented backend architecture with authentication, authorization, validation, Entity Framework Core, automated tests, continuous integration, and reproducible local development with Docker Compose.

## Features

- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- JWT authentication and role-based authorization
- CRUD operations for users, resources, and bookings
- Booking validation and conflict detection
- Pagination and filtering
- Global exception handling
- Automatic booking completion through a background service
- Swagger / OpenAPI documentation
- xUnit tests
- GitHub Actions CI
- Docker and Docker Compose

## Technologies

- .NET 10
- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Swagger / OpenAPI
- xUnit
- Docker and Docker Compose
- GitHub Actions

## Project Structure

```text
BookingSystem.Api/
├── BackgroundServices/
├── Controllers/
├── Data/
│   └── Seed/
├── DTOs/
│   ├── Auth/
│   ├── Booking/
│   ├── Resource/
│   ├── Role/
│   └── User/
├── Enums/
├── Middleware/
├── Migrations/
├── Models/
├── Services/
├── Dockerfile
└── Program.cs

BookingSystem.Api.Tests/
└── ...

.github/
└── workflows/
    └── ci.yml

compose.yaml
.env.example
.dockerignore
.gitignore
```

## Domain Model

The application contains four main entities:

- **User** – represents a user of the booking system.
- **Role** – defines a user's permissions.
- **Resource** – represents a resource that can be booked.
- **Booking** – connects a user to a resource for a specified period.

Relationships:

- A **User** has one **Role**.
- A **Booking** belongs to one **User**.
- A **Booking** uses one **Resource**.

## ER Diagram

![ER Diagram](docs/er-diagram.png)

## API Endpoints

### Authentication

- `POST /api/auth/register`
- `POST /api/auth/login`

Self-registered users are always assigned the `User` role.

### Bookings

- `GET /api/bookings`
- `GET /api/bookings/{id}`
- `POST /api/bookings`
- `PUT /api/bookings/{id}`
- `PATCH /api/bookings/{id}/cancel`
- `PATCH /api/bookings/{id}/complete`
- `DELETE /api/bookings/{id}`

Booking queries support pagination and filtering:

- `GET /api/bookings?page=1&pageSize=10`
- `GET /api/bookings?resourceId=1`
- `GET /api/bookings?fromDate=2026-05-01&toDate=2026-05-31`
- `GET /api/bookings?status=Active`
- `GET /api/bookings?status=Cancelled`
- `GET /api/bookings?status=Completed`

String values such as `Active`, `Cancelled`, and `Completed` are recommended when filtering by booking status.

Authenticated users can access only their own bookings. Administrators can access all bookings.

### Users

- `GET /api/users`
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

User management endpoints require the `Admin` role.

User creation and updates use a `role` field with the supported values `"Admin"` and `"User"`. Internal database role IDs are not exposed through these requests or responses.

### Roles

- `GET /api/roles`
- `GET /api/roles/{id}`

Role endpoints require the `Admin` role.

`Admin` and `User` are built-in system roles created during Development seeding. Creating, modifying, and deleting roles through the API is intentionally not supported.

### Resources

- `GET /api/resources`
- `GET /api/resources/{id}`
- `POST /api/resources`
- `PUT /api/resources/{id}`
- `DELETE /api/resources/{id}`

Authenticated users can read resources. Creating, updating, and deleting resources requires the `Admin` role.

## Authentication and Authorization

The API uses JWT Bearer authentication.

Authenticate through:

```http
POST /api/auth/login
```

A successful login returns a JWT token. In Swagger:

1. Call `POST /api/auth/login`.
2. Copy the returned token.
3. Select **Authorize**.
4. Enter the token in the authorization dialog.
5. Call the protected endpoints.

## Development Admin Account

When the application runs in the Development environment, it applies pending Entity Framework Core migrations and seeds the built-in roles and a local administrator account.

The default admin email is:

```text
admin@bookingsystem.local
```

The source of the admin password depends on how the application is started:

| Run mode | Admin password source | How to view it |
|---|---|---|
| Docker Compose | `DEVELOPMENT_ADMIN_PASSWORD` in the repository-root `.env` file | Open `.env` locally |
| Without Docker | `DevelopmentAdmin:Password` in .NET User Secrets | Run `dotnet user-secrets list` inside the `BookingSystem.Api` project directory |

The `.env` file and .NET User Secrets are separate configuration stores. A value set in one is not automatically available in the other.

## Option 1: Run with Docker Compose

Docker Compose starts both the API and SQL Server. You do not need a local SQL Server installation, Visual Studio, or the .NET SDK for this option.

### Prerequisites

- Git
- Docker with Docker Compose support

Verify the installation:

```bash
docker --version
docker compose version
```

### 1. Clone the repository

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api.git
cd BookingSystem.Api
```

### 2. Create and configure `.env`

Create a local `.env` file from `.env.example`.

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Linux, macOS, or WSL:

```bash
cp .env.example .env
```

Replace every example value in `.env`:

```env
MSSQL_SA_PASSWORD=ReplaceWithAStrongLocalPassword1!
JWT_KEY=ReplaceWithALongRandomJwtSigningKeyForLocalDevelopment123!
DEVELOPMENT_ADMIN_PASSWORD=ReplaceWithADevelopmentAdminPassword1!
```

The values have different purposes:

- `MSSQL_SA_PASSWORD` authenticates the API with the Dockerized SQL Server.
- `JWT_KEY` signs and validates JWT access tokens.
- `DEVELOPMENT_ADMIN_PASSWORD` is the password used to log in as `admin@bookingsystem.local`.

The `.env` file is excluded from Git and must never be committed.

### 3. Start the application

```bash
docker compose up --build
```

Docker Compose builds the API, starts SQL Server, waits for the database health check, applies migrations, and seeds the Development data.

Open Swagger at:

```text
http://localhost:8080/swagger
```

Log in through `POST /api/auth/login` with:

```json
{
  "email": "admin@bookingsystem.local",
  "password": "the value of DEVELOPMENT_ADMIN_PASSWORD in .env"
}
```

### 4. Stop the application

Press `Ctrl+C`, then run:

```bash
docker compose down
```

This removes the containers and network but preserves the SQL Server data volume.

To remove the database volume and all local data as well:

```bash
docker compose down -v
```

> **Warning:** `docker compose down -v` permanently deletes the local Docker database data.

### SQL Server password and persistent volumes

`MSSQL_SA_PASSWORD` is applied when the SQL Server volume is initialized. Changing this value in `.env` does not update the password of an existing database volume.

If the password must be changed after initialization, recreate the local volume:

```bash
docker compose down -v
docker compose up --build
```

### Build only the API image

From the repository root:

```bash
docker build -f ./BookingSystem.Api/Dockerfile -t bookingsystem-api .
```

The complete application still requires access to SQL Server, so Docker Compose is recommended for normal local use.

## Option 2: Run without Docker

Use this option to run the application directly with the .NET SDK and SQL Server or SQL Server LocalDB.

### Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server LocalDB
- Git, unless the repository was downloaded as a ZIP file

### 1. Clone the repository

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api.git
cd BookingSystem.Api
```

### 2. Configure .NET User Secrets

Move into the API project directory:

```bash
cd BookingSystem.Api
```

Configure a JWT signing key and the Development admin password:

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_RANDOM_JWT_SIGNING_KEY"
dotnet user-secrets set "DevelopmentAdmin:Password" "YOUR_DEVELOPMENT_ADMIN_PASSWORD"
```

Optionally override the default admin email:

```bash
dotnet user-secrets set "DevelopmentAdmin:Email" "YOUR_ADMIN_EMAIL"
```

Display the configured values at any time by running the following command from the same `BookingSystem.Api` project directory:

```bash
dotnet user-secrets list
```

The login password is the value shown next to:

```text
DevelopmentAdmin:Password
```

Do not look for this password in `.env` when running without Docker. The `.env` file is used by Docker Compose, whereas direct .NET execution reads the Development password from .NET User Secrets.

.NET User Secrets are stored outside the repository on the local machine and are not committed to Git.

### 3. Configure SQL Server

The default connection string uses SQL Server LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=BookingDb;Trusted_Connection=True;TrustServerCertificate=True;
```

No change is required when SQL Server LocalDB is available. To use another SQL Server instance, store its connection string in .NET User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_SQL_SERVER_CONNECTION_STRING"
```

### 4. Restore, build, and run

Return to the repository root:

```bash
cd ..
```

Then run:

```bash
dotnet restore
dotnet build
dotnet run --project BookingSystem.Api
```

Swagger is available at the URL shown in the terminal, typically:

```text
https://localhost:7223/swagger
```

### 5. Log in

From the API project directory, display the locally configured password if necessary:

```bash
dotnet user-secrets list
```

Use the value of `DevelopmentAdmin:Password` with `POST /api/auth/login`:

```json
{
  "email": "admin@bookingsystem.local",
  "password": "the value of DevelopmentAdmin:Password in .NET User Secrets"
}
```

If `DevelopmentAdmin:Email` was overridden, use that email instead.

## Booking Rules

The application enforces the following rules:

- A booking must start in the future.
- The end time must be later than the start time.
- The selected user and resource must exist.
- The selected resource must be active.
- Active bookings for the same resource cannot overlap.
- Cancelled and completed bookings cannot be modified.
- A background service automatically marks expired active bookings as completed.

## Pagination and Filtering

`GET /api/bookings` supports:

- pagination through `page` and `pageSize`
- filtering by resource ID
- filtering by start and end date
- filtering by booking status

`pageSize` accepts values from 1 through 100.

## Error Handling

Centralized middleware logs unexpected exceptions and returns a consistent `500 Internal Server Error` response without exposing internal exception details.

## Security

- Passwords are stored as hashes using ASP.NET Core's password hasher.
- JWT signing keys and Development admin passwords are kept outside committed configuration files.
- `.env` is excluded from Git and is used only by Docker Compose.
- .NET User Secrets are used for sensitive local configuration when the API runs directly with the .NET SDK.
- Authorization restricts protected operations by user identity and role.
- Database relationships use restrictive delete behavior to protect referenced data.

## Tests

The solution contains xUnit tests for service-layer behavior, authorization rules, authentication, booking validation, and database constraints.

Run all tests from the repository root:

```bash
dotnet test
```

## Continuous Integration

The GitHub Actions workflow in `.github/workflows/ci.yml` automatically restores dependencies, builds the solution in Release configuration, and runs the test suite for pushes and pull requests.

## Purpose

This portfolio project demonstrates practical backend development with ASP.NET Core, including:

- REST API design
- layered application structure
- Entity Framework Core and relational database integration
- authentication and authorization
- validation and error handling
- automated testing
- background processing
- continuous integration
- containerization
- reproducible local development
