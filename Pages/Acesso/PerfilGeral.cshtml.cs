using Ideia.IO.Database;
using Ideia.IO.Models;
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
                UsuarioDB.NomeCompleto = Usuario.NomeCompleto;
                UsuarioDB.Senha = Usuario.Senha;
                if (Usuario.ImgPerfilFile != null && Usuario.ImgPerfilFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Usuario.ImgPerfilFile.CopyToAsync(memoryStream);
                        UsuarioDB.ImgPerfil = memoryStream.ToArray();  
                    }
                }

                _db.SaveChanges();
            }
            return Redirect("/Acesso/PerfilGeral");
        }

    }
}
