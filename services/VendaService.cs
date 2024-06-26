using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loja.services
{
    public class VendaService
    {
        private readonly LojaDbContext _context;

        public VendaService(LojaDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> AddVendaAsync(Venda venda)
        {
            var cliente = await _context.Clientes.FindAsync(venda.ClienteId);
            var produto = await _context.Produtos.FindAsync(venda.ProdutoId);

            if (cliente == null || produto == null)
            {
                throw new Exception("Cliente ou Produto não encontrado.");
            }

            venda.Cliente = cliente;
            venda.Produto = produto;
            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            return venda;
        }

        public async Task<IEnumerable<dynamic>> GetVendasPorProdutoDetalhadaAsync(int produtoId)
        {
            return await _context.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .Select(v => new
                {
                    ProdutoNome = v.Produto.Nome,
                    v.DataVenda,
                    v.Id,
                    ClienteNome = v.Cliente.Nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                }).ToListAsync();
        }


        public async Task<dynamic> GetVendasPorProdutoSumarizadaAsync(int produtoId)
        {
            var vendas = await _context.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => v.Produto.Nome)
                .Select(g => new
                {
                    ProdutoNome = g.Key,
                    QuantidadeTotal = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                }).FirstOrDefaultAsync();

            return vendas;
        }

        public async Task<IEnumerable<dynamic>> GetVendasPorClienteDetalhadaAsync(int clienteId)
        {
            return await _context.Vendas
                .Where(v => v.ClienteId == clienteId)
                .Select(v => new
                {
                    v.Produto.Nome,
                    v.DataVenda,
                    v.Id,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                }).ToListAsync();
        }

        public async Task<dynamic> GetVendasPorClienteSumarizadaAsync(int clienteId)
        {
            var vendas = await _context.Vendas
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.Cliente.Nome)
                .Select(g => new
                {
                    ClienteNome = g.Key,
                    QuantidadeTotal = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotal = g.Sum(v => v.PrecoUnitario * v.QuantidadeVendida)
                }).FirstOrDefaultAsync();

            return vendas;
        }
    }
}
