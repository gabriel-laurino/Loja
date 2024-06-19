using Microsoft.EntityFrameworkCore;
using Loja.Data;
using Loja.Models;

namespace Loja.Services
{
	public class SupplierService
	{
		private readonly LojaDbContext _dbContext;

		public SupplierService(LojaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Método para consultar todos os Fornecedor
		public async Task<List<Fornecedor>> GetAllSuppliersAsync()
		{
			return await _dbContext.Fornecedores.ToListAsync();
		}

		// Método para consultar um Fornecedor a partir do seu Id
		public async Task<Fornecedor> GetSupplierByIdAsync(int id)
		{
			return await _dbContext.Fornecedores.FindAsync(id);
		}

		// Método para  gravar um novo Fornecedor
		public async Task AddSupplierAsync(Fornecedor Fornecedor)
		{
			_dbContext.Fornecedores.Add(Fornecedor);
			await _dbContext.SaveChangesAsync();
		}

		// Método para atualizar os dados de um Fornecedor
		public async Task UpdateSupplierAsync(int id, Fornecedor Fornecedor)
		{
			var existingFornecedor = await _dbContext.Fornecedores.FindAsync(id);
			if (existingFornecedor != null)
			{
				existingFornecedor.Cnpj = Fornecedor.Cnpj;
				existingFornecedor.Nome = Fornecedor.Nome;
				existingFornecedor.Endereco = Fornecedor.Endereco;
				existingFornecedor.Email = Fornecedor.Email;
				existingFornecedor.Telefone = Fornecedor.Telefone;

				await _dbContext.SaveChangesAsync();
			}
		}

		// Método para excluir um Fornecedor
		public async Task DeleteSupplierAsync(int id)
		{
			var Fornecedor = await _dbContext.Fornecedores.FindAsync(id);
			if (Fornecedor != null)
			{
				_dbContext.Fornecedores.Remove(Fornecedor);
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}
