using System.ComponentModel.DataAnnotations.Schema;

namespace Ideia.IO.Models
{
    public class ImagemProjeto
    {
        public int Id { get; set; }
        public byte[]? Imagem { get; set; }
        public int NrOrdem { get; set; }
        public int IdProjeto { get; set; }
    }
}
