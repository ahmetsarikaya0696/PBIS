namespace Application.Features.Izinler.Queries.GetOnaylanacakIzinler
{
    public class GetOnaylanacakIzinlerResponse
    {
        public int Id { get; set; }

        public string IstekTarihi { get; set; }

        public string Unvan { get; set; }

        public string AdSoyad { get; set; }

        public string Birim { get; set; }

        public string IzinDurumu { get; set; }

        public string DogrulamaYontemi { get; set; }
    }
}
