namespace Web.Models
{
    public class HomeVM
    {
        // Şu an izindeyse gelecek proplar
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
        public string IseBaslamaTarihi { get; set; }
        public int GunSayisi { get; set; }

        // Onay yetkisi olanlarda gelecek proplar
        public int KalanSenelikIzinGunSayisi { get; set; }
        public int GelenIzinTalepSayisi { get; set; }
        public int IslemYapilanIzinSayisi { get; set; }
    }
}
