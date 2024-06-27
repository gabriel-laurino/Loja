using Loja.Data;
using Loja.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loja.Services
{
    public class ContratoService
    {
        private readonly LojaDbContext _dbContext;

        public ContratoService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Contrato>> GetContratosByClienteIdAsync(int clienteId)
        {
            return await _dbContext.Contratos
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task AddContratoAsync(Contrato contrato)
        {
            if (!await ClienteExistsAsync(contrato.ClienteId))
            {
                throw new KeyNotFoundException("Cliente não encontrado");
            }

            if (!await ServicoExistsAsync(contrato.ServicoId))
            {
                throw new KeyNotFoundException("Serviço não encontrado");
            }

            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> ClienteExistsAsync(int clienteId)
        {
            return await _dbContext.Clientes.AnyAsync(c => c.Id == clienteId);
        }

        private async Task<bool> ServicoExistsAsync(int servicoId)
        {
            return await _dbContext.Servicos.AnyAsync(s => s.Id == servicoId);
        }
    }
}
