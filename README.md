# 📦 Inventory Management System

A production-ready **RESTful API** built with **.NET 9** and **Clean Architecture**, designed for managing product inventory with file upload support, in-memory caching, structured logging, and a global error handling pipeline.

---

## 🚀 Features

- ✅ **Clean Architecture** — strict separation of concerns across four distinct layers
- ✅ **CQRS Pattern** — Commands and Queries clearly separated in the Application layer
- ✅ **Product Image Upload** — file type & size validation with local disk storage
- ✅ **Paginated Product Listing** — server-side pagination and keyword search
- ✅ **In-Memory Caching** — `IMemoryCache` via `CacheService` (5-minute TTL)
- ✅ **Global Error Handling** — custom `ErrorHandlingMiddleware` returning structured JSON errors
- ✅ **Structured Logging** — Serilog writing to Console and daily rolling log files
- ✅ **Swagger / OpenAPI** — interactive API documentation in the Development environment
- ✅ **Entity Framework Core 9** — SQL Server provider with EF Migrations

---

## 🏛️ Architecture

This project follows **Clean Architecture** principles. Dependencies flow strictly inward — outer layers depend on inner layers, never the reverse.

```
┌──────────────────────────────────────────────────────┐
│                  API  (Presentation)                 │
│         Controllers · Middleware · Program.cs        │
├──────────────────────────────────────────────────────┤
│               Application  (Use Cases)               │
│    Features (Commands/Queries/Handlers) · DTOs       │
│            Interfaces · Behaviors                    │
├──────────────────────────────────────────────────────┤
│                  Domain  (Core)                      │
│          Entities · BaseEntity · Exceptions          │
├──────────────────────────────────────────────────────┤
│            Infrastructure  (Data & Services)         │
│    EF Core · Repositories · CacheService             │
│         FileStorageService · Serilog · Migrations    │
└──────────────────────────────────────────────────────┘
```

---

## 🗂️ Project Structure

```
InventoryManagement/
├── InventoryManagement.sln
│
├── InventoryManagement.API/                  # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── ProductsController.cs            # Product endpoints (POST, GET)
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs       # Global exception handler
│   ├── Logs/                                # Daily Serilog rolling log files
│   ├── appsettings.json                     # DB connection string & logging config
│   └── Program.cs                           # DI registration & middleware pipeline
│
├── InventoryManagement.Application/         # Business Logic / Use Cases
│   ├── Behaviors/
│   │   └── Pagination.cs                   # Pagination helper
│   ├── DTOs/
│   │   ├── ProductDto.cs                   # Read model returned to client
│   │   └── ProductCreateRequest.cs         # Multipart/form-data input model
│   ├── Exceptions/                         # Application-level custom exceptions
│   ├── Features/
│   │   └── Products/
│   │       ├── Commands/
│   │       │   └── CreateProductCommand.cs # Create product command model
│   │       ├── Queries/
│   │       │   └── GetProductsQuery.cs     # Paginated get query model
│   │       └── Handlers/
│   │           └── ProductHandlers.cs      # Business logic for create & list
│   └── Interfaces/
│       ├── IProductRepository.cs           # Repository abstraction
│       ├── ICacheService.cs                # Cache abstraction
│       └── IFileStorageService.cs          # File storage abstraction
│
├── InventoryManagement.Domain/              # Core Domain (no dependencies)
│   ├── Common/
│   │   └── BaseEntity.cs                   # Id + CreatedAt base class
│   ├── Entities/
│   │   └── Product.cs                      # Product domain entity
│   └── Exceptions/                         # Domain-level custom exceptions
│
└── InventoryManagement.Infrastructure/     # Data & External Services
    ├── Logging/
    │   └── SerilogConfig.cs                # Serilog bootstrap configuration
    ├── Migrations/
    │   └── 20260102111341_InitialCreate.*  # EF Core initial migration
    ├── Persistence/
    │   ├── AppDbContext.cs                 # EF Core DbContext
    │   └── ProductRepository.cs           # IProductRepository implementation
    └── Services/
        ├── CacheService.cs                 # IMemoryCache implementation
        └── FileStorageService.cs           # Local disk file storage implementation
```

---

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 9 |
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Logging | Serilog (Console + Rolling File) |
| Caching | `Microsoft.Extensions.Caching.Memory` |
| API Docs | Swashbuckle / Swagger (OpenAPI v1) |
| Pattern | Clean Architecture + CQRS |

---

## 📐 Domain Model

### `Product` entity

| Property | Type | Description |
|---|---|---|
| `Id` | `int` | Auto-generated primary key _(from BaseEntity)_ |
| `CreatedAt` | `DateTime` | UTC creation timestamp _(from BaseEntity)_ |
| `Name` | `string` | Product name (required) |
| `Price` | `decimal` | Unit price |
| `Stock` | `int` | Available quantity |
| `ImageUrl` | `string?` | Relative path to the uploaded image |

---

## 🔌 API Endpoints

Base URL: `https://localhost:{port}/api`

### Products

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/products` | Create a new product with image upload |
| `GET` | `/api/products/get-all` | Retrieve all products (paginated, searchable) |

---

### `POST /api/products`

Creates a new product. Accepts `multipart/form-data`.

**Request fields:**

| Field | Type | Validation |
|---|---|---|
| `Name` | `string` | Required |
| `Price` | `decimal` | Required |
| `Stock` | `int` | Required |
| `Image` | `IFormFile` | Required · Only `image/jpeg` or `image/png` · Max **2 MB** |

**Success Response `200 OK`:**
```json
{
  "message": "Product created successfully"
}
```

**Error Responses:**
- `400 Bad Request` — Image missing, invalid type, or exceeds 2 MB
- `500 Internal Server Error` — Unhandled exception (returned as JSON)

---

### `GET /api/products/get-all`

Returns a paginated list of products.

**Query Parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page |
| `search` | `string?` | — | Filter by product name (case-insensitive contains) |

**Success Response `200 OK`:**
```json
{
  "totalCount": 42,
  "data": [
    {
      "id": 1,
      "name": "Wireless Keyboard",
      "price": 49.99,
      "stock": 120,
      "imageUrl": "/images/3f2a1b4c-...png",
      "createdAt": "2026-01-02T11:13:41Z"
    }
  ]
}
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, or full)
- Visual Studio 2022+ or VS Code

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/InventoryManagement.git
cd InventoryManagement
```

### 2. Configure the Database Connection

Edit `InventoryManagement.API/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply Database Migrations

```bash
cd InventoryManagement.API
dotnet ef database update --project ../InventoryManagement.Infrastructure
```

### 4. Run the Application

```bash
dotnet run --project InventoryManagement.API
```

The API will start at `https://localhost:7xxx` / `http://localhost:5xxx`.  
Swagger UI is available at: **`https://localhost:{port}/swagger`** _(Development only)_

---

## 📁 File Upload

Uploaded product images are stored locally under `wwwroot/images/` with a GUID-prefixed filename to prevent naming collisions.

- **Allowed types:** `image/jpeg`, `image/png`
- **Maximum size:** 2 MB
- **Returned path example:** `/images/3f2a1b4c-d5e6-7f8a-9b0c-1d2e3f4a5b6c.jpg`

To serve these files, make sure `UseStaticFiles()` is enabled (already configured in `Program.cs`).

---

## 📋 Logging

Serilog is bootstrapped before the host is built (`SerilogConfig.Configure()`) and configured with two sinks:

| Sink | Details |
|---|---|
| **Console** | Real-time structured logs in the terminal |
| **Rolling File** | Daily log files written to `Logs/log-YYYYMMDD.txt` |

Log level defaults to `Information`. `Microsoft.AspNetCore` logs are filtered to `Warning` to reduce noise.

---

## 🧩 Key Design Decisions

| Decision | Rationale |
|---|---|
| **CQRS without MediatR** | Commands and Queries are manually dispatched through `ProductHandlers`, keeping the implementation lightweight without the overhead of a full MediatR pipeline |
| **Repository Pattern** | `IProductRepository` decouples the Application layer from EF Core, making the data access layer easily replaceable and testable |
| **Interface-driven services** | `ICacheService` and `IFileStorageService` abstractions allow swapping implementations (e.g., Redis, Azure Blob) without touching business logic |
| **Global Error Middleware** | `ErrorHandlingMiddleware` catches all unhandled exceptions and returns a consistent JSON error envelope |
| **Serilog early bootstrap** | Configured before `WebApplication.CreateBuilder` to capture startup errors |

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'feat: add your feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

This project is open-source. See the [LICENSE](LICENSE) file for details.
