using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryStock.Tests;

public class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetProductById_ReturnsOk()
    {
        var newProduct = new
        {
            ProductName = "UnitTest Product",
            ProductPrice = 100.50m,
            ProductQuantity = 10,
            StockAvailable = 10,
            AddedAt = DateTime.Now
        };
        var postResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        postResponse.EnsureSuccessStatusCode();
        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductList>();
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
    [Fact]
    public async Task GetProductById_ReturnsNotFound()
    {
        int id = 000000;
        var response = await _client.GetAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        // Arrange
        var newProduct = new
        {
            ProductName = "PostTest Product",
            ProductPrice = 199.99m,
            ProductQuantity = 20,
            StockAvailable = 20,
            AddedAt = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductList>();
        Assert.NotNull(createdProduct);
        Assert.Equal("PostTest Product", createdProduct!.ProductName);
    }
    [Fact]
    public async Task CreateProduct_ReturnsBadRequest()
    {
        int id = 000000;
        var newProduct = new
        {
            ProductPrice = 199.99m,
            ProductQuantity = 20,
            StockAvailable = 20,
            AddedAt = DateTime.Now

        };
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task DeleteProduct_ReturnsOk()
    {
        // Step 1: Create a new product to delete
        var newProduct = new
        {
            ProductName = "DeleteTest Product",
            ProductPrice = 149.99m,
            ProductQuantity = 10,
            StockAvailable = 10,
            AddedAt = DateTime.Now
        };

        var postResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        postResponse.EnsureSuccessStatusCode();

        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductList>();
        Assert.NotNull(createdProduct);

        // Step 2: Delete the product
        var deleteResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
    [Fact]
    public async Task DeleteProduct_ReturnsNotFound()
    {
        int Id = 000000;
        var response = await _client.DeleteAsync($"/api/products/{Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOk()
    {
        var newProduct = new
        {
            ProductName = "Original Product",
            ProductPrice = 100.0m,
            ProductQuantity = 5,
            StockAvailable = 5,
            AddedAt = DateTime.Now
        };

        var postResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        postResponse.EnsureSuccessStatusCode();

        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ProductList>();
        Assert.NotNull(createdProduct);

        // Now, update that product
        var updatedProduct = new
        {
            ProductName = "Updated Product",
            ProductPrice = 200.0m,
            ProductQuantity = 10,
            StockAvailable = 10,
            AddedAt = DateTime.Now
        };

        var UpdatedResponse = await _client.PutAsJsonAsync($"/api/products/{createdProduct.Id}", updatedProduct);

        Assert.Equal(HttpStatusCode.OK, UpdatedResponse.StatusCode);
        var responseBody = await UpdatedResponse.Content.ReadFromJsonAsync<ProductList>();
        Assert.Equal("Updated Product", responseBody!.ProductName);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNotFound()
    {
        int Id = 000000;
        var updateData = new
        {
            ProductName = "No exist",
            ProductPrice = 123.45m,
            ProductQuantity = 1,
            StockAvailable = 1,
            AddedAt = DateTime.Now
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{Id}", updateData);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public async Task<int> CreateProductForStocks()
    {
        // Arrange
        var newProduct = new
        {
            ProductName = "PostTest Product",
            ProductPrice = 199.99m,
            ProductQuantity = 20,
            StockAvailable = 20,
            AddedAt = DateTime.Now
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductList>();
        return createdProduct.Id;

    }
    [Fact]
    public async Task DecrementStock_ValidProduct_ReturnsOk()
    {
        int productId = await CreateProductForStocks();
        var quantityToDecrement = 5;
        var response = await _client.PutAsync($"/api/products/decrement-stock/{productId}/{quantityToDecrement}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DecrementStock_ValidProduct_ReturnsBadRequest()
    {
        int productId = await CreateProductForStocks();
        var quantityToDecrement = 1000;
        var response = await _client.PutAsync($"/api/products/decrement-stock/{productId}/{quantityToDecrement}", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task IncrementStock_ValidProduct_ReturnsOk()
    {
        int productId = await CreateProductForStocks();
        var quantityToIncrement = 2;
        var response = await _client.PutAsync($"/api/products/add-to-stock/{productId}/{quantityToIncrement}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task IncrementStock_ValidProduct_ReturnsNotFound()
    {
        int productId = 00000;
        var quantityToIncrement = 100;
        var response = await _client.PutAsync($"/api/products/add-to-stock/{productId}/{quantityToIncrement}", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
