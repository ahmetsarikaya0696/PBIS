namespace Web.Models
{
    public class FormTipIzinUpdateVM
    {
        public int Id { get; set; }

        public int IzinTurId { get; set; }

        public string BaslangicTarihi { get; set; }

        public int GunSayisi { get; set; }

        public int? MahsubenGunSayisi { get; set; }

        public string BitisTarihi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string MahsubenBaslangicTarihi { get; set; }

        public string YerineBakacakKisi { get; set; }

        public string Adres { get; set; }

        public string Telefon { get; set; }

        public bool YillikIzinUcretiIstegi { get; set; }

        public string DogrulamaYontemi { get; set; }

        public string Kod { get; set; }
    }
}
