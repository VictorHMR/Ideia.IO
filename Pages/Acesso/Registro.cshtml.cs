using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Ideia.IO.Pages.Acesso
{
    public class RegistroModel : PageModel
    {
        public readonly Database _db;

        public RegistroModel(Database database)
        {
            _db = database;
        }
        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] Usuario? Usuario)
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
