using Ideia.IO.Models;
using Ideia.IO.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DbContext = Ideia.IO.Database.DbContext;

namespace Ideia.IO.Pages.Acesso
{
    public class RegistroModel : PageModel
    {
        public readonly DbContext _db;

        public RegistroModel(DbContext database)
        {
            _db = database;
        }
        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] Usuario? Usuario)
        {
            if(Usuario?.Senha != Usuario?.ConfirmarSenha)
            {
                ViewData["Fail"] = "As senhas não coincidem!";
                return Page();
            }
            else
            {
                Usuario? UsuarioDB = await _db.Usuario.FirstOrDefaultAsync(x => x.Email == Usuario.Email);
                if(UsuarioDB is null)
                {
                    _db.Usuario.Add(Usuario);
                    _db.SaveChanges();
                }
                else
                {
                    ViewData["Fail"] = "Email já cadastrado em outra conta!" ;
                    return Page();
                }
                return Redirect("/Acesso/Login");
            }
        }
    }
}
