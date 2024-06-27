    using System.ComponentModel.DataAnnotations.Schema;
    
    public class Contrato
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ServicoId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCobrado { get; set; }
        public DateTime DataContratacao { get; set; }
    }