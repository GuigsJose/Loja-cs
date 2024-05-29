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

//criação de cliente
app.MapPost("/createcliente", async (LojaDbContext dbContext, Cliente newCliente)=>
    {
        dbContext.Clientes.Add(newCliente);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/createcliente/{newCliente.Id}", newCliente);
    }
);

//listar clientes 
app.MapGet("/clientes", async (LojaDbContext dbContext) => 
    {
        var clientes = await dbContext.Clientes.ToListAsync();
        return Results.Ok(clientes);
    });


//buscar cliente por Id
app.MapGet("/clientes/{id}", async (int id, LojaDbContext dbContext) =>
    {
        var cliente = await dbContext.Clientes.FindAsync(id);
        if(cliente == null)
        {
            return Results.NotFound($"Produto with ID {id} not found.");
        }
        return Results.Ok(cliente);
    }
);

//endpoint para Atualizar clientes
app.MapPut("/clientes/{id}", async (int id, LojaDbContext dbContext,Cliente updateCliente) =>
    {
        //Verifica se o produto existe na base, com base no ID
        //Se o produto existir na base, sera retornado para dentro do objeto existingProduto
        var existingCliente = await dbContext.Clientes.FindAsync(id);
        if(existingCliente == null)
        {
            return Results.NotFound($"Cliente with ID {id} not found.");
        }
        //atualiza os dados do existingProduto
        existingCliente.Nome = updateCliente.Nome;
        existingCliente.Cpf = updateCliente.Cpf;
        existingCliente.Email = updateCliente.Email;

        //salva no banco
        await dbContext.SaveChangesAsync();

        //retorna para o cliente que invocou o endpoint
        return Results.Ok(existingCliente);
    }
);

//criação de cliente
app.MapPost("/createfornecedor", async (LojaDbContext dbContext, Fornecedor newFornecedor)=>
    {
        dbContext.Fornecedores.Add(newFornecedor);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/createcliente/{newFornecedor.Id}", newFornecedor);
    }
);

//listar clientes 
app.MapGet("/fornecedores", async (LojaDbContext dbContext) => 
    {
        var fornecedor = await dbContext.Fornecedores.ToListAsync();
        return Results.Ok(fornecedor);
    });


//buscar cliente por Id
app.MapGet("/fornecedores/{id}", async (int id, LojaDbContext dbContext) =>
    {
        var fornecedor = await dbContext.Fornecedores.FindAsync(id);
        if(fornecedor == null)
        {
            return Results.NotFound($"Produto with ID {id} not found.");
        }
        return Results.Ok(fornecedor);
    }
);

//endpoint para Atualizar clientes
app.MapPut("/fornecedores/{id}", async (int id, LojaDbContext dbContext,Fornecedor updateFornecedor) =>
    {
        //Verifica se o produto existe na base, com base no ID
        //Se o produto existir na base, sera retornado para dentro do objeto existingProduto
        var existingFornecedor = await dbContext.Fornecedores.FindAsync(id);
        if(existingFornecedor == null)
        {
            return Results.NotFound($"Cliente with ID {id} not found.");
        }
        //atualiza os dados do existingProduto
        existingFornecedor.Nome = updateFornecedor.Nome;
        existingFornecedor.Endereco = updateFornecedor.Endereco;
        existingFornecedor.Email = updateFornecedor.Email;
        existingFornecedor.Telefone = updateFornecedor.Telefone;

        //salva no banco
        await dbContext.SaveChangesAsync();

        //retorna para o cliente que invocou o endpoint
        return Results.Ok(existingFornecedor);
    }
);




app.Run();


