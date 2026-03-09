# 📦 Inventory Management System

> A clean, well-structured RESTful API built with **ASP.NET Core 9** following **Clean Architecture** principles. This system manages product inventory with image upload support, caching, structured logging, and global error handling.

---

## 🚀 GitHub Repository Description

> **Inventory Management API** — A .NET 9 Web API built with Clean Architecture (Domain → Application → Infrastructure → API). Features product CRUD, image upload, in-memory caching, Serilog logging, and global error handling. Uses Entity Framework Core with SQL Server.

---

## ✨ Features

- 📋 **Product Management** — Create products and retrieve them with pagination and search
- 🖼️ **Image Upload** — Upload product images (JPG/PNG, max 2MB) stored to `wwwroot/images/`
- 🔍 **Search & Pagination** — Filter products by name with server-side pagination
- ⚡ **In-Memory Caching** — Product list responses cached for 5 minutes to reduce DB hits
- 📝 **Structured Logging** — Serilog logs to both Console and rolling daily log files
- 🛡️ **Global Error Handling** — Centralized middleware catches unhandled exceptions and returns clean JSON error responses
- 📚 **Swagger / OpenAPI** — Interactive API documentation available in Development mode
- 🗄️ **SQL Server + EF Core** — Code-first database with EF Core Migrations

---

## 🏛️ Architecture

This project follows the **Clean Architecture** pattern, separating concerns across four distinct layers. Each layer depends only on the layers **inward** — never outward.

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│  Controllers · Middleware · Program.cs                  │
├─────────────────────────────────────────────────────────┤
│                 Application Layer                       │
│  Features (Commands/Queries/Handlers) · DTOs            │
│  Interfaces · Behaviors                                 │
├─────────────────────────────────────────────────────────┤
│               Infrastructure Layer                      │
│  Persistence (EF Core) · Services (Cache, File)         │
│  Logging (Serilog) · Migrations                         │
├─────────────────────────────────────────────────────────┤
│                  Domain Layer                           │
│  Entities · Common (BaseEntity) · Exceptions            │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| **Domain** | Core business entities and rules. Zero external dependencies. |
| **Application** | Business use-cases (features), interfaces, and DTOs. Depends only on Domain. |
| **Infrastructure** | Implements interfaces: EF Core DB, caching, file storage, and logging. |
| **API** | HTTP entry-point. Controllers, Middleware, DI wiring in `Program.cs`. |

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| **.NET 9 / ASP.NET Core 9** | Web framework |
| **Entity Framework Core** | ORM / Code-first DB access |
| **SQL Server (LocalDB / SQLEXPRESS)** | Relational database |
| **Serilog** | Structured logging (Console + File) |
| **IMemoryCache** | In-process caching |
| **Swagger / Swashbuckle** | API documentation |
| **Clean Architecture** | Project structure / design pattern |

---

## 📁 Project Structure

```
InventoryManagement/
│
├── InventoryManagement.sln
│
├── InventoryManagement.Domain/              # 🏛️ Domain Layer (innermost)
│   ├── Common/
│   │   └── BaseEntity.cs                   # Abstract base with Id & CreatedAt
│   ├── Entities/
│   │   └── Product.cs                      # Product domain entity
│   └── Exceptions/                         # Custom domain exceptions
│
├── InventoryManagement.Application/        # ⚙️ Application Layer
│   ├── DTOs/
│   │   ├── ProductDto.cs                   # Response DTO
│   │   └── ProductCreateRequest.cs         # Request DTO (with image file)
│   ├── Features/
│   │   └── Products/
│   │       ├── Commands/
│   │       │   └── CreateProductCommand.cs # Command model
│   │       ├── Queries/
│   │       │   └── GetProductsQuery.cs     # Query model (page, pageSize, search)
│   │       └── Handlers/
│   │           └── ProductHandlers.cs      # Business logic handler
│   ├── Interfaces/
│   │   ├── IProductRepository.cs           # Repository contract
│   │   ├── ICacheService.cs                # Cache service contract
│   │   └── IFileStorageService.cs          # File storage contract
│   └── Behaviors/                          # Pipeline behaviors (e.g., validation)
│
├── InventoryManagement.Infrastructure/     # 🔧 Infrastructure Layer
│   ├── Persistence/
│   │   ├── AppDbContext.cs                 # EF Core DbContext
│   │   └── ProductRepository.cs           # IProductRepository implementation
│   ├── Services/
│   │   ├── CacheService.cs                # IMemoryCache implementation
│   │   └── FileStorageService.cs          # Local file storage implementation
│   ├── Logging/
│   │   └── SerilogConfig.cs               # Serilog bootstrap configuration
│   └── Migrations/                        # EF Core generated migrations
│
└── InventoryManagement.API/               # 🌐 API Layer (outermost)
    ├── Controllers/
    │   └── ProductsController.cs           # Products REST controller
    ├── Middleware/
    │   └── ErrorHandlingMiddleware.cs      # Global exception handler
    ├── Logs/                               # Runtime log files (daily rolling)
    ├── Program.cs                          # App bootstrap & DI setup
    └── appsettings.json                    # Configuration (connection strings, etc.)
```

---

## 🗄️ Database Schema

### `Products` Table

| Column | Type | Description |
|---|---|---|
| `Id` | `int` (PK) | Auto-increment primary key |
| `Name` | `nvarchar` | Product name |
| `Price` | `decimal` | Product price |
| `Stock` | `int` | Quantity in stock |
| `ImageUrl` | `nvarchar` (nullable) | Relative URL to uploaded image |
| `CreatedAt` | `datetime2` | Record creation timestamp (UTC) |

---

## 📡 API Endpoints

### Products — `api/products`

| Method | Endpoint | Description | Body |
|---|---|---|---|
| `POST` | `/api/products` | Create a new product with image | `multipart/form-data` |
| `GET` | `/api/products/get-all` | Get paginated & searchable product list | Query params |

#### POST `/api/products` — Request (multipart/form-data)

| Field | Type | Required | Notes |
|---|---|---|---|
| `Name` | `string` | ✅ | Product name |
| `Price` | `decimal` | ✅ | Product price |
| `Stock` | `int` | ✅ | Stock quantity |
| `Image` | `file` | ✅ | JPG or PNG, max 2MB |

#### GET `/api/products/get-all` — Query Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page |
| `search` | `string` | `null` | Filter by product name |

#### Sample Response — Get Products

```json
{
  "items": [
    {
      "id": 1,
      "name": "Wireless Mouse",
      "price": 29.99,
      "stock": 150,
      "imageUrl": "/images/a3f1e2c7-mouse.png"
    }
  ],
  "totalCount": 1
}
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or SQLEXPRESS)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/InventoryManagement.git
cd InventoryManagement
```

### 2. Configure the Database Connection

Open `InventoryManagement.API/appsettings.json` and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Apply EF Core Migrations

```bash
cd InventoryManagement.API
dotnet ef database update --project ../InventoryManagement.Infrastructure
```

### 4. Run the Application

```bash
dotnet run --project InventoryManagement.API
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger` (Development only)

---

## 📋 Key Design Decisions

### ✅ Clean Architecture Layers
Dependencies point inward — the Domain knows nothing about Infrastructure, and the Application knows nothing about how data is stored or how files are served.

### ✅ CQRS-Inspired Feature Folders
Features are organized as `Commands` (write) and `Queries` (read) under the `Features/Products/` folder, making intent explicit and scalable.

### ✅ Repository Pattern
`IProductRepository` is defined in the Application layer and implemented in Infrastructure. This makes the application testable without a real database.

### ✅ Interface-Driven Services
Both `ICacheService` and `IFileStorageService` are Application-layer contracts, so implementations (in-memory cache, local file system) can be swapped without touching business logic.

### ✅ Global Exception Middleware
`ErrorHandlingMiddleware` catches all unhandled exceptions globally and returns a consistent `{ "message": "..." }` JSON response — no leaking stack traces.

### ✅ Serilog Structured Logging
Logs are written to both the console and rolling daily files (`Logs/log-YYYYMMDD.txt`), making debugging easy in both development and production.

---

## 📌 What I Learned (First Clean Architecture Project)

This project was my hands-on introduction to Clean Architecture. Key takeaways:

- 🧅 **Onion/Clean architecture** enforces separation of concerns at the project level, not just class level
- 📦 **Interfaces live in Application**, not Infrastructure — this is the key inversion of dependency
- 🔀 **CQRS** (Commands vs Queries) keeps read and write logic clean and independent
- 🏗️ **BaseEntity** avoids repeating `Id` and `CreatedAt` across every entity
- 🧪 **Testability** — because repositories are behind interfaces, unit testing handlers with mocks is straightforward

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

<p align="center">Built with ❤️ as a first Clean Architecture project in .NET 9</p>
