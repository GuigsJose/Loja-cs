using System.ComponentModel.DataAnnotations;

namespace loja.models
{
    public class Cliente
    {
        //para criar uma chave primaria, basta utilizar [Key]
        [Key]
        public int Id{get;set;}
        public String Nome{get;set;}
        public String Cpf{get;set;}
        public String Email{get;set;}
    }
}