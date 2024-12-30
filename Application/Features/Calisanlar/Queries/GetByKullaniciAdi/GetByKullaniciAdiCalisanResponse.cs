namespace Application.Features.Calisanlar.Queries.GetByKullaniciAdi
{
    public class GetByKullaniciAdiCalisanResponse
    {
        public int Id { get; set; }

        public string Unvan { get; set; }

        public string Birim { get; set; }

        public string Kod { get; set; }

        public string Ad { get; set; }

        public string Soyad { get; set; }

        public string KullaniciAdi { get; set; }

        public long Tc { get; set; }

        public int SicilNo { get; set; }

        public int? LREF { get; set; }

        public string Telefon { get; set; }

        public bool BirimAmiri { get; set; }
    }
}
