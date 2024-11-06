using System.ComponentModel.DataAnnotations;

namespace Ideia.IO.Models
{
    public class PesquisaProjeto
    {
        public string? Pesquisa { get; set; }
        public int Pagina { get; set; } = 1;
        public int ItemsPorPagina { get; set; } = 20;
        public bool Salvos { get; set; } = false;
        public bool MinhasContribuicoes { get; set; } = false;
        public OrdenacaoPesquisa Ordenacao { get; set; } = OrdenacaoPesquisa.Relevancia;
    }

    public enum OrdenacaoPesquisa
    {
        [Display(Name = "Relevância")]
        Relevancia,
        [Display(Name = "Mais Antigos")]
        MaisAntigos,
        [Display(Name = "Mais Novos")]
        MaisNovos
    }
}
