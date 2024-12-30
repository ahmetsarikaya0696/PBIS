namespace Application.Features.Izinler.Queries.GetIzinFormTabVerileriById
{
    public class GetIzinFormTabVerileriByIdResponse
    {
        // İzin Bilgileri
        public string IzinTur { get; set; }

        public string IstekTarihi { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }

        public string MahsubenBaslangicTarihi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string IzinDurumu { get; set; }

        public string YerineBakacakKisi { get; set; }

        public string Aciklama { get; set; }

        public string YillikIzinUcretiIstegi { get; set; }


        // Kişisel Bilgiler
        public string Unvan { get; set; }

        public string AdSoyad { get; set; }

        public string SicilNo { get; set; }

        public string Isyeri { get; set; }

        public string Birim { get; set; }


        // İletişim
        public string Telefon { get; set; }

        public string Adres { get; set; }

        public char FormTip { get; set; }


        // Gün Sayıları
        public int ToplamGunSayisi { get; set; }

        public int MahsubenGunSayisi { get; set; }

        public string GorevGrubu { get; set; }

        // Amirler
        public string BirinciOnayVerenAdSoyad { get; set; }
        public string IkinciOnayVerenAdSoyad { get; set; }
        public string MerkezMuduruAdSoyad { get; set; }
    }
}
