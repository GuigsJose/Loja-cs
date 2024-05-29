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
app.MapPost("/createproduto", async(LojaDbContext dbContext, Produto newProduto) =>
    {
        dbContext.Produtos.Add(newProduto);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/createproduto/{newProduto.Id}", newProduto);
    }
);

//Lista todos os produtos
app.MapGet("/produtos", async (LojaDbContext dbContext) => 
    {
        var produtos = await dbContext.Produtos.ToListAsync();
        return Results.Ok(produtos);
    });

//consulta por ID
app.MapGet("/produtos/{id}", async (int id, LojaDbContext dbContext) =>
    {
        var produto = await dbContext.Produtos.FindAsync(id);
        if(produto == null)
        {
            return Results.NotFound($"Produto with ID {id} not found.");
        }
        return Results.Ok(produto);
    }
);

//endpoint para Atualizar produtos
app.MapPut("/produtos/{id}", async (int id, LojaDbContext dbContext,Produto updateProduto) =>
    {
        //Verifica se o produto existe na base, com base no ID
        //Se o produto existir na base, sera retornado para dentro do objeto existingProduto
        var existingProduto = await dbContext.Produtos.FindAsync(id);
        if(existingProduto == null)
        {
            return Results.NotFound($"Produto with ID {id} not found.");
        }
        //atualiza os dados do existingProduto
        existingProduto.Nome = updateProduto.Nome;
        existingProduto.Preco = updateProduto.Preco;
        existingProduto.Fornecedor = updateProduto.Fornecedor;

        //salva no banco
        await dbContext.SaveChangesAsync();

        //retorna para o cliente que invocou o endpoint
        return Results.Ok(existingProduto);
    }
);







app.Run();


