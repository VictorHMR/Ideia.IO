namespace Ideia.IO.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public double Valor { get; set; }
        public DateTime DtTransacao { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdProjeto{ get; set; }
    }
}
