using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using loja.models;
using loja.data;
using loja.services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

var secretKey = "abcabcabcabcabcabcabcabcabcabcabc"; // Idealmente, isto deve ser armazenado em uma variável de ambiente
var autorizador = new Autorizador(secretKey);

builder.Services.AddSingleton(autorizador);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };
    });

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VendaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapPost("/login", async (HttpContext context, Autorizador autorizador) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var json = JsonDocument.Parse(body);
    var username = json.RootElement.GetProperty("username").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();
    var token = "";

    if (senha == "1029")
    {
        token = autorizador.GerarToken(email);
    }
    await context.Response.WriteAsync(token);
});

app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
}).RequireAuthorization();

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductsByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
}).RequireAuthorization();

app.MapPost("/produtos", async (Produto produto, ProductService productService) =>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
}).RequireAuthorization();

app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
{
    if (id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }
    await productService.UpdateProductAsync(produto);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProdctAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Clientes
app.MapGet("/clientes", async (ClientService clientService) =>
{
    var clientes = await clientService.GetAllClientsAsync();
    return Results.Ok(clientes);
}).RequireAuthorization();

app.MapGet("/clientes/{id}", async (int id, ClientService clientService) =>
{
    var cliente = await clientService.GetClientByIdAsync(id);
    if (cliente == null)
    {
        return Results.NotFound($"Client with ID {id} not found.");
    }
    return Results.Ok(cliente);
}).RequireAuthorization();

app.MapPost("/clientes", async (Cliente cliente, ClientService clientService) =>
{
    await clientService.AddClientAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
}).RequireAuthorization();

app.MapPut("/clientes/{id}", async (int id, Cliente cliente, ClientService clientService) =>
{
    if (id != cliente.Id)
    {
        return Results.BadRequest("Client ID mismatch.");
    }
    await clientService.UpdateClientAsync(cliente);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/clientes/{id}", async (int id, ClientService clientService) =>
{
    await clientService.DeleteClientAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Fornecedores
app.MapGet("/fornecedores", async (SupplierService supplierService) =>
{
    var fornecedores = await supplierService.GetAllSuppliersAsync();
    return Results.Ok(fornecedores);
}).RequireAuthorization();

app.MapGet("/fornecedores/{id}", async (int id, SupplierService supplierService) =>
{
    var fornecedor = await supplierService.GetSupplierByIdAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Supplier with ID {id} not found.");
    }
    return Results.Ok(fornecedor);
}).RequireAuthorization();

app.MapPost("/fornecedores", async (Fornecedor fornecedor, SupplierService supplierService) =>
{
    await supplierService.AddSupplierAsync(fornecedor);
    return Results.Created($"/fornecedores/{fornecedor.Id}", fornecedor);
}).RequireAuthorization();

app.MapPut("/fornecedores/{id}", async (int id, Fornecedor fornecedor, SupplierService supplierService) =>
{
    if (id != fornecedor.Id)
    {
        return Results.BadRequest("Supplier ID mismatch.");
    }
    await supplierService.UpdateSupplierAsync(fornecedor);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/fornecedores/{id}", async (int id, SupplierService supplierService) =>
{
    await supplierService.DeleteSupplierAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Vendas
app.MapPost("/vendas", async (Venda venda, VendaService vendaService) =>
{
    try
    {
        var novaVenda = await vendaService.AddVendaAsync(venda);
        return Results.Created($"/vendas/{novaVenda.Id}", novaVenda);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/detalhada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasPorProdutoDetalhadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/sumarizada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasPorProdutoSumarizadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/detalhada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasPorClienteDetalhadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/sumarizada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasPorClienteSumarizadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.Run();
