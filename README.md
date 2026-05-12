# BookingSystem API
> This project demonstrates a production-like ASP.NET Core API with authentication, authorization, background jobs, and business rules.

A RESTful API built with ASP.NET Core for managing bookings, resources, users, and roles.

## Overview

The project demonstrates modern backend practices including layered architecture, JWT authentication, role-based authorization, background processing, and clean API design.


### Example Booking Flow
```
1. User creates a booking  
2. System validates time and availability  
3. Booking is stored with status `Active`  
4. Background service automatically updates expired bookings to `Completed`  
5. User or admin can cancel or complete bookings manually  
```
## Key Features

- Booking management with conflict detection (no overlapping bookings)
- Clean layered architecture (Controllers, Services, DTOs)
- Background service for automatic booking status updates
- Validation rules to ensure data integrity
- Pagination and filtering support


## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core **10.0.7**
- Swagger / OpenAPI
- C#

---

## Important (Versioning)

This project requires:

- .NET SDK (compatible with the project)
- Entity Framework Core **10.0.7**

All EF Core packages must use the **same version**,  
otherwise the project may fail at build or runtime.
```PowerShell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.7
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.7
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.7
```
---
## Authentication (JWT)

### Required packages

Before running the project, ensure the following NuGet packages are installed:

```PowerShell
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

### Configuration

Development JWT settings are included by default for local testing. The JWT key in `appsettings.json` is not intended for production use.

### Security note

The JWT key in `appsettings.json` is for development purposes only. In a production environment, secrets should be stored securely using:
- Environment variables
- Secret managers (e.g. Azure Key Vault)

### How authentication works

1. User logs in via:

```
POST /api/auth/login
```
2. API returns a JWT token
3. Token must be included in requests:
```
Authorization: Bearer {token}
```

### Swagger (Testing authentication)

Swagger UI supports JWT authentication:
1. Click Authorize
2. Enter:
```
Bearer YOUR_TOKEN_HERE
```
3. Call secured endpoints

### Notes

 - Authentication is handled using JWT Bearer tokens
 - Authorization middleware is enabled in `Program.cs`
 - Swagger is configured to support authenticated requests
 - Tokens are validated using issuer, audience, and signing key

### Future improvements

- Refresh tokens for improved session management

## ER Diagram

![ER Diagram](docs/er-diagram.png)

---

## Data Model

- A **User** has one **Role**
- A **Booking** belongs to one **User**
- A **Booking** uses one **Resource**

---

## API Endpoints

### Bookings
- `GET /api/bookings`
- `GET /api/bookings?page=1&pageSize=10`
- `GET /api/bookings?resourceId=1`
- `GET /api/bookings?userId=1`
- `GET /api/bookings?fromDate=2026-05-01&toDate=2026-05-31`

`BookingStatus` API endpoint url supports string representations of enum values. 

- `GET /api/bookings?status=Active`
- `GET /api/bookings?status=Cancelled`
- `GET /api/bookings?status=Completed`

Using string values (e.g. `Completed`) is recommended for better readability and maintainability. Note that the Swagger UI only accepts integer for `status`.  

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
- `POST /api/roles`
- `GET /api/roles/{id}`
- `PUT /api/roles/{id}`
- `DELETE /api/roles/{id}`

### Resources
- `GET /api/resources`
- `POST /api/resources`
- `GET /api/resources/{id}`
- `PUT /api/resources/{id}`
- `DELETE /api/resources/{id}`

---
## Booking Business Rules & Validation

The API enforces a set of validation rules to ensure data integrity and prevent invalid or conflicting bookings.

### Business Rules

The following rules apply:

1. **Valid Time Range**
   - `StartTime` must be earlier than `EndTime`
   - Returns `400 Bad Request` if invalid

2. **Valid StartTime Range**
   - `StartTime` must be in the future.
   - Returns `400 Bad Request` if invalid

3. **Role Must Exist**
   - The provided `RoleId` must exist in the system
   - Returns `404 Not Found` if role does not exist

4. **User Must Exist**
   - The provided `UserId` must exist in the system
   - Returns `404 Not Found` if user does not exist

5. **User `Email` Must be Unique**
   - The provided `Email` is not already in use.
   - Returns `409 Conflict` if the provided `Email` already exist in the system

6. **Resource Must Exist**
   - The provided `ResourceId` must exist in the system
   - Returns `404 Not Found` if resource does not exist

7. **Booking Must Exist**
   - The provided `BookingId` must exist in the system
   - Returns `404 Not Found` if booking does not exist

8. **The requested role to be deleted has no users**
   - The provided `RoleId` to be deleted does not have any users
   - Returns `409 Conflict` if any users are detected

9. **The requested user to be deleted has no bookings**
   - Provided `UserId` to be deleted does not have any bookings
   - Returns `409 Conflict` if booking is detected

10. **The requested resource to be deleted has no bookings**
    - The provided `ResourceId` to be deleted does not have any bookings
    - Returns `409 Conflict` if booking is detected

11. **No Overlapping Bookings**
    - A resource cannot be double-booked within overlapping time ranges
    - Returns `409 Conflict` if overlap is detected

12. **The requested resource to be booked must be Active**
    - The resource must have `IsActive = true`
    - Returns `400 Bad Request` if inactive

13. **The requested role name to be created should not already be in use**
    - A newly created role should have an unique name
    - Returns `409 Conflict` if a role name already exists in the system

14. **A booking cannot be cancelled more than once**
    - The specified `BookingId` must not already have the status "Cancelled"
    - Returns `400 Bad Request` if the booking is already cancelled

15. **Completed bookings cannot be cancelled**
    - The specified `BookingId` must not already have the status "Completed"
    - Returns `400 Bad Request` if the booking is already completed

16. **Cancelled bookings cannot be completed**
    - The specified `BookingId` must not already have the status "Cancelled"
    - Returns `400 Bad Request` if the booking is already cancelled

17. **Booking cannot be completed before EndTime has passed**
    - The specified `BookingId` can only be completed if EndTime is in the past
    - Returns `400 Bad Request` if EndTime is not in the past

18. **A booking cannot be completed more than once**
    - The specified `BookingId` must not already have the status "completed"
    - Returns `400 Bad Request` if the booking is already completed

---

**Successful Booking**

   For creating a booking (`POST /api/bookings`):
   - If the validations 1., 2., 4., 6., 11., 12. pass, the booking is created successfully
   - Returns `201 Created` with the created booking

   For Cancelling a booking (`PATCH /api/bookings/{id}/cancel`):
   - If the validations 7., 14. and 15. pass, the booking is cancelled successfully
   - Returns `204 No Content`

   For Completing a booking (`PATCH /api/bookings/{id}/complete`):
   - If the validations 7., 16., 17. and 18. pass, the booking is completed successfully
   - Returns `204 No Content`

   For updating an existing booking (`PUT /api/bookings`):
   - If the validations 1., 2., 4., 6., 11., 12. pass, the existing booking is updated successfully
   - Returns `204 No Content`
   - Note: Completed and cancelled bookings are immutable and cannot be modified.

   For deleting an existing booking (`DELETE /api/bookings`):
   - If validation 7. pass, the existing booking is deleted successfully
   - Returns `204 No Content`

---

**Successful Resource**

   For creating a resource (`POST /api/resources`):
   - No validations need to be passed, and the resource will be created successfully
   - Returns `201 Created` with the created resource

   For updating an existing resource (`PUT /api/resources`):
   - If validation 6. pass, the existing resource is updated successfully
   - Returns `204 No Content`

   For deleting an existing resource (`DELETE /api/resources`):
   - If validation 6. and 10. pass, the existing resource is deleted successfully
   - Returns `204 No Content`

---

**Successful User**

   For creating a user (`POST /api/users`):
   - If validation 3. and 5. pass, the user is created successfully
   - Returns `201 Created` with the created user

   For updating an existing user (`PUT /api/users`):
   - If validation 3., 4. and 5. pass, the existing user is updated successfully
   - Returns `204 No Content`

   For deleting an existing user (`DELETE /api/users`):
   - If validation 4. and 9. pass, the existing user is deleted successfully
   - Returns `204 No Content`

---

**Successful Role**

   For creating a role (`POST /api/roles`):
   - If validation 13. pass, the role is created successfully
   - Returns `201 Created` with the created role

   For updating an existing role (`PUT /api/roles`):
   - If validation 3. and 13. pass, the existing role is updated successfully
   - Returns `204 No Content`

   For deleting an existing role (`DELETE /api/roles`):
   - If validation 3. and 8. pass, the existing role is deleted successfully
   - Returns `204 No Content`

---

### Example Error Response

```json
{
  "error": "Resource is already booked in this time range."
}
```

---

This project follows a clean, layered architecture with clear separation of concerns.

### Core Layers

- **Controllers**  
  Handle HTTP requests and responses.  
  Responsible for routing, model binding, and returning appropriate HTTP status codes.

- **Services**  
  Contain all business logic and validation rules.  
  Keep controllers thin and logic reusable and testable.

- **DTOs (Data Transfer Objects)**  
  Define API input/output models.  
  Prevent direct exposure of domain entities.

- **Data (DbContext)**  
  Handles database access using Entity Framework Core.

---

### Supporting Layers

- **Enums**  
  Strongly-typed domain values (e.g. `BookingStatus`).  
  Improve readability and prevent invalid states.

- **Middleware**  
  Handles cross-cutting concerns such as global error handling.  
  Ensures consistent API responses.

- **BackgroundServices**  
  Executes background processes independently of HTTP requests.  
  Example: Automatically marks expired bookings as `Completed`.

- **Seed (DbSeeder)**  
  Seeds initial data such as roles and a development admin user.  
  Ensures the system is usable immediately after setup.

---

### Design Principles

- Separation of concerns  
- Thin controllers  
- Centralized business logic  
- Secure-by-default API design  
- Scalable and maintainable structure  

---

## Time Handling

All timestamps are stored and processed in **UTC** (`DateTime.UtcNow`).

- Avoids issues with time zones and daylight saving time  
- Ensures consistent behavior across environments  
- Clients are responsible for converting to local time  

## API Documentation

Swagger UI is available when running the application.

Typically:
```bash
https://localhost:7223/swagger
```
Note: The port may vary depending on your local setup.

---

## Authentication & Authorization

This API uses **JWT (JSON Web Token)** authentication with **role-based authorization (RBAC)**.

---

### Authentication

Authenticate via:

```http
POST /api/auth/login
```
---

The API returns a JWT token which must be included in subsequent requests:

```http
Authorization: Bearer {token}
```

## Authorization (RBAC)

Access is controlled using roles:

| Role  | Permissions                          |
|-------|--------------------------------------|
| Admin | Full access to all resources         |
| User  | Limited access (own bookings only)   |

---

### Examples

- **Bookings**
  - Users can only access their own bookings
  - Admins can access all bookings

- **Resources**
  - All authenticated users can read
  - Only Admins can create/update/delete

- **Users & Roles**
  - Admin only

> Access control is enforced both at controller level and within the service layer.

### What happens on startup

- Database migrations are applied
- Roles are created if missing
- Admin user is created if missing

---


## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api
cd BookingSystem.Api/
```

### 2. Apply database migrations

```bash
dotnet restore
dotnet ef database update
```

### 3. Run the application

```bash
dotnet run
```

## API Documentation

Swagger UI is available when running the application:

```bach
https://localhost:7223/swagger
```

## Swagger Authentication

1. Login via `/api/auth/login`
2. Copy the returned JWT token
3. Click **Authorize** in Swagger UI
4. Enter:

```bash
Bearer YOUR_TOKEN_HERE
```

## Features

- CRUD operations for bookings, users, roles, and resources  
- Pagination and filtering  
- Booking status lifecycle (`Active`, `Cancelled`, `Completed`)  
- Background job for automatic status updates  
- JWT authentication  
- Role-based authorization (RBAC)  
- Ownership validation  
- Global error handling  
- Clean API routes  
- Seed data for easy setup  

## Default Admin Login (Development)

To quickly access the system, a default admin user is created on startup:
```
Email: admin@bookingsystem.local 
Password: Admin123!
```

> This account is for development purposes only.
> Change credentials in production environments.

## Security Notes

- JWT key in `appsettings.json` is for development only  
- Use environment variables or secret managers in production  
- Passwords are hashed using ASP.NET Core Identity utilities  