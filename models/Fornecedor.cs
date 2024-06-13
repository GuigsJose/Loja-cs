using System.ComponentModel.DataAnnotations;

namespace loja.models
{
    public class Fornecedor
    {  
        //para criar uma chave primaria, basta utilizar [Key]
        [Key]
        public int Id{get;set;}
        public String? Nome{get;set;}
        public String? Endereco{get;set;}
        public String? Email{get;set;}
        public String? Telefone{get;set;}

    }
}