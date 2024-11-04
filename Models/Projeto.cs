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
        public bool Ativo { get; set; } = true;
        public int Visitas { get; set; } = 0;

        [NotMapped]
        public byte[]? CapaProj { get; set; }
        [NotMapped]
        public byte[]? ImgAutor { get; set; }
        [NotMapped]
        public double? Contribuido{ get; set; }
        [NotMapped]
        public int? PorcentagemConcluida { get; set; }
        [NotMapped]
        public string Autor { get; set; }

    }
}
