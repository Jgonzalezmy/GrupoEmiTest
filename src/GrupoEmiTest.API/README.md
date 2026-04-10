# GrupoEmiTest — .NET Backend Developer Technical Test

A fully-featured ASP.NET Core 10 Web API built as a solution to the EMI Group backend technical test. The project demonstrates **Clean Architecture**, **SOLID principles**, the **Repository Pattern**, the **Unit of Work Pattern**, the **Result Pattern**, JWT-based authentication, role-based authorization, and keyset pagination — all running on SQL Server via Docker.

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [SOLID Principles Applied](#2-solid-principles-applied)
3. [Design Patterns](#3-design-patterns)
4. [Project Structure](#4-project-structure)
5. [Database Schema](#5-database-schema)
6. [Prerequisites](#6-prerequisites)
7. [Running with Docker (Recommended)](#7-running-with-docker-recommended)
8. [Running Locally without Docker](#8-running-locally-without-docker)
9. [Default Seed Data & Credentials](#9-default-seed-data--credentials)
10. [API Endpoints & Usage Guide](#10-api-endpoints--usage-guide)
11. [Authentication & Authorization](#11-authentication--authorization)
12. [Keyset Pagination](#12-keyset-pagination)
13. [Section-by-Section Test Answers](#13-section-by-section-test-answers)

---

## 1. Architecture Overview

The solution follows **Clean Architecture** (also known as Ports and Adapters), organized into four independent layers. Dependency always flows inward — outer layers depend on inner ones, never the reverse.

```
┌─────────────────────────────────────────────────────────┐
│                  GrupoEmiTest.API                        │
│  Controllers · Middlewares · Validators · Policies       │
│  (Presentation Layer — depends on Application)           │
├─────────────────────────────────────────────────────────┤
│               GrupoEmiTest.Application                   │
│  Services · DTOs · Interfaces · Extensions               │
│  (Business Logic Layer — depends on Domain)              │
├─────────────────────────────────────────────────────────┤
│                 GrupoEmiTest.Domain                      │
│  Entities · Enums · Errors · Common (Result/Error)       │
│  (Core Domain Layer — no external dependencies)          │
├─────────────────────────────────────────────────────────┤
│              GrupoEmiTest.Infrastructure                 │
│  EF Core · Repositories · UnitOfWork · Security · Seed  │
│  (Data Access Layer — depends on Application + Domain)   │
└─────────────────────────────────────────────────────────┘
```

**Why Clean Architecture?**
- The **Domain** layer has zero external NuGet dependencies. All business rules live there, isolated and easily testable.
- Swapping the database engine, ORM, or authentication provider requires changes only in Infrastructure, with zero impact on business logic.
- Each layer is compiled as a separate project, enforcing compile-time dependency direction.

---

## 2. SOLID Principles Applied

### S — Single Responsibility Principle
Every class has exactly one reason to change.

- `Employee` knows only how to represent an employee and calculate its bonus.
- `EmployeeService` orchestrates employee business workflows (create, update, delete) but delegates persistence to `IUnitOfWork`.
- `TokenService` is solely responsible for generating JWT tokens.
- `PasswordHasher` is solely responsible for hashing and verifying passwords.
- `RequestLoggingMiddleware` has one job: log request and response details.
- `DataSeeder` is only responsible for seeding initial data.
- Each `*Configuration` class (e.g., `EmployeeConfiguration`) configures one and only one EF Core entity.

### O — Open/Closed Principle
Classes are open for extension, closed for modification.

- `PositionType` is an enum with clearly defined managerial levels. Adding a new position type (e.g., `CTO`) requires adding one enum value, not modifying `CalculateYearlyBonus`. The method delegates the managerial check to `PositionTypeExtensions.IsManagerPosition()`.
- The generic `Repository<T>` is closed to modification. Specialized repositories (`EmployeeRepository`, `PositionHistoryRepository`) extend it by inheriting, adding queries without touching the base.
- The `Result<T>` type is closed to modification. New error categories are added via the `Error` factory methods (`Error.NotFound(...)`, `Error.Conflict(...)`, etc.) without modifying the Result class.
- `ResultExtensions.ToProblemResult` maps `ErrorType` values to HTTP status codes. New error types extend the mapping without changing caller code.

### L — Liskov Substitution Principle
Derived types can replace their base types without breaking behavior.

- `EmployeeRepository` extends `Repository<Employee>`. Any code that depends on `IRepository<Employee>` (the interface) works identically whether the runtime instance is `Repository<Employee>` or `EmployeeRepository`.
- `Result<TValue>` extends `Result`. Anywhere a `Result` is expected, a `Result<TValue>` is substitutable.

### I — Interface Segregation Principle
Clients depend only on interfaces they actually use.

- `IRepository<T>` provides generic CRUD and query operations.
- `IEmployeeRepository` extends it only with employee-specific queries (`GetByIdWithDetailsAsync`, `GetAllWithDetailsAsync`, `GetByDepartmentWithProjectsAsync`).
- `IPositionHistoryRepository` extends it only with history-specific queries.
- Application services depend on `IUnitOfWork`, not on the concrete `UnitOfWork` or `DbContext`.
- `IPasswordHasher` and `ITokenService` are small, focused interfaces — the auth service only needs to know about hashing and token generation, not EF Core internals.

### D — Dependency Inversion Principle
High-level modules depend on abstractions, not on concrete implementations.

- `AuthService` depends on `IUnitOfWork`, `IPasswordHasher`, and `ITokenService` — all interfaces defined in the Application or Domain layer. The implementations live in Infrastructure, which is the lowest-level layer.
- `EmployeeService` depends on `IUnitOfWork`, never on `GrupoEmiTestDBContext` directly.
- All dependencies are resolved at startup via ASP.NET Core's built-in DI container, registered in dedicated `DependencyInjection*.cs` extension classes per layer.

---

## 3. Design Patterns

### Repository Pattern
Abstracts all data access behind interfaces defined in the Domain layer.

- `IRepository<T>` — generic contract: `GetByIdAsync`, `FindAsync`, `AddAsync`, `Update`, `Delete`, `GetPageAsync`, etc.
- `Repository<T>` — generic EF Core implementation.
- `IEmployeeRepository` / `EmployeeRepository` — specialized extension with eager-loading queries that use `AsSplitQuery()` to prevent Cartesian explosion when joining multiple collections.
- `IPositionHistoryRepository` / `PositionHistoryRepository` — specialized queries for position history lookups.

The Application layer never imports `Microsoft.EntityFrameworkCore`. It only calls interface methods, keeping business logic decoupled from any specific ORM.

### Unit of Work Pattern
Coordinates multiple repositories under one shared `DbContext`, enabling atomic multi-entity transactions.

`IUnitOfWork` exposes:
- `Employees`, `PositionHistories`, `Users` — all sharing one `GrupoEmiTestDBContext` instance.
- `SaveChangesAsync` — flushes all tracked changes in a single round-trip.
- `AddAndSaveAsync<T>` / `UpdateAndSaveAsync<T>` / `DeleteAndSaveAsync<T>` — convenience methods for single-entity atomic operations.
- `ExecuteInTransactionAsync<T>(Func<Task<Result<T>>>)` — wraps any async operation in a database transaction with automatic commit on success and rollback on failure or exception.

The `UpdateAsync` operation in `EmployeeService` is a good example: it atomically updates the employee record and closes/creates `PositionHistory` entries in one transaction — if anything fails, the entire operation is rolled back.

### Result Pattern
Replaces exception-driven flow control with explicit, type-safe return values.

- `Error` — an immutable record with `Code`, `Description`, and `ErrorType` (NotFound, Conflict, Validation, Unauthorized, Failure, Problem).
- `Result` — base class, `IsSuccess / IsFailure / Error`.
- `Result<TValue>` — generic variant that carries a value on success. Accessing `.Value` on a failure result throws `InvalidOperationException`.
- Implicit operators allow returning an `Error` directly from a method that returns `Result<T>`, keeping service code concise.
- `ResultExtensions.ToProblemResult` maps results to RFC 7807-compliant `ProblemDetails` HTTP responses in the API layer.

Domain errors are catalogued in static classes:
- `EmployeeErrors` — `NotFound`, `NameEmpty`, `InvalidSalary`, `InvalidDepartmentId`
- `AuthErrors` — `InvalidCredentials`, `DuplicateUsername`, `DuplicateEmail`
- `PositionHistoryErrors` — `NotFound`

### Factory Method Pattern
Domain entities expose static `Create(...)` factory methods instead of public constructors, centralizing validation logic.

```csharp
// Employee.Create validates name, salary > 0, and departmentId > 0
Result<Employee> result = Employee.Create(name, position, salary, departmentId);

// PositionHistory.Create returns a ready-to-persist instance
PositionHistory history = PositionHistory.Create(employeeId, position, startDate);

// ApplicationUser.Create ensures consistent object construction
ApplicationUser user = ApplicationUser.Create(username, email, passwordHash, role);
```

---

## 4. Project Structure

```
GrupoEmiTest/
├── docker-compose.yml
├── docker-compose.override.yml
├── .env
└── src/
    ├── GrupoEmiTest.API/
    │   ├── Controllers/
    │   │   ├── AuthController.cs
    │   │   └── EmployeesController.cs
    │   ├── Extensions/
    │   │   └── ResultExtensions.cs        # Maps Result → IActionResult (ProblemDetails)
    │   ├── Middlewares/
    │   │   └── RequestLoggingMiddleware.cs
    │   ├── Policies/
    │   │   └── PolicyConstants.cs         # ReadPolicy, WritePolicy, EditPolicy, DeletePolicy
    │   ├── Validators/                    # FluentValidation validators
    │   └── DependencyInjectionAPI.cs
    │
    ├── GrupoEmiTest.Application/
    │   ├── DTOs/
    │   │   ├── Request/
    │   │   └── Response/
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── IEmployeeService.cs
    │   │   ├── IPasswordHasher.cs
    │   │   └── ITokenService.cs
    │   ├── Services/
    │   │   ├── AuthService.cs
    │   │   └── EmployeeService.cs
    │   └── DependencyInjectionApplication.cs
    │
    ├── GrupoEmiTest.Domain/
    │   ├── Common/
    │   │   ├── Error.cs                   # Immutable error descriptor
    │   │   ├── Result.cs                  # Result / Result<T>
    │   │   ├── PagedResult.cs             # Keyset pagination response
    │   │   └── PageRequest.cs             # Keyset pagination request
    │   ├── Entities/
    │   │   ├── Employee.cs
    │   │   ├── PositionHistory.cs
    │   │   ├── Department.cs
    │   │   ├── Project.cs
    │   │   ├── EmployeeProject.cs         # Join entity
    │   │   └── ApplicationUser.cs
    │   ├── Enums/
    │   │   ├── PositionType.cs
    │   │   ├── RoleType.cs
    │   │   └── ErrorType.cs
    │   ├── Errors/
    │   │   ├── EmployeeErrors.cs
    │   │   ├── AuthErrors.cs
    │   │   └── PositionHistoryErrors.cs
    │   └── Interfaces/
    │       ├── IRepository.cs
    │       ├── IEmployeeRepository.cs
    │       ├── IPositionHistoryRepository.cs
    │       └── IUnitOfWork.cs
    │
    └── GrupoEmiTest.Infrastructure/
        ├── Data/
        │   ├── DataSeeder.cs
        │   ├── Configurations/            # One IEntityTypeConfiguration<T> per entity
        │   └── Migrations/
        ├── Repositories/
        │   ├── Repository.cs
        │   ├── EmployeeRepository.cs
        │   └── PositionHistoryRepository.cs
        ├── UnitOfWork/
        │   └── UnitOfWork.cs
        ├── Security/
        │   ├── PasswordHasher.cs          # BCrypt-style hash & verify
        │   └── TokenService.cs            # JWT generation
        └── GrupoEmiTestDBContext.cs
```

---

## 5. Database Schema

```
┌──────────────────┐       ┌──────────────────────┐       ┌───────────────────┐
│   Departments    │       │      Employees        │       │     Projects      │
│──────────────────│       │──────────────────────│       │───────────────────│
│ Id (PK)          │◄──────│ Id (PK)               │       │ Id (PK)           │
│ Name             │       │ Name                  │       │ Name              │
└──────────────────┘       │ CurrentPosition (int) │       │ Description       │
                           │ Salary (decimal)      │       └───────────────────┘
                           │ DepartmentId (FK)     │               ▲
                           └──────────────────────┘               │
                                      │   |                         │
                    ┌─────────────────┘   |____________ ┌──────────────────────┐
                    │                                   │   EmployeeProjects   │
                    ▼                                   │──────────────────────│
          ┌──────────────────────┐                      │ EmployeeId (PK, FK)  │
          │    PositionHistory   │                      │ ProjectId  (PK, FK)  │
          │──────────────────────│                      └──────────────────────┘
          │ Id (PK)              │
          │ EmployeeId (FK)      │       ┌──────────────────────┐
          │ Position (int)       │       │   ApplicationUsers   │
          │ StartDate            │       │──────────────────────│
          │ EndDate (nullable)   │       │ Id (PK)              │
          └──────────────────────┘       │ Username             │
                                         │ Email                │
                                         │ PasswordHash         │
                                         │ Role (int)           │
                                         └──────────────────────┘
```

- `EmployeeProjects` is the many-to-many join table between `Employees` and `Projects`.
- `PositionHistory.EndDate` is nullable — a `null` value means the position is current.
- `ApplicationUsers` is completely separate from `Employees`; it manages API authentication only.

---

## 6. Prerequisites

| Tool | Minimum Version |
|---|---|
| Docker Desktop | 4.x |
| Docker Compose | v2 (bundled with Docker Desktop) |
| .NET SDK *(only if running locally)* | 10.0 |

> Docker is the recommended and easiest way to run the project. No database installation is required.

---

## 7. Running with Docker (Recommended)

### Step 1 — Clone the repository

```bash
git clone <https://github.com/Jgonzalezmy/GrupoEmiTest.git>
cd GrupoEmiTest
```

### Step 2 — Review the environment file

The `.env` file at the root contains the SQL Server credentials used by both containers:

```env
SA_PASSWORD=GrupoEmi2026
DB_NAME=GrupoEmiTest
```

> You may change these values, but both the `SA_PASSWORD` and the connection string in `docker-compose.override.yml` are derived from this file automatically.

### Step 3 — Build and start the containers

```bash
docker compose up --build
```

This command:
1. Builds the API image from `src/GrupoEmiTest.API/Dockerfile`.
2. Pulls the `mcr.microsoft.com/mssql/server:2022-latest` SQL Server Express image.
3. Starts the `grupoemitest-sqlserver` container (port `11433:1433`).
4. Waits for SQL Server to pass its health check before starting the API container.
5. Starts the `grupoemitest-api` container (port `8081:8080`).
6. On first startup, the API automatically applies EF Core migrations and seeds the database.

### Step 4 — Verify the containers are running

```bash
docker compose ps
```

Expected output:

```
NAME                      STATUS
grupoemitest-sqlserver    running (healthy)
grupoemitest-api          running
```

### Step 5 — Open the API documentation

Navigate to the interactive Scalar API reference:

```
http://localhost:8081/scalar/v1
```

> The OpenAPI spec is also available at `http://localhost:8081/openapi/v1.json`

### Step 6 — Stop the containers

```bash
docker compose down
```

To also remove the SQL Server data volume (full reset):

```bash
docker compose down -v
```

---

## 8. Running Locally without Docker

If you prefer to run the API directly with the .NET CLI, you need a SQL Server instance accessible from your machine (local install or existing Docker container).

### Step 1 — Update the connection string

Edit `src/GrupoEmiTest.API/appsettings.Development.json` and set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost,1433;Database=GrupoEmiTest;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### Step 2 — Restore and run

```bash
cd src/GrupoEmiTest.API
dotnet restore
dotnet run
```

The application will apply migrations and seed data on startup, then listen on `http://localhost:5000` (or the port shown in the console).

### Step 3 — Open the API documentation

```
http://localhost:5000/scalar/v1
```

---

## 9. Default Seed Data & Credentials

On first startup, the application automatically seeds the database with realistic data. No manual setup is required.

### Pre-seeded Users

| Username | Email | Password | Role |
|---|---|---|---|
| `admin.grupoemi` | `admin@grupoemi.com` | `GrupoEmi2026!` | Admin |
| `user.grupoemi` | `user@grupoemi.com` | `GrupoEmi2026!` | User |

### Pre-seeded Departments

| Id | Name |
|---|---|
| 1 | Engineering |
| 2 | Marketing |
| 3 | Human Resources |

### Pre-seeded Projects

| Id | Name |
|---|---|
| 1 | ERP System Modernization |
| 2 | Digital Marketing Platform |
| 3 | Talent Management System |

### Pre-seeded Employees (19 records)

The seed includes employees across all departments, with varied positions (RegularEmployee through VicePresident), full position histories, and project assignments. Some notable records:

| Name | Position | Department | Salary |
|---|---|---|---|
| Roberto Sánchez | Director | Engineering | 12,000 |
| Patricia López | VicePresident | Marketing | 15,000 |
| Miguel Ángel Reyes | GeneralManager | Engineering | 11,000 |
| Carlos Méndez | DepartmentManager | Engineering | 8,500 |
| Ana García | TeamLead | Marketing | 6,200 |
| Luis Torres | RegularEmployee | Human Resources | 3,800 |

---

## 10. API Endpoints & Usage Guide

All endpoints except `/api/auth/register` and `/api/auth/login` require a valid JWT Bearer token in the `Authorization` header.

### Step-by-Step Walkthrough

#### Step 1 — Obtain a JWT token

**Login as Admin:**

```http
POST http://localhost:8081/api/auth/login
Content-Type: application/json

{
  "username": "admin.grupoemi",
  "password": "GrupoEmi2026!"
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "admin.grupoemi",
  "role": "Admin"
}
```

Copy the `token` value. All subsequent requests must include the header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

#### Step 2 — List all employees (paginated)

```http
GET http://localhost:8081/api/employees?pageSize=5
Authorization: Bearer <token>
```

**Response:**

```json
{
  "data": [ ... ],
  "nextCursor": 5,
  "hasNextPage": true
}
```

To get the next page, pass the `nextCursor` value as the `cursor` query parameter:

```http
GET http://localhost:8081/api/employees?pageSize=5&cursor=5
Authorization: Bearer <token>
```

#### Step 3 — Get a single employee by ID

```http
GET http://localhost:8081/api/employees/1
Authorization: Bearer <token>
```

#### Step 4 — Get employees by department with project assignments

This endpoint returns employees that belong to a specific department **and** are assigned to at least one project.

```http
GET http://localhost:8081/api/employees/by-department/1?pageSize=10
Authorization: Bearer <token>
```

(Department 1 = Engineering)

#### Step 5 — Create a new employee (Admin only)

```http
POST http://localhost:8081/api/employees
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "Juan Pérez",
  "currentPosition": 1,
  "salary": 4500.00,
  "departmentId": 1
}
```

Position values:
- `1` = RegularEmployee
- `2` = TeamLead
- `3` = DepartmentManager
- `4` = GeneralManager
- `5` = Director
- `6` = VicePresident

**Response:** `201 Created` with the created employee, including its new ID and an initial `PositionHistory` record.

#### Step 6 — Update an employee (Admin only)

```http
PUT http://localhost:8081/api/employees/1
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "name": "Carlos Méndez",
  "currentPosition": 4,
  "salary": 9500.00,
  "departmentId": 1
}
```

When the position changes, the previous `PositionHistory` record is automatically closed (its `EndDate` is set) and a new open history record is created — all within a single database transaction.

#### Step 7 — Delete an employee (Admin only)

```http
DELETE http://localhost:8081/api/employees/1
Authorization: Bearer <admin-token>
```

**Response:** `204 No Content`

#### Step 8 — Register a new API user

```http
POST http://localhost:8081/api/auth/register
Content-Type: application/json

{
  "username": "new.user",
  "email": "new@grupoemi.com",
  "password": "SecurePass123!",
  "role": 2
}
```

Role values: `1` = Admin, `2` = User.

### Complete Endpoint Reference

| Method | Route | Auth Required | Roles | Description |
|---|---|---|---|---|
| POST | `/api/auth/register` | No | — | Register a new API user |
| POST | `/api/auth/login` | No | — | Login and receive a JWT token |
| GET | `/api/employees` | Yes | Admin, User | Get paginated list of employees |
| GET | `/api/employees/{id}` | Yes | Admin, User | Get a single employee by ID |
| POST | `/api/employees` | Yes | Admin | Create a new employee |
| PUT | `/api/employees/{id}` | Yes | Admin | Update an existing employee |
| DELETE | `/api/employees/{id}` | Yes | Admin | Delete an employee |
| GET | `/api/employees/by-department/{id}` | Yes | Admin, User | Employees in a department assigned to at least one project |

---

## 11. Authentication & Authorization

### JWT Authentication

The API uses **JWT Bearer tokens** (`Microsoft.AspNetCore.Authentication.JwtBearer`). Tokens are generated by `TokenService` and signed using an HMAC-SHA256 key configured in `appsettings.json`:

```json
"Jwt": {
  "Secret": "super-secret-key-grupoemitest-2026!!",
  "Issuer": "GrupoEmiTest",
  "Audience": "GrupoEmiTest",
  "ExpiryHours": "1"
}
```

The token payload includes the user's ID, username, email, and role as claims.

### Role-Based Authorization via Policies

Rather than applying `[Authorize(Roles = "Admin")]` directly on actions (which scatters authorization logic across the codebase), the solution uses **named authorization policies** configured once in `DependencyInjectionAPI.cs`:

| Policy | Allowed Roles | Applied to |
|---|---|---|
| `ReadPolicy` | Admin, User | GET endpoints |
| `WritePolicy` | Admin | POST endpoint |
| `EditPolicy` | Admin | PUT endpoint |
| `DeletePolicy` | Admin | DELETE endpoint |

The `EmployeesController` is decorated with `[Authorize(Policy = "ReadPolicy")]` at the class level (affecting all GET operations), and individual write/edit/delete actions override it with their specific policy. This means:

- A **User** role token can call `GET /api/employees` and `GET /api/employees/{id}` successfully.
- A **User** role token attempting `POST`, `PUT`, or `DELETE` receives `403 Forbidden`.
- An unauthenticated request to any employee endpoint receives `401 Unauthorized`.

### Request Logging Middleware

A custom middleware (`RequestLoggingMiddleware`) is inserted into the pipeline before authentication. It logs every incoming request and its response:

```
[REQUEST]  GET /api/employees | TraceId: 0HN9...
[RESPONSE] GET /api/employees => 200 | Elapsed: 45.2ms | TraceId: 0HN9...
```

---

## 12. Keyset Pagination

Standard offset-based pagination (`SKIP`/`OFFSET`) degrades as the dataset grows because the database must scan and discard rows. This API uses **keyset pagination** (also called cursor-based or seek pagination).

**How it works:**

1. The first request omits the `cursor` parameter. The query filters `WHERE Id > 0` (effectively no filter), ordered by `Id`, and takes `pageSize + 1` rows.
2. If `pageSize + 1` rows are returned, there is a next page. The extra row is removed and the last visible row's `Id` is returned as `nextCursor`.
3. The next request passes `cursor=<nextCursor>`. The query filters `WHERE Id > cursor`, giving O(1) seek performance with a proper index on `Id`.

```
Request:  GET /api/employees?pageSize=5
Response: { data: [...5 items], nextCursor: 5, hasNextPage: true }

Request:  GET /api/employees?pageSize=5&cursor=5
Response: { data: [...5 items], nextCursor: 10, hasNextPage: true }

Request:  GET /api/employees?pageSize=5&cursor=10
Response: { data: [...4 items], nextCursor: null, hasNextPage: false }
```

---

## 13. Section-by-Section Test Answers

### Section 1: C# Programming

**Q1 — Employee class with bonus calculation and position history**

The `Employee` class is defined in `src/GrupoEmiTest.Domain/Entities/Employee.cs`. It satisfies all requirements:

- Properties: `Id`, `Name`, `CurrentPosition` (typed as `PositionType` enum rather than a raw `int` for type safety), `Salary`, `DepartmentId`.
- `CalculateYearlyBonus()` returns `Salary * 0.20m` for managerial positions and `Salary * 0.10m` for regular employees. The distinction is encapsulated in `PositionTypeExtensions.IsManagerPosition()`, which returns `true` for `TeamLead`, `DepartmentManager`, `GeneralManager`, `Director`, and `VicePresident`.
- "Many types of managers" is modeled through the `PositionType` enum, which has 5 managerial values (`TeamLead` through `VicePresident`). Adding a new managerial level requires only a new enum value — the bonus logic does not change.
- `PositionHistory` is defined in `src/GrupoEmiTest.Domain/Entities/PositionHistory.cs` with `EmployeeId`, `Position`, `StartDate`, and `EndDate` (nullable). It is linked to `Employee` via the `PositionHistories` navigation property (one-to-many). When a position change occurs during an update, the service closes the current record and opens a new one atomically.

---

### Section 2: ASP.NET Core

**Q1 — CRUD Web API endpoints**

All five required endpoints are implemented in `EmployeesController`:

| Endpoint | Method |
|---|---|
| `GET /api/employees` | `GetAll` — returns paginated list |
| `GET /api/employees/{id}` | `GetById` — returns employee with department, history, and projects |
| `POST /api/employees` | `Create` — creates employee and initial position history |
| `PUT /api/employees/{id}` | `Update` — updates employee and manages position history transition |
| `DELETE /api/employees/{id}` | `Delete` — removes employee record |

**Q2 — Authentication and authorization implementation**

Authentication is implemented with **JWT Bearer tokens**. On login, `AuthService` validates credentials, then `TokenService` creates a signed JWT with user claims (id, username, email, role). The token is returned to the client and must be sent as `Authorization: Bearer <token>` on all protected requests. ASP.NET Core's `UseAuthentication()` middleware validates the token signature and expiry on every request.

Authorization uses **named policies** (see [Section 11](#11-authentication--authorization)). Policies map to roles, and each controller action is decorated with the appropriate policy attribute.

**Q3 — Custom request logging middleware**

`RequestLoggingMiddleware` (in `src/GrupoEmiTest.API/Middlewares/RequestLoggingMiddleware.cs`) is a hand-written ASP.NET Core middleware that:

1. Records the start time and logs the HTTP method, path, query string, and trace ID before passing to the next component.
2. After the inner pipeline completes, logs the response status code and elapsed time in milliseconds.

It is registered in `Program.cs` via the `app.UseRequestLogging()` extension method, placed after `app.InitialiseDatabaseAsync()` and before `app.UseAuthentication()`.

---

### Section 3: Authentication and Authorization

**Q1 — Authentication controller with JWT**

`AuthController` (in `src/GrupoEmiTest.API/Controllers/AuthController.cs`) exposes:

- `POST /api/auth/register` — accepts `RegisterRequest` (username, email, password, role), hashes the password with `IPasswordHasher`, creates an `ApplicationUser`, persists it, and returns a JWT.
- `POST /api/auth/login` — accepts `LoginRequest` (username, password), finds the user, verifies the hashed password, and returns a JWT if valid. Returns `401 Unauthorized` on invalid credentials.

**Q2 — Role-based authorization (Admin / User)**

Two roles are defined in the `RoleType` enum:
- `Admin` (value `1`) — full access to all employee endpoints.
- `User` (value `2`) — read-only access (`GET` endpoints only).

The role is embedded in the JWT as a claim and checked by ASP.NET Core's policy evaluation on every request.

**Q3 — Protecting endpoints with roles**

The `EmployeesController` uses a layered policy approach:

```csharp
[Authorize(Policy = PolicyConstants.ReadPolicy)]   // class-level: Admin + User
public sealed class EmployeesController : ControllerBase
{
    [HttpGet]                                       // inherits ReadPolicy
    public async Task<IActionResult> GetAll(...) { }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.WritePolicy)] // overrides: Admin only
    public async Task<IActionResult> Create(...) { }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyConstants.EditPolicy)]  // overrides: Admin only
    public async Task<IActionResult> Update(...) { }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = PolicyConstants.DeletePolicy)] // overrides: Admin only
    public async Task<IActionResult> Delete(...) { }
}
```

---

### Section 4: Database Design and EF Core

**Q1 — SQL database schema**

The schema contains five tables: `Departments`, `Employees`, `Projects`, `EmployeeProjects`, and `PositionHistory` (plus `ApplicationUsers` for API authentication). The ER diagram is shown in [Section 5](#5-database-schema).

**Q2 — EF Core DbContext and entity configurations**

`GrupoEmiTestDBContext` inherits from `DbContext` and uses the Fluent API exclusively (no data annotations on entities). Each entity has a dedicated `IEntityTypeConfiguration<T>` class in `src/GrupoEmiTest.Infrastructure/Data/Configurations/`:

- `EmployeeConfiguration` — sets column types, required fields, relationship with Department (many-to-one), and cascading behavior.
- `PositionHistoryConfiguration` — configures the FK to Employee, `StartDate`/`EndDate` precision.
- `EmployeeProjectConfiguration` — configures the composite PK `(EmployeeId, ProjectId)` and both FK relationships.
- `DepartmentConfiguration` / `ProjectConfiguration` — basic table structure.
- `ApplicationUserConfiguration` — unique indexes on `Username` and `Email`.

**Q3 — LINQ query: employees in a department assigned to at least one project**

```csharp
// Implemented in EmployeeRepository.GetByDepartmentWithProjectsAsync
var employees = await _dbSet
    .AsNoTracking()
    .AsSplitQuery()
    .Include(e => e.Department)
    .Include(e => e.PositionHistories)
    .Include(e => e.EmployeeProjects)
        .ThenInclude(ep => ep.Project)
    .Where(e => e.DepartmentId == departmentId
             && e.EmployeeProjects.Any())
    .OrderBy(e => e.Id)
    .ToListAsync(cancellationToken);
```

The condition `e.EmployeeProjects.Any()` translates to `EXISTS (SELECT 1 FROM EmployeeProjects WHERE EmployeeId = e.Id)` in SQL, filtering out employees with no project assignments. `AsSplitQuery()` avoids a Cartesian product when joining multiple one-to-many collections simultaneously.

This query is exposed through `GET /api/employees/by-department/{id}`.

---

### Section 5: Performance and Optimization

**Q1 — Common performance issues in .NET and how to address them**

| Issue | Mitigation applied in this project |
|---|---|
| N+1 query problem | Eager loading with `Include` / `ThenInclude` on all detail queries |
| Cartesian explosion on multi-collection joins | `AsSplitQuery()` splits one multi-join query into separate targeted queries |
| Unnecessary change tracking on read operations | `AsNoTracking()` on all read-only queries in the repository |
| Offset pagination performance degradation | Keyset pagination eliminates `OFFSET`/`SKIP`; uses indexed `WHERE Id > cursor` instead |
| Synchronous blocking I/O | All repository, service, and controller methods are fully `async/await` |
| Over-fetching data | DTOs (`EmployeeResponse`, `EmployeeProjectResponse`) project only the fields the client needs, not full EF tracked entities |
| Exception-based control flow overhead | The Result pattern avoids `throw`/`catch` for expected business failures (not found, conflict, validation) |
| Memory leaks from DbContext | `UnitOfWork` is registered as `Scoped` (one per HTTP request) and implements `IDisposable` |

**Q2 — Profiling and optimizing a slow-running query**

The recommended approach in an ASP.NET Core application:

1. **Identify the slow query** — enable EF Core sensitive logging (`EnableSensitiveDataLogging`) in the Development environment to see the generated SQL in the console. Use Application Insights or OpenTelemetry to find slow request traces in production.

2. **Examine the generated SQL** — copy the query from the logs and run it directly in SSMS or Azure Data Studio with `SET STATISTICS IO ON; SET STATISTICS TIME ON;` to measure logical reads and CPU time.

3. **Check for missing indexes** — use `sys.dm_exec_query_stats` or the Missing Index DMVs (`sys.dm_db_missing_index_details`) to find index recommendations. Add indexes on foreign key columns and frequently filtered columns via EF Core's `HasIndex()` in entity configurations.

4. **Avoid SELECT * / over-fetching** — use `.Select(e => new EmployeeResponse { ... })` projections so EF Core generates a `SELECT` with only the required columns.

5. **Use AsSplitQuery where appropriate** — for queries that include multiple collection navigations, split queries reduce data transferred and can be faster than single joins with Cartesian product.

6. **Benchmark before and after** — use BenchmarkDotNet or simply measure with `Stopwatch` in an integration test to confirm the optimization had the intended effect.

---

### Additional Requirements

**Architectural Pattern:** Clean Architecture — described in [Section 1](#1-architecture-overview).

**Design Patterns (more than two implemented):**
- **Repository Pattern** — `IRepository<T>` and specialized variants.
- **Unit of Work Pattern** — `IUnitOfWork` coordinates repositories and transactions.
- **Result Pattern** — `Result` / `Result<T>` / `Error` replaces exceptions for business failures.
- **Factory Method Pattern** — `Employee.Create(...)`, `PositionHistory.Create(...)`, `ApplicationUser.Create(...)`.

**SOLID Principles:** Described in detail in [Section 2](#2-solid-principles-applied).
