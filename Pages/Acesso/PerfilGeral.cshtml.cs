using Ideia.IO.Database;
using Ideia.IO.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ideia.IO.Pages.Acesso
{
    [Authorize]
    public class PerfilGeralModel : PageModel
    {
        public readonly DbContext _db;

        [BindProperty]
        public Usuario? Usuario { get; set; }

        public PerfilGeralModel(DbContext database)
        {
            _db = database;
        }

        public void OnGet()
        {
            Usuario = _db.Usuario.Find(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Usuario UsuarioDB = _db.Usuario.Find(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            if(UsuarioDB is not null)
            {
                if(Usuario.Senha is not null)
                {
                    if(UsuarioDB.Senha != Usuario.Senha)
                    {
                        ViewData["Fail"] = "Senha incorreta";
                        return Page();
                    }
                    if (Usuario.NovaSenha != Usuario.ConfirmarSenha)
                    {
                        ViewData["Fail"] = "As senhas n�o coincidem";
                        return Page();
                    }
                    UsuarioDB.Senha = Usuario.NovaSenha;
                }
                var user = (ClaimsIdentity)User.Identity;
                
                if (UsuarioDB.NomeUsuario != Usuario.NomeUsuario)
                {

                    var oldClaim = user.FindFirst(ClaimTypes.Name);
                    if (oldClaim != null)
                    {
                        user.RemoveClaim(oldClaim);
                    }

                    user.AddClaim(new Claim(ClaimTypes.Name, Usuario.NomeUsuario));

                }

                UsuarioDB.NomeCompleto = Usuario.NomeCompleto;
                UsuarioDB.NomeUsuario = Usuario.NomeUsuario;
                UsuarioDB.Telefone = Usuario.Telefone;
                if (Usuario.ImgPerfilFile != null && Usuario.ImgPerfilFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Usuario.ImgPerfilFile.CopyToAsync(memoryStream);
                        UsuarioDB.ImgPerfil = memoryStream.ToArray();  
                    }
                }

                _db.SaveChanges();

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(user));

            }
            return Redirect("/Acesso/PerfilGeral");
        }

    }
}
