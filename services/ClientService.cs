using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.data;
using loja.models;

namespace loja.services
{
    public class ClientService
    {
        private readonly LojaDbContext _dbContext;

        public ClientService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //consultar todos
        public async Task<List<Cliente>> GetAllClientsAsync()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        //consultar por id
        public async Task<Cliente> GetClientByIdAsync(int id)
        {
            return await _dbContext.Clientes.FindAsync(id);
        }

        //gravar cliente
        public async Task AddClientAsync(Cliente cliente)
        {
            _dbContext.Clientes.Add(cliente);
            await _dbContext.SaveChangesAsync();
        }

        //atualizar os dados
        public async Task UpdateClientAsync(Cliente cliente)
        {
            _dbContext.Entry(cliente).State=EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        //deletar
        public async Task DeleteClientAsync(int id)
        {
            var cliente = await _dbContext.Clientes.FindAsync(id);
            if(cliente != null)
            {
                _dbContext.Clientes.Remove(cliente);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}