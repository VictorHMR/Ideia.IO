using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ideia.IO.Pages.Projetos
{
    public class ProjetosModel : PageModel
    {
        public readonly DbContext _db;

        public Projeto? Projeto { get; set; }
        [BindProperty]
        public List<ImagemProjeto>? ImagemProjeto { get; set; }

        public List<Transacao> Transacoes { get; set; }

        public ProjetosModel(DbContext database)
        {
            _db = database;
        }
        public IActionResult OnGet(int IdProjeto)
        {
            int IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            Projeto = _db.Projeto.Find(IdProjeto);
            if (Projeto is not null)
            {
                Projeto.Salvo = _db.ProjetoSalvo.Any(x => x.IdProjeto == IdProjeto && x.IdUsuario == IdUsuario);
                Projeto.Visitas++;
                _db.SaveChanges();
                ImagemProjeto = _db.ImagemProjeto.Where(x => x.IdProjeto == IdProjeto).ToList();
                Transacoes = _db.Transacao.Where(x => x.IdProjeto == IdProjeto).ToList();
                return Page();
            }
            else
                return Redirect("/index");
        }

        public IActionResult OnPostContribuir(int IdProjeto, double Valor)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["Fail"] = "Você precisa estar logado para completar essa ação";
                return RedirectToPage();
            }

            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (_db.Transacao.Where(x => x.IdUsuario == idUsuario && x.IdProjeto == null).Sum(x => x.Valor) >= Valor)
            {
                Transacao Transacao = new Transacao
                {
                    Valor = Valor,
                    IdProjeto = IdProjeto,
                    IdUsuario = idUsuario,
                    DtTransacao = DateTime.Now
                };

                Transacao Transacao2 = new Transacao
                {
                    Valor = -Valor,
                    IdUsuario = idUsuario,
                    DtTransacao = DateTime.Now
                };
                _db.Transacao.AddRange([Transacao, Transacao2]);
                _db.SaveChanges();

            }
            else
                TempData["Fail"] = "Saldo insuficiente para realizar essa operação";

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int IdProjeto)
        {
            Projeto Projeto = _db.Projeto.Find(IdProjeto);
            if (Projeto is not null)
            {
                Projeto.Ativo = false;
                _db.SaveChanges();
            }
            else
            {
                TempData["Fail"] = "Projeto não existe";
            }

            return RedirectToPage("/Index");
        }

        public IActionResult OnPostSalvar(int IdProjeto, bool salvar)
        {
            int IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Projeto Projeto = _db.Projeto.Find(IdProjeto);
            if (Projeto is not null)
            {
                ProjetoSalvo? projSalvo = _db.ProjetoSalvo.FirstOrDefault(x => x.IdProjeto == IdProjeto && x.IdUsuario == IdUsuario);
                if (salvar && projSalvo is null)
                    _db.ProjetoSalvo.Add(new ProjetoSalvo { IdProjeto = IdProjeto, IdUsuario = IdUsuario });
                else
                    _db.ProjetoSalvo.Remove(projSalvo);
                _db.SaveChanges();
            }
            else
            {
                TempData["Fail"] = "Projeto não existe";
            }
            return RedirectToPage();
        }
    }
}
