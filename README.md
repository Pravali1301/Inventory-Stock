 Inventory Stock Management API

This is a .NET 8 Minimal API project designed for managing product inventory. The API supports CRUD operations, stock updates, and is backed by Entity Framework Core with SQL Server. It includes integration tests using xUnit.

---

## 🛠️ Tech Stack

- [.NET 8]
- **Entity Framework Core** (Code First)
- **SQL Server** (Local/Remote)
- **Minimal API**
- **xUnit** (for API testing)
- **Swagger UI**

---

##  Features

- Add, view, update, delete products.
- Auto-generated 6-digit unique Product IDs (seed starts at 100000).
- Stock management: increment/decrement product stock.
- Swagger documentation UI.
- Integration tests for API endpoints.

---

##  Project Structure

```bash
C:\Users\durga.p.chodipindi\
│
├── InventoryStock/             # Main API project
│   ├── Program.cs              # All minimal APIs and configuration
│   ├── appsettings.json        # Contains DB connection string
│   ├── appsettings.Development.json
│   ├── InventoryStock.csproj   # API project file
│   ├── InventoryStock.sln      # Solution file
│   ├── ProductList.cs          # Product model
│   ├── AppDbContext.cs         # EF Core DB context
│   └── AppDbContextFactory.cs
│
└── InventoryStock.Test/        # Test project using xUnit
    ├── ProductApiTests.cs
    └── InventoryStock.Test.csproj
```

---



### Prerequisites

- [.NET 8 SDK]
- SQL Server (local or cloud)
- Visual Studio or VS Code

---

###  Setup Instructions

1. **Clone the repository**

```bash
git clone https://github.com/Pravali1301/Inventory-Stock.git
cd Inventory-Stock
Move the InventoryStock.Test Folder as shown in project structure. 
```

2. **Configure SQL Server connection string**

Update the `appsettings.json` in `InventoryStock/`:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ProductsDb;Trusted_Connection=True;Encrypt=False;"
}


> Note: The connection string is scrubbed in the repo. Replace with your actual SQL Server details.

---

3. **Apply EF Core Migrations & Create DB**

```bash
cd InventoryStock
dotnet ef database update
```

---

4. **Run the API**

```bash
dotnet run
```

- Navigate to [https://localhost:<port>/swagger](https://localhost:<port>/swagger) for API documentation and testing.

---

### ✅ Running the Tests

```bash
cd InventoryStock.Test
dotnet test
```

> The tests use the same SQL Server database. Ensure the connection string in `InventoryStock/appsettings.json` is valid and DB is created.

---

## API Endpoints Summary

| Method | Endpoint                                 | Description                      |
|--------|------------------------------------------|----------------------------------|
| GET    | `/api/products`                              | Get all products                 |
| GET    | `/api/products/{id}`                         | Get product by ID                |
| POST   | `/api/products`                              | Create a new product             |
| PUT    | `/api/products/{id}`                     | Update an existing product       |
| DELETE | `/api/products/{id}`                         | Delete a product                 |
| PUT    | `/api/products/add-to-stock/{id}/{q}` | Add quantity to product stock    |
| PUT    | `/api/products/decrement-stock/{id}/{q}` | Reduce quantity from product     |

