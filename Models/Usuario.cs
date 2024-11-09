using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ideia.IO.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string? NomeUsuario { get; set; }
        [Required]
        public string? NomeCompleto { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Senha { get; set; }

        public string? Telefone { get; set; }
        public string? ChavePix { get; set; }

        public byte[]? ImgPerfil { get; set; }

        [NotMapped]
        public IFormFile? ImgPerfilFile { get; set; }  // Propriedade temporária para upload de arquivo
        [NotMapped]
        public string? ConfirmarSenha { get; set; }
        [NotMapped]
        public string? NovaSenha { get; set; }

    }
}
