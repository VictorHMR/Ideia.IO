namespace Ideia.IO.Models
{
    public class PesquisaProjeto
    {
        public string? Pesquisa { get; set; }
        public int Pagina { get; set; } = 1;
        public int ItemsPorPagina { get; set; } = 2;
    }
}
