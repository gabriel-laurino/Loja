using Loja.Data;
using Loja.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loja.Services
{
    public class ServicoService
    {
        private readonly LojaDbContext _dbContext;

        public ServicoService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Servico>> GetAllServicosAsync()
        {
            return await _dbContext.Servicos.ToListAsync();
        }

        public async Task<Servico> GetServicoByIdAsync(int id)
        {
            return await _dbContext.Servicos.FindAsync(id);
        }

        public async Task AddServicoAsync(Servico servico)
        {
            _dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateServicoAsync(int id, Servico servico)
        {
            var existingServico = await _dbContext.Servicos.FindAsync(id);
            if (existingServico != null)
            {
                existingServico.Nome = servico.Nome;
                existingServico.Preco = servico.Preco;
                existingServico.Status = servico.Status;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteServicoAsync(int id)
        {
            var servico = await _dbContext.Servicos.FindAsync(id);
            if (servico != null)
            {
                _dbContext.Servicos.Remove(servico);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
