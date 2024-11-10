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
            CarregarListaOrdenada();

            switch (Pesquisa.Local)
            {
                case LocalPesquisa.Pesquisa:
                    ListarProjetos();
                    break;
                case LocalPesquisa.MinhasContribuicoes:
                    ListarProjetosContribuidos();
                    break;
                case LocalPesquisa.MeusProjetos:
                    ListarMeusProjetos();
                    break;

            }
        }

        public void ListarProjetos()
        {

            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa) 
                ? LstProjetos.Where(x => x.Ativo).Count() 
                : LstProjetos.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo).Count(); 
            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);

            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? LstProjetos?.Where(x=> x.Ativo).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : LstProjetos?.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();

            CarregarDadosAdicionais();
        }

        public void ListarProjetosContribuidos()
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<int?> ProjetosContribuidos = _db.Transacao.Where(x => x.IdUsuario == idUsuario && x.IdProjeto != null).Select(x => x.IdProjeto).Distinct().ToList();
            
            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? LstProjetos.Where(x => ProjetosContribuidos.Contains(x.Id)).Count()
                : LstProjetos.Where(x => ProjetosContribuidos.Contains(x.Id) && (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa))).Count();

            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);
            TotalContribuido = _db.Transacao.Where(x => x.IdUsuario == idUsuario && x.IdProjeto != null).Sum(y => y.Valor);

            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? LstProjetos?.Where(x => ProjetosContribuidos.Contains(x.Id)).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : LstProjetos?.Where(x => ProjetosContribuidos.Contains(x.Id) && (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa))).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();

            CarregarDadosAdicionais();


            LstProjetos.ForEach(x => x.Contribuido = _db.Transacao.Where(y=> y.IdProjeto == x.Id && y.IdUsuario == idUsuario).Sum(z=> z.Valor));

        }

        public void ListarMeusProjetos()
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            TotalProj = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? LstProjetos.Where(x => x.Ativo && x.IdUsuAutor == idUsuario).Count()
                : LstProjetos.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo).Count();
            TotalPaginas = (int)Math.Ceiling(TotalProj / (double)Pesquisa.ItemsPorPagina);

            LstProjetos = string.IsNullOrEmpty(Pesquisa.Pesquisa)
                ? LstProjetos?.Where(x => x.Ativo && x.IdUsuAutor == idUsuario).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList()
                : LstProjetos?.Where(x => (x.Titulo.Contains(Pesquisa.Pesquisa) || x.Descricao.Contains(Pesquisa.Pesquisa)) && x.Ativo && x.IdUsuAutor == idUsuario).Skip((Pesquisa.Pagina - 1) * Pesquisa.ItemsPorPagina).Take(Pesquisa.ItemsPorPagina).ToList();

            CarregarDadosAdicionais();
        }


        internal void CarregarListaOrdenada()
        {
            switch (Pesquisa.Ordenacao)
            {
                case OrdenacaoPesquisa.MaisNovos:
                    LstProjetos = _db.Projeto.OrderByDescending(x => x.DtCriacao).ToList();
                    break;
                case OrdenacaoPesquisa.MaisAntigos:
                    LstProjetos = _db.Projeto.OrderBy(x => x.DtCriacao).ToList();
                    break;
                default:
                    LstProjetos = _db.Projeto.OrderByDescending(x => x.Visitas).ToList();
                    break;

            }
        }

        internal void CarregarDadosAdicionais()
        {
            foreach (var projeto in LstProjetos)
            {
                double Arrecadado = _db.Transacao.Where(x => x.IdProjeto == projeto.Id).Sum(x => x.Valor);

                projeto.ImgAutor = _db.Usuario.Find(projeto.IdUsuAutor)?.ImgPerfil;
                projeto.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == projeto.Id && y.NrOrdem == 1)?.Imagem;
                projeto.Descricao = projeto.Descricao.Length > 100 ? projeto.Descricao.Substring(0, 100) + "..." : projeto.Descricao;
                projeto.Autor = _db.Usuario.Find(projeto.IdUsuAutor)?.NomeUsuario ?? "";
                projeto.PorcentagemConcluida = (int)Math.Round((Arrecadado / projeto.Meta ?? 0) * 100);
            }
        }

    }
}
