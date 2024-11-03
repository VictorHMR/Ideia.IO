using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            LstProjetos = _db.Projeto.Where(x=>x.Ativo).Take(5).ToList();
            LstProjetos.ForEach(x => x.CapaProj = _db.ImagemProjeto.FirstOrDefault(y => y.IdProjeto == x.Id && y.NrOrdem == 1)?.Imagem);
            TotalProj = _db.Projeto.Count();
            ProjConcluidos = _db.Projeto.Where(x=> x.Meta <= _db.Transacao.Where(y=> y.IdProjeto == x.Id).Sum(z=> z.Valor)).Count();
            TotalArrec = _db.Transacao.Where(x=> x.IdProjeto != null).Sum(x=>x.Valor);
        }
    }
}
