using Microsoft.EntityFrameworkCore;
using Loja.Data;
using Loja.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexão com o BD
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))));

var app = builder.Build();

app.UseHttpsRedirection();

// Endpoint para criar um novo produto
app.MapPost("/createproduto", async (LojaDbContext dbContext, Produto newProduto) =>
{
    dbContext.Produtos.Add(newProduto);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createproduto/{newProduto.Id}", newProduto);
});

// Endpoint para listar todos os produtos
app.MapGet("/produtos", async (LojaDbContext dbContext) =>
{
    var produtos = await dbContext.Produtos.ToListAsync();
    return Results.Ok(produtos);
});

// Endpoint para buscar um produto por ID
app.MapGet("/produtos/{id}", async (int id, LojaDbContext dbContext) =>
{
    var produto = await dbContext.Produtos.FindAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Produto com ID {id} não encontrado.");
    }
    return Results.Ok(produto);
});

// Endpoint para atualizar um produto existente
app.MapPut("/produtos/{id}", async (int id, LojaDbContext dbContext, Produto updatedProduto) =>
{
    var existingProduto = await dbContext.Produtos.FindAsync(id);
    if (existingProduto == null)
    {
        return Results.NotFound($"Produto com ID {id} não encontrado.");
    }

    existingProduto.Nome = updatedProduto.Nome;
    existingProduto.Descricao = updatedProduto.Descricao;
    existingProduto.Preco = updatedProduto.Preco;
    existingProduto.Fornecedor = updatedProduto.Fornecedor;

    await dbContext.SaveChangesAsync();
    return Results.Ok(existingProduto);
});

app.MapPost("/createcliente", async (LojaDbContext dbContext, Cliente newCliente) =>
{
    dbContext.Clientes.Add(newCliente);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createcliente/{newCliente.Id}", newCliente);
});

// Endpoint para listar todos os clientes
app.MapGet("/clientes", async (LojaDbContext dbContext) =>
{
    var clientes = await dbContext.Clientes.ToListAsync();
    return Results.Ok(clientes);
});

// Endpoint para buscar um cliente por ID
app.MapGet("/clientes/{id}", async (int id, LojaDbContext dbContext) =>
{
    var cliente = await dbContext.Clientes.FindAsync(id);
    if (cliente == null)
    {
        return Results.NotFound($"Cliente com ID {id} não encontrado.");
    }
    return Results.Ok(cliente);
});

// Endpoint para atualizar um cliente existente
app.MapPut("/clientes/{id}", async (int id, LojaDbContext dbContext, Cliente updatedCliente) =>
{
    var existingCliente = await dbContext.Clientes.FindAsync(id);
    if (existingCliente == null)
    {
        return Results.NotFound($"Cliente com ID {id} não encontrado.");
    }

    existingCliente.Nome = updatedCliente.Nome;
    existingCliente.Email = updatedCliente.Email;
    existingCliente.Telefone = updatedCliente.Telefone;

    await dbContext.SaveChangesAsync();
    return Results.Ok(existingCliente);
});

// Endpoint para criar um novo fornecedor
app.MapPost("/createfornecedor", async (LojaDbContext dbContext, Fornecedor newFornecedor) =>
{
    dbContext.Fornecedores.Add(newFornecedor);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createfornecedor/{newFornecedor.Id}", newFornecedor);
});

// Endpoint para listar todos os fornecedores
app.MapGet("/fornecedores", async (LojaDbContext dbContext) =>
{
    var fornecedores = await dbContext.Fornecedores.ToListAsync();
    return Results.Ok(fornecedores);
});

// Endpoint para buscar um fornecedor por ID
app.MapGet("/fornecedores/{id}", async (int id, LojaDbContext dbContext) =>
{
    var fornecedor = await dbContext.Fornecedores.FindAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor com ID {id} não encontrado.");
    }
    return Results.Ok(fornecedor);
});

// Endpoint para atualizar um fornecedor existente
app.MapPut("/fornecedores/{id}", async (int id, LojaDbContext dbContext, Fornecedor updatedFornecedor) =>
{
    var existingFornecedor = await dbContext.Fornecedores.FindAsync(id);
    if (existingFornecedor == null)
    {
        return Results.NotFound($"Fornecedor com ID {id} não encontrado.");
    }

    existingFornecedor.Cnpj = updatedFornecedor.Cnpj;
    existingFornecedor.Nome = updatedFornecedor.Nome;
    existingFornecedor.Endereco = updatedFornecedor.Endereco;
    existingFornecedor.Email = updatedFornecedor.Email;
    existingFornecedor.Telefone = updatedFornecedor.Telefone;

    await dbContext.SaveChangesAsync();
    return Results.Ok(existingFornecedor);
});

app.Run();