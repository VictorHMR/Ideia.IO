using Ideia.IO.Models;
using Ideia.IO.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DbContext = Ideia.IO.Database.DbContext;

namespace Ideia.IO.Pages.Acesso
{
    public class LoginModel : PageModel
    {
        public readonly DbContext _db;


        public LoginModel(DbContext database)
        {
            _db = database;
        }
        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] Usuario? Usuario)
        {
            Usuario? UsuarioDB = await _db.Usuario.FirstOrDefaultAsync(x => x.Email == Usuario.Email && x.Senha == Usuario.Senha);

            if (UsuarioDB is null)
            {
                ViewData["Fail"] = "Email e/ou senha incorreto.";
                return Page();
            }

            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, UsuarioDB.Id.ToString()),
                 new Claim(ClaimTypes.Name, UsuarioDB.NomeCompleto)
            ];
            var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            var identity = new ClaimsIdentity(claims, authScheme);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(authScheme, principal);

            return RedirectToPage("/Index");

        }

    }
}
