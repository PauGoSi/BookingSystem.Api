# BookingSystem API

A RESTful API built with ASP.NET Core for managing bookings, users, roles, and resources.

This project focuses on clean structure, the DTO pattern, and relational database design.

---

## Features

- Partial CRUD for:
  - Bookings
  - Users
  - Roles
  - Resources
- Foreign key relationships
- DTO pattern implemented
- Swagger UI for testing

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core **10.0.0**
- Swagger / OpenAPI
- C#

---

## Important (Versioning)

This project requires:

- .NET SDK (compatible with the project)
- Entity Framework Core **10.0.0**

All EF Core packages must use the **same version**,  
otherwise the project may fail at build or runtime.
```PowerShell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
```
---

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
- `POST /api/bookings`
- `PUT /api/bookings`
- `DELETE /api/bookings`

### Users
- `GET /api/users`
- `POST /api/users`
- `PUT /api/users`
- `DELETE /api/users`

### Roles
- `GET /api/roles`
- `POST /api/roles`

### Resources
- `GET /api/resources`
- `POST /api/resources`
- `PUT /api/resources`
- `DELETE /api/resources`

---
## Booking Business Rules & Validation

The API enforces a set of validation rules to ensure data integrity and prevent invalid or conflicting bookings.

### Business Rules

The following rules apply:

1. **Valid Time Range**
   - `StartTime` must be earlier than `EndTime`
   - Returns `400 Bad Request` if invalid

2. **Role Must Exist**
   - The provided `RoleId` must exist in the system
   - Returns `404 Not Found` if role does not exist

3. **User Must Exist**
   - The provided `UserId` must exist in the system
   - Returns `404 Not Found` if user does not exist

4. **Resource Must Exist**
   - The provided `ResourceId` must exist in the system
   - Returns `404 Not Found` if resource does not exist

5. **Booking Must Exist**
   - The provided `BookingId` must exist in the system
   - Returns `404 Not Found` if booking does not exist

6. **The requested user to be deleted has no bookings**
   - Provided `UserId` to be deleted does not have any bookings
   - Returns `409 Conflict` if booking is detected

7. **The requested resource to be deleted has no bookings**
   - The provided `ResourceId` does not have any bookings
   - Returns `409 Conflict` if booking is detected

8. **No Overlapping Bookings**
   - A resource cannot be double-booked within overlapping time ranges
   - Returns `409 Conflict` if overlap is detected

9. **The requested resource to be booked must be Active**
   - The resource must have `IsActive = true`
   - Returns `400 Bad Request` if inactive



**Successful Booking**

   For creating a booking (`POST /api/bookings`):
   - If the validations 1., 3., 4., 8., 9. pass, the booking is created successfully
   - Returns `201 Created` with the created booking

   For updating an existing booking (`PUT /api/bookings`):
   - If the validations 1., 3., 4., 8., 9. pass, the existing booking is updated successfully
   - Returns `204 No Content`

   For deleting an existing booking (`DELETE /api/bookings`):
   - If validation 5. pass, the existing booking is deleted successfully
   - Returns `204 No Content`

**Successful Resource**

   For creating a resource (`POST /api/resources`):
   - No validations need to be passed, and the resource will be created successfully
   - Returns `201 Created` with the created resource

   For updating an existing resource (`PUT /api/resources`):
   - If validation 4. pass, the existing resource is updated successfully
   - Returns `204 No Content`

   For deleting an existing resource (`DELETE /api/resources`):
   - If validation 4. and 7. pass, the existing resource is deleted successfully
   - Returns `204 No Content`

**Successful User**

   For creating a user (`POST /api/users`):
   - If validation 2. pass, the existing user is created successfully
   - Returns `201 Created` with the created user

   For updating an existing user (`PUT /api/users`):
   - If validation 3. pass, the existing resource is updated successfully
   - Returns `204 No Content`

   For deleting an existing user (`DELETE /api/users`):
   - If validation 3. and 6. pass, the existing user is deleted successfully
   - Returns `204 No Content`

---
### Example Error Response

```json
{
  "error": "Resource is already booked in this time range."
}
```


### This project follows a layered architecture:

 - Controllers: Handle HTTP requests
 - Services: Contain business logic and validation rules
 - DTOs: Define API input/output models
 - Data (DbContext): Handles database access via Entity Framework Core

Business logic is isolated in the service layer to ensure:
 - Clean controllers
 - Reusable logic
 - Easier testing and maintenance
 
## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/PauGoSi/BookingSystem.Api
```

## API Documentation
Swagger UI is available when running the application.

Typically:
https://localhost:7223/swagger

Note: The port may vary depending on your local setup.