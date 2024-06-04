using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace loja.models
{
    public class Produto
    {
        //para criar uma chave primaria, basta utilizar [Key]
        [Key]
        public int Id{get;set;}
        public String Nome{get;set;}
        public Double Preco{get;set;}
        [ForeignKey("Fornecedor")]
        public int FornecedorId{get;set;}
        public Fornecedor Fornecedor{get;set;}
        
    }
}