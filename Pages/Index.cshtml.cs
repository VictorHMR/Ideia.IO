using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;

namespace Ideia.IO.Pages
{
    public class IndexModel : PageModel
    {
        public readonly DbContext _db;

        public List<Projeto>? LstProjetos { get; set; }
        public int TotalProj { get; set; }
        public int ProjConcluidos { get; set; }
        public double TotalArrec { get; set; }

        public IndexModel(DbContext database)
        {
            _db = database;
        }

        public void OnGet()
        {
            LstProjetos = _db.Projeto.Where(x=>x.Ativo).OrderByDescending(x=>x.Visitas).Take(4).ToList();
            foreach(var projeto in LstProjetos)
            {
                double Arrecadado = _db.Transacao.Where(x => x.IdProjeto == projeto.Id).Sum(x => x.Valor);

                projeto.ImgAutor = _db.Usuario.Find(projeto.IdUsuAutor)?.ImgPerfil;
                projeto.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == projeto.Id && y.NrOrdem == 1)?.Imagem;
                projeto.Descricao= projeto.Descricao.Length > 80 ? projeto.Descricao.Substring(0, 80) + "..." : projeto.Descricao;
                projeto.Autor = _db.Usuario.Find(projeto.IdUsuAutor)?.NomeCompleto ?? "";
                projeto.PorcentagemConcluida = (int)Math.Round((Arrecadado / projeto.Meta ?? 0) * 100);
            }
            TotalProj = _db.Projeto.Count();
            ProjConcluidos = _db.Projeto.Where(x=> x.Meta <= _db.Transacao.Where(y=> y.IdProjeto == x.Id).Sum(z=> z.Valor)).Count();
            TotalArrec = _db.Transacao.Where(x=> x.IdProjeto != null).Sum(x=>x.Valor);
        }
    }
}
