# 📅 BookingSystem API

A RESTful API built with ASP.NET Core for managing bookings, users, roles, and resources.

This project focuses on clean structure, the DTO pattern, and relational database design.

---

## 🚀 Features

- Partial CRUD for:
  - Bookings
  - Users
  - Roles
  - Resources
- Foreign key relationships
- DTO pattern implemented
- Swagger UI for testing

---

## 🧱 Tech Stack

- ASP.NET Core Web API
- Entity Framework Core **10.0.0**
- SQL Server
- Swagger / OpenAPI
- C#

---

## ⚠️ Important (Versioning)

This project requires:

- .NET SDK (compatible with the project)
- Entity Framework Core **10.0.0**

👉 All EF Core packages must use the **same version**,  
otherwise the project may fail at build or runtime.

---

## 📊 ER Diagram

![ER Diagram](docs/er-diagram.png)

---

## 🔗 Data Model

- A **User** has one **Role**
- A **Booking** belongs to one **User**
- A **Booking** uses one **Resource**

---

## 📦 API Endpoints

### Bookings
- `GET /api/bookings`
- `POST /api/bookings`

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

## ▶️ Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/YOUR-USERNAME/BookingSystem.Api.git
cd BookingSystem.Api