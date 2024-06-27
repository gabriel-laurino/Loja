using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Models
{
    public class Servico
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        public bool Status { get; set; }
    }
}
