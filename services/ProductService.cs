using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class ProductService
    {
        private readonly LojaDbContext _dbContext;

        public ProductService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //consultar todos os produtos
        public async Task<List<Produto>> GetAllProductsAsync()
        {
            return await _dbContext.Produtos.ToListAsync();
        }

        //consultar produto pelo Id
        public async Task<Produto> GetProductsByIdAsync(int Id)
        {
            return await _dbContext.Produtos.FindAsync(Id);
        }

        //salvar novo produto
        public async Task AddProductAsync(Produto produto)
        {
            _dbContext.Produtos.Add(produto);
            await _dbContext.SaveChangesAsync();
        }

        //atualizar dados do produto
        public async Task UpdateProductAsync(Produto produto)
        {
            _dbContext.Entry(produto).State=EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        //excluir produto
        public async Task DeleteProdctAsync(int id)
        {
            var produto = await _dbContext.Produtos.FindAsync(id);
            if(produto != null)
            {
                _dbContext.Produtos.Remove(produto);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}