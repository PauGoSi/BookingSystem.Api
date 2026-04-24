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
- SQL Server
- Swagger / OpenAPI
- C#

---

## Important (Versioning)

This project requires:

- .NET SDK (compatible with the project)
- Entity Framework Core **10.0.0**

All EF Core packages must use the **same version**,  
otherwise the project may fail at build or runtime.

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

### Users
- `GET /api/users`
- `POST /api/users`

### Roles
- `GET /api/roles`
- `POST /api/roles`

### Resources
- `GET /api/resources`
- `POST /api/resources`

---
## Booking Business Rules & Validation

The Booking API enforces a set of validation rules to ensure data integrity and prevent invalid or conflicting bookings.

### Business Rules

When creating a booking (`POST /api/bookings`) or updating a booking (`PUT /api/bookings`), the following rules apply:

1. **Valid Time Range**
   - `StartTime` must be earlier than `EndTime`
   - Returns `400 Bad Request` if invalid

2. **User Must Exist**
   - The provided `UserId` must exist in the system
   - Returns `404 Not Found` if user does not exist

3. **Resource Must Exist**
   - The provided `ResourceId` must exist
   - Returns `404 Not Found` if resource does not exist

4. **Resource Must Be Active**
   - The resource must have `IsActive = true`
   - Returns `400 Bad Request` if inactive

5. **No Overlapping Bookings**
   - A resource cannot be double-booked within overlapping time ranges
   - Returns `409 Conflict` if overlap is detected

6. **Successful Booking**

   For creating:
   - If all validations (1.-5.) pass, the booking is created successfully
   - Returns `201 Created` with the created booking

   For updating:
   - If all validations (1.-5.) pass, the booking is updated successfully
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