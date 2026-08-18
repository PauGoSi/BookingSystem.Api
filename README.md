# BookingSystem.Api

A RESTful booking system built with ASP.NET Core Web API.

The project demonstrates a production-oriented backend architecture with authentication, authorization, validation, Entity Framework Core, automated tests, CI, and Docker-based local development.

## Features

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT authentication
- Role-based authorization
- CRUD operations for users, roles, resources, and bookings
- Booking validation and conflict handling
- Pagination and filtering
- Global exception handling
- Automatic booking completion using a background service
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
- Docker
- Docker Compose
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

## Main Entities

The application contains four main domain entities:

- **User** – represents a user of the booking system.
- **Role** – defines the user's role and authorization level.
- **Resource** – represents a resource that can be booked.
- **Booking** – connects a user with a resource for a specified time period.

## ER Diagram

![ER Diagram](docs/er-diagram.png)

## Data Model

- A **User** has one **Role**
- A **Booking** belongs to one **User**
- A **Booking** uses one **Resource**

## API Endpoints

### Bookings

- `GET /api/bookings`
- `GET /api/bookings?page=1&pageSize=10`
- `GET /api/bookings?resourceId=1`
- `GET /api/bookings?userId=1`
- `GET /api/bookings?fromDate=2026-05-01&toDate=2026-05-31`

`BookingStatus` API endpoint URL supports string representations of enum values.

- `GET /api/bookings?status=Active`
- `GET /api/bookings?status=Cancelled`
- `GET /api/bookings?status=Completed`

Using string values (e.g. `Completed`) is recommended for better readability and maintainability.

- `POST /api/bookings`
- `PATCH /api/bookings/{id}/cancel`
- `PATCH /api/bookings/{id}/complete`
- `GET /api/bookings/{id}`
- `PUT /api/bookings/{id}`
- `DELETE /api/bookings/{id}`

### Users

- `GET /api/users`
- `POST /api/users`
- `GET /api/users/{id}`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

### Roles

- `GET /api/roles`
- `GET /api/roles/{id}`

`Admin` and `User` are built-in system roles created during Development seeding. Role creation, modification, and deletion are intentionally not supported.

User creation and updates use the `role` field with the supported values `"Admin"` and `"User"` instead of exposing database role IDs. User responses likewise expose the role name rather than the internal `RoleId`.

Self-registered users are always assigned the `User` role.

### Resources

- `GET /api/resources`
- `POST /api/resources`
- `GET /api/resources/{id}`
- `PUT /api/resources/{id}`
- `DELETE /api/resources/{id}`

## Authentication and Authorization

The API uses JWT Bearer authentication.

Users authenticate through:

```http
POST /api/auth/login
```

A successful login returns a JWT token.

The token can then be supplied through Swagger using the **Authorize** button.

Protected endpoints use role-based authorization where appropriate.

## Running the Project with Docker

Docker is the recommended way to run the complete application locally.

Docker Compose starts both:

1. the ASP.NET Core API
2. Microsoft SQL Server

This means that you do **not** need to install SQL Server locally.

You also do **not** need Visual Studio or the .NET SDK when running the application entirely through Docker.

### Prerequisites

You need:

- Git
- Docker with Docker Compose support

Verify Docker:

```bash
docker --version
docker compose version
```

Both commands should return version information.

> Docker can be used from PowerShell, Command Prompt, Linux, macOS, or WSL.  
> Ubuntu/WSL is not required.

### 1. Clone the repository

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api.git
cd BookingSystem.Api
```

### 2. Create the local environment file

The repository contains an `.env.example` file with the required environment variable names.

Create your own `.env` file from it.

#### Windows PowerShell

```powershell
Copy-Item .env.example .env
```

#### Linux / macOS / WSL

```bash
cp .env.example .env
```

The resulting `.env` file should contain:

```env
MSSQL_SA_PASSWORD=ReplaceWithAStrongLocalPassword1!
JWT_KEY=ReplaceWithALongRandomJwtSigningKeyForLocalDevelopment123!
DEVELOPMENT_ADMIN_PASSWORD=ReplaceWithADevelopmentAdminPassword1!
```

Replace the example values with your own local development values.

The `.env` file is excluded from Git and **must not be committed**.

### Important: SQL Server password and Docker volumes

`MSSQL_SA_PASSWORD` is used when the SQL Server Docker volume is initialized for the first time.

Once SQL Server has been initialized, its data is stored in a persistent Docker volume.

Therefore, changing only:

```env
MSSQL_SA_PASSWORD=...
```

in `.env` after the database has already been created will **not** change the password used by the existing SQL Server instance.

This may result in an error similar to:

```text
Login failed for user 'sa'
```

If you need to change `MSSQL_SA_PASSWORD` after SQL Server has already been initialized, recreate the local database volume:

```bash
docker compose down -v
docker compose up --build
```

> **Warning:** `docker compose down -v` deletes the local Docker database volume and all data stored in it. This is intended only for resetting the local development environment.

The development admin password is separate from the SQL Server `sa` password.

### 3. Start the application

Run:

```bash
docker compose up --build
```

Docker Compose will:

1. build the ASP.NET Core API image
2. start SQL Server
3. start the API
4. connect the API to SQL Server
5. initialize the application database
6. seed the required development data

The first startup may take longer because Docker may need to download the required base images.

When the API is ready, the logs should contain something similar to:

```text
Now listening on: http://[::]:8080
Application started.
```

### 4. Open Swagger

Open:

```text
http://localhost:8080/swagger
```

Swagger provides an interactive interface for exploring and testing the API.

## Development Admin Login

In the Development environment, the application seeds a local administrator account:

```text
Email: admin@bookingsystem.local
```

The password is the value configured in:

```env
DEVELOPMENT_ADMIN_PASSWORD
```

For example, if your `.env` contains:

```env
DEVELOPMENT_ADMIN_PASSWORD=MyLocalAdminPassword123!
```

use:

```json
{
  "email": "admin@bookingsystem.local",
  "password": "MyLocalAdminPassword123!"
}
```

with:

```http
POST /api/auth/login
```

A successful request returns a JWT token.

### Using the JWT token in Swagger

1. Call `POST /api/auth/login`.
2. Copy the returned JWT token.
3. Click **Authorize** at the top of Swagger.
4. Enter the token as required by the Swagger authorization dialog.
5. Click **Authorize**.
6. You can now call protected endpoints.

## Stopping the Application

Press:

```text
Ctrl+C
```

in the terminal running Docker Compose.

Then run:

```bash
docker compose down
```

This removes the containers and Docker network but preserves the SQL Server data volume.

The next:

```bash
docker compose up
```

will therefore reuse the existing local database.

To remove the database as well:

```bash
docker compose down -v
```

Again, this permanently deletes the local Docker database data.

## Building Only the API Docker Image

The API image can also be built independently of Docker Compose.

From the repository root:

```bash
docker build -f ./BookingSystem.Api/Dockerfile -t bookingsystem-api .
```

Verify the image:

```bash
docker images
```

You should see an image named:

```text
bookingsystem-api
```

Docker Compose is recommended when running the complete application because the API depends on SQL Server.

## Running Without Docker

The project can also be run directly with the .NET SDK.

For this approach you need:

- .NET 10 SDK
- access to a SQL Server instance
- appropriate local configuration/secrets

From the API project directory:

```bash
cd BookingSystem.Api
dotnet restore
dotnet build
dotnet run
```

For most reviewers, the Docker Compose setup is the simplest way to run the complete application.

## Tests

The solution contains automated tests using xUnit.

Run all tests from the repository root:

```bash
dotnet test
```

The tests cover selected service-layer behavior and validation rules.

## Continuous Integration

The repository contains a GitHub Actions workflow:

```text
.github/workflows/ci.yml
```

The CI pipeline automatically restores dependencies, builds the solution, and runs the automated tests.

This helps ensure that committed changes continue to compile and pass the test suite.

## Booking Rules

The booking domain includes validation such as:

- a booking cannot be created in the past
- the end time must be later than the start time
- bookings are associated with an existing user and resource
- booking status is handled by the application
- completed bookings can be updated automatically by a background service

## Pagination and Filtering

Booking queries support pagination and filtering.

Examples include filtering by:

- Resource ID
- User ID
- From date
- To date

Pagination is controlled through page and page-size parameters.

## Error Handling

The API contains centralized error handling through middleware.

This keeps exception handling out of individual controllers and provides a consistent approach to API errors.

## Security

Sensitive configuration values are not intended to be committed to source control.

Local Docker secrets are stored in:

```text
.env
```

and the file is excluded through `.gitignore`.

Only:

```text
.env.example
```

is committed, containing example/placeholder values.

JWT signing keys, SQL Server passwords, and development administrator passwords should always be replaced with appropriate local values.

## Quick Start

For a reviewer who already has Git and Docker installed:

### PowerShell

```powershell
git clone https://github.com/PauGoSi/BookingSystem.Api.git
cd BookingSystem.Api
Copy-Item .env.example .env
# Edit .env and replace the example secrets
docker compose up --build
```

### Linux / macOS / WSL

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api.git
cd BookingSystem.Api
cp .env.example .env
# Edit .env and replace the example secrets
docker compose up --build
```

Then open:

```text
http://localhost:8080/swagger
```

Login with:

```text
admin@bookingsystem.local
```

and the password configured as `DEVELOPMENT_ADMIN_PASSWORD` in `.env`.

## Purpose

This project was created as a portfolio project to demonstrate practical backend development with ASP.NET Core, including:

- REST API design
- layered application structure
- Entity Framework Core
- relational database integration
- authentication and authorization
- validation and error handling
- automated testing
- background processing
- CI
- containerization
- reproducible local development with Docker Compose