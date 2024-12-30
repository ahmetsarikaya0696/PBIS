namespace Application.Features.Izinler.Queries.GetIzinByBirimId
{
    public class GetIzinByOrganizasyonIdResponse
    {
        public string Unvan { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Birim { get; set; }
        public string Baslangic { get; set; }
        public string Bitis { get; set; }
        public string IzinTur { get; set; }
        public int GunSayisi { get; set; }
    }
}
