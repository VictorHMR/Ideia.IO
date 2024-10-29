using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ideia.IO.Pages.Projetos
{
    public class ProjetosModel : PageModel
    {
        public readonly Database _db;

        public Projeto? Projeto { get; set; }

        public ProjetosModel(Database database)
        {
            _db = database;
        }
        public IActionResult OnGet(int? IdProjeto)
        {
            if (IdProjeto is null)
                ViewData["Action"] = "Create";
            else
            {
                Projeto = _db.Projeto.Find(IdProjeto);
                if (Projeto is not null)
                    ViewData["Action"] = "Edit";
                else
                    return Redirect("/index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? IdProjeto, [FromForm] Projeto? projeto)
        {
            if(IdProjeto is null)
            {
                if (projeto is not null)
                {
                    projeto.DtCriacao = DateTime.Now;
                    projeto.IdUsuAutor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _db.Projeto.Add(projeto);
                    _db.SaveChanges();
                }
            }
            else 
            {
                Projeto ProjetoDB = _db.Projeto.Find(IdProjeto);
                if (ProjetoDB is not null)
                {
                    ProjetoDB.Titulo = projeto.Titulo;
                    ProjetoDB.Descricao = projeto.Descricao;
                    ProjetoDB.Meta = projeto.Meta;
                    _db.SaveChanges();

                }
            }
            return Redirect("/Projetos/MeusProjetos");
        }
    }
}
