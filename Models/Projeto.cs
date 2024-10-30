using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ideia.IO.Models
{
    public class Projeto
    {
        public int Id { get; set; }
        [Required]
        public string? Titulo{ get; set; }
        [Required]
        public string? Descricao{ get; set; }
        [Required]
        public double? Meta{ get; set; }
        [Required]
        public DateTime? DtCriacao{ get; set; }
        public int IdUsuAutor { get; set; }
        

    }
}
