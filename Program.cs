using Microsoft.EntityFrameworkCore;
using loja.models;
using loja.data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options=>options.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,36))));



var app = builder.Build();


app.MapPost("/createproduto", async(LojaDbContext dbContext, Produto newProduto) =>
    {
        dbContext.Produtos.Add(newProduto);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/createproduto/{newProduto.Id}", newProduto);
    }
);

app.Run();


