using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ideia.IO.Pages.Projetos
{

    public class MeusProjetosModel : PageModel
    {
        public readonly DbContext _db;

        public List<Projeto>? LstProjetos { get; set; }

        public MeusProjetosModel(DbContext database)
        {
            _db = database;
        }
        public void OnGet()
        {
            int? idUsu = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            LstProjetos = _db.Projeto.Where(x=> x.IdUsuAutor == idUsu).ToList();
        }

        public IActionResult OnPostDelete(int IdProjeto)
        {
            _db.ImagemProjeto.RemoveRange(_db.ImagemProjeto.Where(x => x.IdProjeto == IdProjeto).ToList());
            _db.Projeto.Remove(_db.Projeto.Find(IdProjeto));
            _db.SaveChanges();

            return RedirectToPage();
        }
    }
}
