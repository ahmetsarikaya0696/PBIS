using Application.Features.IzinGruplari.Commands.Create;

namespace Web.Models
{
    public class CreateIzinGrupVM
    {
        public string Aciklama { get; set; }

        public int? CalisanId { get; set; }

        public int? UnvanId { get; set; }

        public int? BirimId { get; set; }

        public int? IsyeriId { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }

        public List<IzinOnayTanimIdWithSira> IzinOnayTanimVeSiralari { get; set; }
    }
}
