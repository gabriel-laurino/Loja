using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        
        public string? Fornecedor { get; set; }
    }
}
