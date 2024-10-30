using Ideia.IO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Ideia.IO.Pages.Projetos
{
    public class ProjetosModel : PageModel
    {
        public readonly Database _db;

        public Projeto? Projeto { get; set; }
        [BindProperty]
        public List<ImagemProjeto>? ImagemProjeto { get; set; }
        [BindProperty]
        public List<IFormFile> ImagemsUpload { get; set; }

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
                {
                    ImagemProjeto = _db.ImagemProjeto.Where(x=> x.IdProjeto == IdProjeto).ToList();
                    ViewData["Action"] = "Edit";
                }
                else
                    return Redirect("/index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? IdProjeto, [FromForm] Projeto? projeto)
        {
            if (IdProjeto is null)
            {
                if (projeto is not null)
                {
                    projeto.DtCriacao = DateTime.Now;
                    projeto.IdUsuAutor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _db.Projeto.Add(projeto);
                    _db.SaveChanges();
                    int ordem = 1;
                    foreach (var imagem in ImagemsUpload)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imagem.CopyToAsync(memoryStream);
                            var imagemProduto = new ImagemProjeto
                            {
                                Imagem = memoryStream.ToArray(),
                                NrOrdem = ordem,
                                IdProjeto = projeto.Id
                            };
                            _db.ImagemProjeto.Add(imagemProduto);
                        }
                        ordem++;
                    }
                    _db.SaveChanges();
                    return Redirect("/Projetos/" +projeto.Id);

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

        public IActionResult OnPostDeleteImage(int IdImagem)
        {
            _db.ImagemProjeto.Remove(_db.ImagemProjeto.Find(IdImagem));
            _db.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostAddImage([FromForm]List<IFormFile> ImagemProjetofrm, int IdProjeto)
        {
            int ordem = _db.ImagemProjeto.OrderByDescending(x => x.NrOrdem).FirstOrDefault(x => x.IdProjeto == IdProjeto)?.NrOrdem ?? 1;

            foreach (var imagem in ImagemProjetofrm)
            {
                using (var memoryStream = new MemoryStream())
                {
                    imagem.CopyTo(memoryStream);
                    var imagemProjeto = new ImagemProjeto
                    {
                        Imagem = memoryStream.ToArray(),
                        NrOrdem = ordem++,
                        IdProjeto = IdProjeto
                    };
                    _db.ImagemProjeto.Add(imagemProjeto);
                }
            }

            _db.SaveChanges();
            return RedirectToPage();

        }



    }
}
