namespace Web.Models
{
    public class TextTipIzinUpdateVM
    {
        public int Id { get; set; }

        public int IzinTurId { get; set; }

        public string BaslangicTarihi { get; set; }

        public int GunSayisi { get; set; }

        public string BitisTarihi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string Aciklama { get; set; }

        public string DogrulamaYontemi { get; set; }

        public string Kod { get; set; }
    }
}
