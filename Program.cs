using Microsoft.EntityFrameworkCore;
using Loja.Models;
using Loja.Services;
using Loja.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar a conexão com o BD
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))));

// Adicionar serviços ao contêiner.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SellService>();
builder.Services.AddAuthorization();

string secretKey = "dfhviocsjserkvknkjsdajvbejnvjfjsdf";

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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

string GenerateToken(string email)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, email)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256Signature
        )
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.MapPost("/login", async (HttpContext context, UserService userService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    var json = JsonDocument.Parse(body);
    var login = json.RootElement.GetProperty("login").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    var usuario = await userService.GetUserByLoginAsync(login);

    if (usuario != null && userService.ValidatePassword(senha, usuario.Senha))
    {
        var token = GenerateToken(login);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { token }));
        return;
    }

    context.Response.StatusCode = 401; // Unauthorized
    await context.Response.WriteAsync("\nUsuário ou senha inválidos");
});

// Endpoints de Vendas
app.MapPost("/vendas", async (Venda venda, SellService sellService) =>
{
    try
    {
        var novaVenda = await sellService.AddVendaAsync(venda);
        return Results.Created($"/vendas/{novaVenda.Id}", novaVenda);
    }
    catch (ArgumentException e)
    {
        return Results.BadRequest(e.Message);
    }
}).RequireAuthorization();

app.MapGet("/vendas/produto/detalhada/{produtoId}", async (int produtoId, SellService sellService) =>
{
    var vendas = await sellService.GetVendasByProdutoDetalhadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/produto/sumarizada/{produtoId}", async (int produtoId, SellService sellService) =>
{
    var vendas = await sellService.GetVendasByProdutoSumarizadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/detalhada/{clienteId}", async (int clienteId, SellService sellService) =>
{
    var vendas = await sellService.GetVendasByClienteDetalhadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/sumarizada/{clienteId}", async (int clienteId, SellService sellService) =>
{
    var vendas = await sellService.GetVendasByClienteSumarizadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();


app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
}).RequireAuthorization();

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"\nProduto com ID {id} não encontrado.");
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
    await productService.UpdateProductAsync(id, produto);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProductAsync(id);
    return Results.Ok();
}).RequireAuthorization();

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
        return Results.NotFound($"\nCliente com ID {id} não encontrado.");
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
    await clientService.UpdateClientAsync(id, cliente);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/clientes/{id}", async (int id, ClientService clientService) =>
{
    await clientService.DeleteClientAsync(id);
    return Results.Ok();
}).RequireAuthorization();

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
        return Results.NotFound($"\nFornecedor com ID {id} não encontrado.");
    }
    return Results.Ok(fornecedor);
}).RequireAuthorization();

app.MapPost("/fornecedores", async (Fornecedor fornecedor, SupplierService supplierService) =>
{
    await supplierService.AddSupplierAsync(fornecedor);
    return Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor);
}).RequireAuthorization();

app.MapPut("/fornecedores/{id}", async (int id, Fornecedor fornecedor, SupplierService supplierService) =>
{
    await supplierService.UpdateSupplierAsync(id, fornecedor);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/fornecedores/{id}", async (int id, SupplierService supplierService) =>
{
    await supplierService.DeleteSupplierAsync(id);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/usuarios", async (UserService userService) =>
{
    var usuarios = await userService.GetAllUsersAsync();
    return Results.Ok(usuarios);
}).RequireAuthorization();

app.MapGet("/usuarios/{id}", async (int id, UserService userService) =>
{
    var usuario = await userService.GetUserByIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound($"\nUsuário com ID {id} não encontrado.");
    }
    return Results.Ok(usuario);
}).RequireAuthorization();

app.MapPost("/usuarios", async (Usuario usuario, UserService userService) =>
{
    await userService.AddUserAsync(usuario);
    return Results.Created($"/usuario/{usuario.Id}", usuario);
}).RequireAuthorization();

app.MapPut("/usuarios/{id}", async (int id, Usuario usuario, UserService userService) =>
{
    await userService.UpdateUserAsync(id, usuario);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/usuarios/{id}", async (int id, UserService userService) =>
{
    await userService.DeleteUserAsync(id);
    return Results.Ok();
}).RequireAuthorization();

app.Run();