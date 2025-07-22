using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//Add Product//
app.MapPost("/api/products", async (AppDbContext db, ProductList product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created("/products/{product.Id}", product);
});


//Get all Products//
app.MapGet("/api/products", async (AppDbContext db) =>
{

    var products = await db.Products.ToListAsync();
    return Results.Ok(products ?? new List<ProductList>()); 
    //Results.Ok(products);
});


//Get  Product by Id//
app.MapGet("/api/products/{id}", async (AppDbContext db,int id) =>
{

    var product = await db.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound($"Product with ID {id} not found.");
   
});

//Delete Product by Id//
app.MapDelete("/api/products/{id}", async (AppDbContext db, int id) =>
{

    var deleteproduct = await db.Products.FindAsync(id);
    if (deleteproduct is null)
        return Results.NotFound($"Product with ID {id} not found.");
    else
    {
        db.Products.Remove(deleteproduct);
        await db.SaveChangesAsync();
        return Results.Ok($"Product with ID {id} deleted.");
    }
});


//Update Product by Id//
app.MapPut("/api/products/{id}", async (int id, ProductList updatedProduct, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound($"Product with ID {id} not found.");

    product.ProductName = updatedProduct.ProductName;
    product.ProductPrice = updatedProduct.ProductPrice;
    product.ProductQuantity = updatedProduct.ProductQuantity;
    product.StockAvailable = updatedProduct.StockAvailable;
    product.AddedAt = DateTime.Now;

    await db.SaveChangesAsync();

    return Results.Ok(product);
});


//Decrease Stock of a product by Id//
app.MapPut("/api/products/decrement-stock/{id}/{quantity}", async (int id, int quantity, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound($"Product with ID {id} not found.");

    if (product.StockAvailable < quantity)
        return Results.BadRequest("Not enough stock available to decrement.");

    product.StockAvailable =product.StockAvailable- quantity;
    await db.SaveChangesAsync();
    return Results.Ok($"Stock decremented.");
});

//Increase Stock of a product by Id//

app.MapPut("/api/products/add-to-stock/{id}/{quantity}", async (int id, int quantity, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound($"Product with ID {id} not found.");

    product.StockAvailable = product.StockAvailable + quantity;
    await db.SaveChangesAsync();
    return Results.Ok($"Stock added.");
});

app.Run();


public class ProductList
{
    [Key]
    public int Id { get; set; } 

    public required string ProductName { get; set; }

    [Range(0,double.MaxValue)]
    public required decimal ProductPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int ProductQuantity { get; set; }

    [Range(0, int.MaxValue)]
    public int StockAvailable { get; set; }

    public DateTime AddedAt { get; set; }

}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<ProductList> Products => Set<ProductList>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductList>()
            .Property(p => p.Id)
            .UseIdentityColumn(seed: 100000, increment: 1);
    }
    
}
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server={ServerName};Database={DbName};Trusted_Connection=True;Encrypt=False;");

        return new AppDbContext(optionsBuilder.Options);
    }
}

public partial class Program { } // Needed for WebApplicationFactory
