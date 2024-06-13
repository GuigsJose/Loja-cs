using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class SupplierService
    {
        private readonly LojaDbContext _dbContext;
        public SupplierService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //consulta
        public async Task<List<Fornecedor>> GetAllSuppliersAsync()
        {
            return await _dbContext.Fornecedores.ToListAsync();
        }  

        public async Task<Fornecedor> GetSupplierByIdAsync(int id)
        {
            return await _dbContext.Fornecedores.FindAsync(id);
        }  

        public async Task AddSupplierAsync(Fornecedor fornecedor)
        {
            _dbContext.Fornecedores.Add(fornecedor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(Fornecedor fornecedor)
        {
            _dbContext.Entry(fornecedor).State=EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var fornecedor = await _dbContext.Fornecedores.FindAsync(id);
            if(fornecedor != null)
            {
                _dbContext.Fornecedores.Remove(fornecedor);
                await _dbContext.SaveChangesAsync();
            }
        }
    
    }
}