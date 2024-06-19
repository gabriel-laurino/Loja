using Microsoft.EntityFrameworkCore;
using Loja.Data;
using Loja.Models;

namespace Loja.Services
{
	public class ClientService
	{
		private readonly LojaDbContext _dbContext;

		public ClientService(LojaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Método para consultar todos os Cliente
		public async Task<List<Cliente>> GetAllClientsAsync()
		{
			return await _dbContext.Clientes.ToListAsync();
		}

		// Método para consultar um Cliente a partir do seu Id
		public async Task<Cliente> GetClientByIdAsync(int id)
		{
			return await _dbContext.Clientes.FindAsync(id);
		}

		// Método para  gravar um novo Cliente
		public async Task AddClientAsync(Cliente Cliente)
		{
			_dbContext.Clientes.Add(Cliente);
			await _dbContext.SaveChangesAsync();
		}

		// Método para atualizar os dados de um Cliente
		public async Task UpdateClientAsync(int id, Cliente Cliente)
		{
			var existingCliente = await _dbContext.Clientes.FindAsync(id);
			if (existingCliente != null)
			{
				existingCliente.Nome = Cliente.Nome;
				existingCliente.Email = Cliente.Email;
				existingCliente.Telefone = Cliente.Telefone;

				await _dbContext.SaveChangesAsync();
			}
		}

		// Método para excluir um Cliente
		public async Task DeleteClientAsync(int id)
		{
			var Cliente = await _dbContext.Clientes.FindAsync(id);
			if (Cliente != null)
			{
				_dbContext.Clientes.Remove(Cliente);
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}
