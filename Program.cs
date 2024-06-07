using Microsoft.EntityFrameworkCore;
using loja.models;
using loja.data;
using loja.services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

//add services to the container
builder.Services.AddScoped<ProductService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options=>options.UseMySql(connectionString, new MySqlServerVersion(new Version(8,0,36))));



var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


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

app.MapPost("/clientes{id}", async (Cliente cliente, ClientService clientService)=>
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


//old version
// app.MapPost("/createproduto", async(LojaDbContext dbContext, Produto newProduto) =>
//     {
//         dbContext.Produtos.Add(newProduto);
//         await dbContext.SaveChangesAsync();
//         return Results.Created($"/createproduto/{newProduto.Id}", newProduto);
//     }
// );

// //Lista todos os produtos
// app.MapGet("/produtos", async (LojaDbContext dbContext) => 
//     {
//         var produtos = await dbContext.Produtos.ToListAsync();
//         return Results.Ok(produtos);
//     });

// //consulta por ID
// app.MapGet("/produtos/{id}", async (int id, LojaDbContext dbContext) =>
//     {
//         var produto = await dbContext.Produtos.FindAsync(id);
//         if(produto == null)
//         {
//             return Results.NotFound($"Produto with ID {id} not found.");
//         }
//         return Results.Ok(produto);
//     }
// );

// //endpoint para Atualizar produtos
// app.MapPut("/produtos/{id}", async (int id, LojaDbContext dbContext,Produto updateProduto) =>
//     {
//         //Verifica se o produto existe na base, com base no ID
//         //Se o produto existir na base, sera retornado para dentro do objeto existingProduto
//         var existingProduto = await dbContext.Produtos.FindAsync(id);
//         if(existingProduto == null)
//         {
//             return Results.NotFound($"Produto with ID {id} not found.");
//         }
//         //atualiza os dados do existingProduto
//         existingProduto.Nome = updateProduto.Nome;
//         existingProduto.Preco = updateProduto.Preco;
//         existingProduto.Fornecedor = updateProduto.Fornecedor;

//         //salva no banco
//         await dbContext.SaveChangesAsync();

//         //retorna para o cliente que invocou o endpoint
//         return Results.Ok(existingProduto);
//     }
// );

// // Deletar produto
// app.MapDelete("/produtos/{id}", async (int id, LojaDbContext dbContext)=>
// {
//     var produto = await dbContext.Produtos.FindAsync(id);
//     if(produto == null)
//     {
//         return Results.NotFound();
//     }

//     dbContext.Produtos  .Remove(produto);

//     return Results.NoContent();
// });

//old version
//criação de cliente
// app.MapPost("/createcliente", async (LojaDbContext dbContext, Cliente newCliente)=>
//     {
//         dbContext.Clientes.Add(newCliente);
//         await dbContext.SaveChangesAsync();
//         return Results.Created($"/createcliente/{newCliente.Id}", newCliente);
//     }
// );

// //listar clientes 
// app.MapGet("/clientes", async (LojaDbContext dbContext) => 
//     {
//         var clientes = await dbContext.Clientes.ToListAsync();
//         return Results.Ok(clientes);
//     });


// //buscar cliente por Id
// app.MapGet("/clientes/{id}", async (int id, LojaDbContext dbContext) =>
//     {
//         var cliente = await dbContext.Clientes.FindAsync(id);
//         if(cliente == null)
//         {
//             return Results.NotFound($"Produto with ID {id} not found.");
//         }
//         return Results.Ok(cliente);
//     }
// );

// //endpoint para Atualizar clientes
// app.MapPut("/clientes/{id}", async (int id, LojaDbContext dbContext,Cliente updateCliente) =>
//     {
//         //Verifica se o cliente existe na base, com base no ID
//         //Se o cliente existir na base, sera retornado para dentro do objeto existingProduto
//         var existingCliente = await dbContext.Clientes.FindAsync(id);
//         if(existingCliente == null)
//         {
//             return Results.NotFound($"Cliente with ID {id} not found.");
//         }
//         //atualiza os dados do existingCliente
//         existingCliente.Nome = updateCliente.Nome;
//         existingCliente.Cpf = updateCliente.Cpf;
//         existingCliente.Email = updateCliente.Email;

//         //salva no banco
//         await dbContext.SaveChangesAsync();

//         //retorna para o cliente que invocou o endpoint
//         return Results.Ok(existingCliente);
//     }
// );

// // Deletar cliente
// app.MapDelete("/clientes/{id}", async (int id, LojaDbContext dbContext)=>
// {
//     var cliente = await dbContext.Clientes.FindAsync(id);
//     if(cliente == null)
//     {
//         return Results.NotFound();
//     }

//     dbContext.Clientes.Remove(cliente);

//     return Results.NoContent();
// });

// //criação de fornecedor
// app.MapPost("/createfornecedor", async (LojaDbContext dbContext, Fornecedor newFornecedor)=>
//     {
//         dbContext.Fornecedores.Add(newFornecedor);
//         await dbContext.SaveChangesAsync();
//         return Results.Created($"/createcliente/{newFornecedor.Id}", newFornecedor);
//     }
// );

// //listar clientes 
// app.MapGet("/fornecedores", async (LojaDbContext dbContext) => 
//     {
//         var fornecedor = await dbContext.Fornecedores.ToListAsync();
//         return Results.Ok(fornecedor);
//     });


//buscar fornecedor por Id
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

//endpoint para Atualizar fornecedor
app.MapPut("/fornecedores/{id}", async (int id, LojaDbContext dbContext,Fornecedor updateFornecedor) =>
    {
        //Verifica se o fornecedor existe na base, com base no ID
        //Se o fornecedor existir na base, sera retornado para dentro do objeto existingProduto
        var existingFornecedor = await dbContext.Fornecedores.FindAsync(id);
        if(existingFornecedor == null)
        {
            return Results.NotFound($"Cliente with ID {id} not found.");
        }
        //atualiza os dados do existingFornecedor
        existingFornecedor.Nome = updateFornecedor.Nome;
        existingFornecedor.Endereco = updateFornecedor.Endereco;
        existingFornecedor.Email = updateFornecedor.Email;
        existingFornecedor.Telefone = updateFornecedor.Telefone;

        //salva no banco
        await dbContext.SaveChangesAsync();

        //retorna para o fornecedor que invocou o endpoint
        return Results.Ok(existingFornecedor);
    }
);

// Deletar fornecedor
app.MapDelete("/fornecedores/{id}", async (int id, LojaDbContext dbContext)=>
{
    var fornecedor = await dbContext.Fornecedores.FindAsync(id);
    if(fornecedor == null)
    {
        return Results.NotFound();
    }

    dbContext.Fornecedores.Remove(fornecedor);

    return Results.NoContent();
});



app.Run();


