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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc"))
        };
    });

//add services to the container
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<UserService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options=>options.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,36))));



var app = builder.Build();

app.UseHttpsRedirection();
//uma variavel que recebe o token e verifica ele antes de fazer cada requisição


string GenerateToken(string data)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var secretyKey = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcabc"); // esta chave será gravada em uma variavel de ambiente
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Expires = DateTime.UtcNow.AddHours(1), // O token expira em 1 hora
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(secretyKey),
            SecurityAlgorithms.HmacSha256Signature
         )
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token); // converte o token em string
}

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapPost("/login", async (HttpContext context) =>
{
//receber o request
using var reader = new StreamReader(context.Request.Body);
var body = await reader.ReadToEndAsync();
//Deserializar o objeto
var json = JsonDocument.Parse(body);
var username = json.RootElement.GetProperty("username").GetString();
var email = json.RootElement.GetProperty("email").GetString();
var senha = json.RootElement.GetProperty("senha").GetString();
//Esta parte do código será complementada com a service na próxima aula
var token = "";

if (senha == "1029")
{
token = GenerateToken(email); //O método generateToken será reimplementado
// em uma classe especializada
}
// return token;
await context.Response.WriteAsync(token);
});


//Cria um produto
/*
    Estrutura do JSON:
    {
    "id": 2,
    "nome": "Notebook Mac Air",
    "preco": 8700,
    "fornecedor": "Loja Apple Cwb"
    }
*/
//produtos
app.MapGet("/produtos",async(ProductService productService)=>
{
    
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
});


//por id

app.MapGet("/produtos/{id}", async (int id,ProductService productService) =>
{
    var produto = await productService.GetProductsByIdAsync(id);
    if(produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
});

app.MapPost("/produtos",async (Produto produto, ProductService productService)=>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPut("/produtos/{id}",async (int id, Produto produto, ProductService productService)=>
{
    if(id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }
    await productService.UpdateProductAsync(produto);
    return Results.Ok();
});

app.MapDelete("/produtos/{id}", async (int id, ProductService productService)=>
{
    await productService.DeleteProdctAsync(id);
    return Results.Ok();
});

//clientes
app.MapGet("/clientes", async(ClientService clientService) =>
{
    var clientes = await clientService.GetAllClientsAsync();
    return Results.Ok(clientes);
});

app.MapGet("/produtos/{id}", async (int id,ClientService clientService)=>
{
    var cliente = await clientService.GetClientByIdAsync(id);
    if(cliente == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(cliente);
});

app.MapPost("/clientes", async (Cliente cliente, ClientService clientService)=>
{
    await clientService.AddClientAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

app.MapPut("/clientes/{id}",async (int id, Cliente cliente,ClientService clientService)=>
{
    if(id!=cliente.Id)
    {
        return Results.BadRequest("Product Id mismatch.");
    }
    await clientService.UpdateClientAsync(cliente);
    return Results.Ok();
});

app.MapDelete("/clientes/{id}",async (int id, ClientService clientService)=>
{
    await clientService.DeleteClientAsync(id);
    return Results.Ok();
});

//fornecedores
app.MapGet("/fornecedores",async(SupplierService supplierService)=>
{
    var fornecedores = await supplierService.GetAllSuppliersAsync();
    return Results.Ok(fornecedores);
});

app.MapGet("/fornecedores/{id}", async(int id,SupplierService supplierService) =>
{
    var fornecedor = await supplierService.GetSupplierByIdAsync(id);
    if(fornecedor == null)
    {
        return Results.NotFound($"Supplier with ID {id} not found.");
    }

    return Results.Ok(fornecedor);

});

app.MapPost("/fornecedores",async (Fornecedor fornecedor, SupplierService supplierService)=>
{
    await supplierService.AddSupplierAsync(fornecedor);
    return Results.Created($"/fornecedores/{fornecedor.Id}", fornecedor);
});

app.MapPut("/fornecedores/{id}",async (int id, Fornecedor fornecedor, SupplierService supplierService)=>
{
    if(id != fornecedor.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }
    await supplierService.UpdateSupplierAsync(fornecedor);
    return Results.Ok();
});

app.MapDelete("/fornecedores/{id}",async (int id, SupplierService supplierService)=>
{
    await supplierService.DeleteSupplierAsync(id);
    return Results.Ok();
});


app.Run();


