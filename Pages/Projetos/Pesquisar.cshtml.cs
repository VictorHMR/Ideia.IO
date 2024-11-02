using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ideia.IO.Pages.Projetos
{
    public class PesquisarModel : PageModel
    {
        public readonly DbContext _db;

        public List<Projeto>? LstProjetos { get; set; }
        public int TotalProj { get; set; }
        public int TotalPaginas { get; set; }

        [BindProperty(SupportsGet = true)]
        public PesquisaProjeto Pesquisa { get; set; }

        public PesquisarModel(DbContext database)
        {
            _db = database;
        }

        public void OnGet()
        {
            ListarProjetos();
        }

        public IActionResult OnGetPesquisar()
        {
            ListarProjetos();
            return Page();
        }

        public void ListarProjetos()
        {
            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? _db.Projeto.Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : _db.Projeto.Where(x => x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();



            LstProjetos.ForEach(x => x.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == x.Id && y.NrOrdem == 1)?.Imagem);

            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa) 
                ? _db.Projeto.Count() 
                : _db.Projeto.Where(x => x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)).Count(); 
            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);
        }
    }
}
