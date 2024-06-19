using Microsoft.EntityFrameworkCore;
using Loja.Data;
using Loja.Models;

namespace Loja.Services
{
	public class UserService
	{
		private readonly LojaDbContext _dbContext;

		public UserService(LojaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Método para consultar todos os Usuarios
		public async Task<List<Usuario>> GetAllUsersAsync()
		{
			return await _dbContext.Usuarios.ToListAsync();
		}

		// Método para consultar um Usuario a partir do seu Id
		public async Task<Usuario> GetUserByIdAsync(int id)
		{
			return await _dbContext.Usuarios.FindAsync(id);
		}

		// Método para consultar um Usuário a partir do seu Login
		public async Task<Usuario> GetUserByLoginAsync(string login)
		{
			return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Login == login);
		}


		// Método para gravar um novo Usuario
		public async Task AddUserAsync(Usuario usuario)
		{
			usuario.Senha = HashPassword(usuario.Senha);
			_dbContext.Usuarios.Add(usuario);
			await _dbContext.SaveChangesAsync();
		}

		// Método para atualizar os dados de um Usuario
		public async Task UpdateUserAsync(int id, Usuario usuario)
		{
			var existingUsuario = await _dbContext.Usuarios.FindAsync(id);
			if (existingUsuario != null)
			{
				existingUsuario.Login = usuario.Login;
				existingUsuario.Email = usuario.Email;
				if (!string.IsNullOrEmpty(usuario.Senha))
				{
					existingUsuario.Senha = HashPassword(usuario.Senha);
				}

				await _dbContext.SaveChangesAsync();
			}
		}

		// Método para excluir um Usuario
		public async Task DeleteUserAsync(int id)
		{
			var usuario = await _dbContext.Usuarios.FindAsync(id);
			if (usuario != null)
			{
				_dbContext.Usuarios.Remove(usuario);
				await _dbContext.SaveChangesAsync();
			}
		}

		// Método para validar uma senha
		public bool ValidatePassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}

		// Método para hashear uma senha
		private string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
	}
}
