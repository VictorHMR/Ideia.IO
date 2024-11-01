using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ideia.IO.Pages.Financeiro
{
    public class DepositoModel : PageModel
    {
        public readonly DbContext _db;


        public DepositoModel(DbContext database)
        {
            _db = database;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(double Valor)
        {
            var Transacao = new Transacao
            {
                Valor = Valor,
                DtTransacao = DateTime.Now,
                IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
            };
            _db.Transacao.Add(Transacao);

            _db.SaveChanges();
            return RedirectToPage("/Index");
        }
    }
}
