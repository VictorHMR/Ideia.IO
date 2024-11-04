using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ideia.IO.Pages.Projetos
{
    public class PesquisarModel : PageModel
    {
        public readonly DbContext _db;

        public List<Projeto>? LstProjetos { get; set; }
        public int TotalProj { get; set; }
        public int TotalPaginas { get; set; }
        public double? TotalContribuido { get; set; }

        [BindProperty(SupportsGet = true)]
        public PesquisaProjeto Pesquisa { get; set; }

        public PesquisarModel(DbContext database)
        {
            _db = database;
        }

        public void OnGet()
        {
            if (Pesquisa.MinhasContribuicoes)
                ListarProjetosContribuidos();
            else
                ListarProjetos();
        }

        public IActionResult OnGetPesquisar()
        {
            if (Pesquisa.MinhasContribuicoes)
                ListarProjetosContribuidos();
            else
                ListarProjetos();

            return Page();
        }

        public void ListarProjetos()
        {
            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? _db.Projeto.Where(x=> x.Ativo).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : _db.Projeto.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();

            foreach (var projeto in LstProjetos)
            {
                double Arrecadado = _db.Transacao.Where(x => x.IdProjeto == projeto.Id).Sum(x => x.Valor);

                projeto.ImgAutor = _db.Usuario.Find(projeto.IdUsuAutor)?.ImgPerfil;
                projeto.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == projeto.Id && y.NrOrdem == 1)?.Imagem;
                projeto.Descricao = projeto.Descricao.Length > 100 ? projeto.Descricao.Substring(0, 100) + "..." : projeto.Descricao;
                projeto.Autor = _db.Usuario.Find(projeto.IdUsuAutor)?.NomeCompleto ?? "";
                projeto.PorcentagemConcluida = (int)Math.Round((Arrecadado / projeto.Meta ?? 0) * 100);
            }
            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa) 
                ? _db.Projeto.Where(x => x.Ativo).Count() 
                : _db.Projeto.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo).Count(); 
            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);
        }

        public void ListarProjetosContribuidos()
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<int?> ProjetosContribuidos = _db.Transacao.Where(x => x.IdUsuario == idUsuario && x.IdProjeto != null).Select(x => x.IdProjeto).Distinct().ToList();

            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? _db.Projeto.Where(x => ProjetosContribuidos.Contains(x.Id)).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : _db.Projeto.Where(x => ProjetosContribuidos.Contains(x.Id) && (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa))).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();


            foreach (var projeto in LstProjetos)
            {
                double Arrecadado = _db.Transacao.Where(x => x.IdProjeto == projeto.Id).Sum(x => x.Valor);

                projeto.ImgAutor = _db.Usuario.Find(projeto.IdUsuAutor)?.ImgPerfil;
                projeto.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == projeto.Id && y.NrOrdem == 1)?.Imagem;
                projeto.Descricao = projeto.Descricao.Length > 100 ? projeto.Descricao.Substring(0, 100) + "..." : projeto.Descricao;
                projeto.Autor = _db.Usuario.Find(projeto.IdUsuAutor)?.NomeCompleto ?? "";
                projeto.PorcentagemConcluida = (int)Math.Round((Arrecadado / projeto.Meta ?? 0) * 100);
            }
            LstProjetos.ForEach(x => x.Contribuido = _db.Transacao.Where(y=> y.IdProjeto == x.Id && y.IdUsuario == idUsuario).Sum(z=> z.Valor));

            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? _db.Projeto.Where(x => ProjetosContribuidos.Contains(x.Id)).Count()
                : _db.Projeto.Where(x => ProjetosContribuidos.Contains(x.Id) && (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa))).Count();

            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);
            TotalContribuido = _db.Transacao.Where(x => x.IdUsuario == idUsuario && x.IdProjeto != null).Sum(y => y.Valor);
        }

    }
}
