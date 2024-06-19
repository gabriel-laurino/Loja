using Loja.Data;
using Loja.Models;
using Microsoft.EntityFrameworkCore;

namespace Loja.Services
{
    public class SellService
    {
        private readonly LojaDbContext _context;

        public SellService(LojaDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> AddVendaAsync(Venda venda)
        {
            // Validação se o cliente e o produto existem
            var cliente = await _context.Clientes.FindAsync(venda.ClienteId);
            var produto = await _context.Produtos.FindAsync(venda.ProdutoId);

            if (cliente == null || produto == null)
            {
                throw new ArgumentException("\nCliente ou Produto não encontrado");
            }

            venda.PrecoUnitario = produto.Preco;

            if (venda.DataVenda == default(DateTime))
            {
                venda.DataVenda = DateTime.Now;
            }

            if (string.IsNullOrEmpty(venda.NumeroNotaFiscal))
            {
                venda.NumeroNotaFiscal = Guid.NewGuid().ToString();
            }

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();
            return venda;
        }

        public async Task<IEnumerable<object>> GetVendasByProdutoDetalhadaAsync(int produtoId)
        {
            return await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ProdutoId == produtoId)
                .Select(v => new 
                {
                    v.Id,
                    v.DataVenda,
                    ProdutoNome = v.Produto.Nome,
                    ClienteNome = v.Cliente.Nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByProdutoSumarizadaAsync(int produtoId)
        {
            return await _context.Vendas
                .Include(v => v.Produto)
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => new { v.ProdutoId, v.Produto.Nome })
                .Select(g => new 
                {
                    ProdutoNome = g.Key.Nome,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByClienteDetalhadaAsync(int clienteId)
        {
            return await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ClienteId == clienteId)
                .Select(v => new 
                {
                    v.Id,
                    v.DataVenda,
                    ProdutoNome = v.Produto.Nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByClienteSumarizadaAsync(int clienteId)
        {
            return await _context.Vendas
                .Include(v => v.Cliente)
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.ClienteId)
                .Select(g => new 
                {
                    ClienteNome = g.FirstOrDefault().Cliente.Nome,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                })
                .ToListAsync();
        }
    }
}
