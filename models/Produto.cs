using System.ComponentModel.DataAnnotations;

namespace loja.models
{
    public class Produto
    {
        //para criar uma chave primaria, basta utilizar [Key]
        [Key]
        public int Id{get;set;}
        public String Nome{get;set;}
        public Double Preco{get;set;}
        public int FornecedorId{get;set;}
        public Fornecedor Fornecedor{get;set;}
        
    }
}