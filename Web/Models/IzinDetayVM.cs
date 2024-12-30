using Application.Features.Izinler.Queries.GetIzinDetayById;

namespace Web.Models
{
    public class IzinDetayVM
    {
        // İzin Bilgileri
        public int Id { get; set; }

        public string IzinTur { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }

        public int GunSayisi { get; set; }

        public int? MahsubenGunSayisi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string YerineBakacakKisi { get; set; }

        public string Aciklama { get; set; }

        public string YillikIzinUcretiIstegi { get; set; }

        public int KalanSenelikIzinGunSayisi { get; set; }


        // Kişisel Bilgiler
        public string Fotograf { get; set; }

        public string Unvan { get; set; }

        public string AdSoyad { get; set; }

        public string SicilNo { get; set; }

        public string Birim { get; set; }

        // İletişim
        public string Telefon { get; set; }

        public string Adres { get; set; }

        // Ret Formu
        public int RetSebepId { get; set; }
        public string Detay { get; set; }

        // Mahsuben İzin Tarihleri için gerekli alanlar
        public bool PersonelSubeYetkisi { get; set; }
        public bool SenelikIzinMi { get; set; }

        // Düzenle ve Onayla Alanları
        public DateTime? MahsubenBaslangicTarihi { get; set; }

        // Aynı Bölümdeki İzin İsteklerinin Listesi
        public List<KesisenIzin> KesisenIzinler { get; set; }
    }
}
