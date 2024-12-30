namespace Application.Features.IzinGruplari.Queries.GetAllIzinGrup
{
    public class GetAllIzinGrupResponse
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public string UnvanAdSoyad { get; set; }

        public string Unvan { get; set; }

        public string Birim { get; set; }

        public string Isyeri { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }
    }
}
